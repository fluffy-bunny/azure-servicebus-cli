namespace ServiceBusCLI.Utils
{
    public interface ISecurityAccessSignatureProviderAssessor
    {
        ISecurityAccessSignatureProvider SecurityAccessSignatureProvider { get;  }
    }
}