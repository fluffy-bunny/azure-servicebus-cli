namespace ServiceBusCLI.Utils
{
    public class SecurityAccessSignatureProviderAssessor : ISecurityAccessSignatureProviderAssessor
    {
        public ISecurityAccessSignatureProvider SecurityAccessSignatureProvider { get; set; }
    }
}