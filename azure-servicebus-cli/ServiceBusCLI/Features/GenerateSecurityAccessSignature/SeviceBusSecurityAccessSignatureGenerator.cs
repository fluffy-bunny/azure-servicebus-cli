using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ServiceBusCLI.Features.GenerateSecurityAccessSignature
{
    public static class SeviceBusSecurityAccessSignatureGenerator
    {
        public static string GenerateSecurityAccessSignature(string Namespace, string queueName, string key, string sbPolicy, TimeSpan tsExpiry)
        {
            var serviceUri = $"https://{Namespace}.servicebus.windows.net/{queueName}";
            var generatedSas = CreateToken(serviceUri, sbPolicy, key, tsExpiry);
            return generatedSas;
        }
        private static string CreateToken(string resourceUri, string keyName, string key, TimeSpan tsExpiry)
        {
            TimeSpan sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var expiry = Convert.ToString((int)sinceEpoch.TotalSeconds + tsExpiry.TotalSeconds);
            string stringToSign = HttpUtility.UrlEncode(resourceUri) + "\n" + expiry;
            HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));

            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            var sasToken = string.Format(CultureInfo.InvariantCulture,
            "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
                HttpUtility.UrlEncode(resourceUri), HttpUtility.UrlEncode(signature), expiry, keyName);

            return sasToken;
        }

    }
}
