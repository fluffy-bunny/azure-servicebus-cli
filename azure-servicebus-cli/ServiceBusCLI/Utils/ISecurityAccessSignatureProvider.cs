namespace ServiceBusCLI.Utils
{
    public interface ISecurityAccessSignatureProvider
    {
        string GenerateSecurityAccessSignature(int secondsTTL);
    }
}