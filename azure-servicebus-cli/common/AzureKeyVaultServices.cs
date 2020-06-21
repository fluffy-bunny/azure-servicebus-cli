using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Common.Extensions;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Azure.Management.CosmosDB.Fluent.GremlinGraph.Update;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class AzureKeyVaultServices : IAzureKeyVaultServices
    {
        internal static class Constants
        {
            public static class CurveOids
            {
                public const string P256 = "1.2.840.10045.3.1.7";
                public const string P384 = "1.3.132.0.34";
                public const string P521 = "1.3.132.0.35";
            }
        }

        private IAzureKeyVaultClients _azureKeyVaultClients;
        private ISerializer _serializer;
        private ILogger<AzureKeyVaultServices> _logger;

        public AzureKeyVaultServices(IAzureKeyVaultClients azureKeyVaultClients, ISerializer serializer, 
            ILogger<AzureKeyVaultServices> logger)
        {
            _azureKeyVaultClients = azureKeyVaultClients;
            _serializer = serializer;
            _logger = logger;
        }
        internal static string GetCrvValueFromCurve(ECCurve curve)
        {
            return curve.Oid.Value switch
            {
                Constants.CurveOids.P256 => JsonWebKeyECTypes.P256,
                Constants.CurveOids.P384 => JsonWebKeyECTypes.P384,
                Constants.CurveOids.P521 => JsonWebKeyECTypes.P521,
                _ => throw new InvalidOperationException($"Unsupported curve type of {curve.Oid.Value} - {curve.Oid.FriendlyName}"),
            };
        }

        public async Task<JwksDiscoveryDocument> GetECDsaJwksDiscoveryDocumentAsync(string keyVaultName, string keyName)
        {
            var uctNow = DateTime.UtcNow;
            var keyVaultUrl = $"https://{keyVaultName}.vault.azure.net/";
            var keyClient = _azureKeyVaultClients.CreateKeyClient(keyVaultUrl);
            var propertiesOfKeyVersionsAsync = keyClient.GetPropertiesOfKeyVersionsAsync(keyName);
            var pages = propertiesOfKeyVersionsAsync.AsPages();
            var securityKeyInfos = new List<SecurityKeyInfo>();
            var result = new List<IdentityServer4.Models.JsonWebKey>();
 

            await foreach (var page in pages)
            {
                var keyProperties = page.Values;
                foreach (var prop in keyProperties)
                {
                    if (uctNow < prop.ExpiresOn)
                    {

                        var key = await keyClient.GetKeyAsync(prop.Name, prop.Version);
                        var ecDsa = key.Value.Key.ToECDsa();
                        var securityKey = new ECDsaSecurityKey(ecDsa) { KeyId = prop.Version };
                        var algorithm = "";

                        if (key.Value.Key.CurveName == KeyCurveName.P256)
                        {
                            algorithm = "ES256";
                        }
                        else if (key.Value.Key.CurveName == KeyCurveName.P384)
                        {
                            algorithm = "ES384";
                        }
                        else if (key.Value.Key.CurveName == KeyCurveName.P521)
                        {
                            algorithm = "ES521";
                        }
                        else
                        {
                            continue;
                        }
                        securityKeyInfos.Add(new SecurityKeyInfo
                        {
                            Key = securityKey,
                            SigningAlgorithm = algorithm
                        });
                    }
                }

                foreach (var keyInfo in securityKeyInfos)
                {
                    var ecdsaKey = keyInfo.Key as ECDsaSecurityKey;

                    var parameters = ecdsaKey.ECDsa.ExportParameters(false);
                    var x = Base64Url.Encode(parameters.Q.X);
                    var y = Base64Url.Encode(parameters.Q.Y);

                    var ecdsaJsonWebKey = new IdentityServer4.Models.JsonWebKey
                    {
                        kty = "EC",
                        use = "sig",
                        kid = ecdsaKey.KeyId,
                        x = x,
                        y = y,
                        crv = GetCrvValueFromCurve(parameters.Curve),
                        alg = keyInfo.SigningAlgorithm
                    };
                    result.Add(ecdsaJsonWebKey);
                }
            }
            return new JwksDiscoveryDocument()
            {
                keys = result
            };

        }

        public async Task<string> CreateECDsaSignedJWTAsync(string keyVaultName, string keyName,
            Token token)
        {
            var keyVaultUrl = $"https://{keyVaultName}.vault.azure.net/";
            var keyClient = _azureKeyVaultClients.CreateKeyClient(keyVaultUrl);
            var preferredClient = await CreatePreferredCryptographyClientAsync(keyClient, keyName);

            var jwt = token.CreateJwtSecurityToken(null, _logger);
            string algorithm = "ES256";
            var header = Base64UrlEncoder.Encode(_serializer.Serialize(new Dictionary<string, string>()
            {
                { JwtHeaderParameterNames.Alg, algorithm },
                { JwtHeaderParameterNames.Kid, preferredClient.KeyProperties.Version },
                { JwtHeaderParameterNames.Typ, "JWT" }
            }));
            var rawDataBytes = Encoding.UTF8.GetBytes(header + "." + jwt.EncodedPayload);
            byte[] hash;
            using (var hasher = CryptoHelper.GetHashAlgorithmForSigningAlgorithm(algorithm))
            {
                hash = hasher.ComputeHash(rawDataBytes);
            }
            var signResult = await preferredClient.CryptographyClient.SignAsync(
                new SignatureAlgorithm(algorithm), hash);

            var rawSignature = Base64UrlTextEncoder.Encode(signResult.Signature);

            return $"{header}.{jwt.EncodedPayload}.{rawSignature}";


        }
      
        public async Task<PreferredCryptographyClient> CreatePreferredCryptographyClientAsync(
            KeyClient keyClient, string keyName)
        {
            var utcNow = DateTimeOffset.UtcNow;
            DateTimeOffset? currentStartOn = DateTimeOffset.UtcNow.Subtract(new TimeSpan(10000,0,0,0));
            var propertiesOfKeyVersionsAsync = keyClient.GetPropertiesOfKeyVersionsAsync(keyName);
            var pages = propertiesOfKeyVersionsAsync.AsPages();
            var securityKeyInfos = new List<SecurityKeyInfo>();
            var result = new List<IdentityServer4.Models.JsonWebKey>();

            KeyProperties currentKeyProperties = null;

            await foreach (var page in pages)
            {
                var keyProperties = page.Values;
                foreach (var prop in keyProperties)
                {
                    if (utcNow < prop.ExpiresOn && prop.NotBefore <= utcNow)
                    {
                        if (prop.NotBefore > currentStartOn)
                        {
                            currentStartOn = prop.NotBefore;
                            currentKeyProperties = prop;
                        }
                    }
                }
            }
            var cryptoClient = await _azureKeyVaultClients.CreateCryptographyClientAsync(keyClient, keyName, currentKeyProperties.Version);
            return new PreferredCryptographyClient
            {
                KeyProperties = currentKeyProperties,
                CryptographyClient = cryptoClient
            };
        }
    }
}
