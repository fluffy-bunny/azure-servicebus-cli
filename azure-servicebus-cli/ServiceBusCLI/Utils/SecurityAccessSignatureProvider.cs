using ServiceBusCLI.Features.GenerateSecurityAccessSignature;

namespace ServiceBusCLI.Utils
{
    public class SecurityAccessSignatureProvider : ISecurityAccessSignatureProvider
    {

        public SecurityAccessSignatureProvider(AppSettings<GenerateSecurityAccessSignature.SecurityAccessSignature> sasSettings)
        {
            SasSettings = sasSettings.Load(Features.GenerateSecurityAccessSignature.GenerateSecurityAccessSignature.SettingsFileName); 
        }

        public GenerateSecurityAccessSignature.SecurityAccessSignature SasSettings { get; }

        public string GenerateSecurityAccessSignature(int secondsTTL)
        {
            return SeviceBusSecurityAccessSignatureGenerator.GenerateSecurityAccessSignature(
                SasSettings.Namespace, SasSettings.Queue, SasSettings.Key, SasSettings.Policy, new System.TimeSpan(0, 0, secondsTTL)
                );
        }
    }
}