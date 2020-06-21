using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Common
{
    public interface ISignatureProvider
    {
        Task<string> CreateJwtAsync(JwtSecurityToken token);
    }
    public interface IECDsaSignatureProvider : ISignatureProvider { }
    public class ECDsaSignatureProvider : IECDsaSignatureProvider
    {
        public Task<string> CreateJwtAsync(JwtSecurityToken token)
        {
            throw new System.NotImplementedException();
        }
    }
    public class AzureClient
    {
        public AzureCredentials AzureCredentials { get; set; }
        public IAzure AzureInstance { get; set; }

    }
}
