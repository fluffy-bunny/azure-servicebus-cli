using Azure.Core;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class AzureServiceTokenCredential : TokenCredential
    {
        public AzureServiceTokenCredential(string endPoint)
        {
            if (string.IsNullOrWhiteSpace(endPoint))
            {
                throw new ArgumentException("message", nameof(endPoint));
            }

            EndPoint = endPoint;
        }

        public string EndPoint { get; }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            var token = GetTokenAsync(requestContext, cancellationToken).GetAwaiter().GetResult();
            return token;
        }

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            var tokenProvider = new AzureServiceTokenProvider();
            return new ValueTask<AccessToken>(tokenProvider
                .GetAccessTokenAsync(EndPoint, null, cancellationToken)
                .ContinueWith(task => {
                    return new AccessToken(task.Result, DateTimeOffset.MaxValue);
                }));
        }
    }
}
