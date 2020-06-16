using System.Threading.Tasks;

namespace Common
{
    public interface IAzureManagementTokenProvider
    {
        Task<string> AcquireAccessTokenAsync();
    };
}
