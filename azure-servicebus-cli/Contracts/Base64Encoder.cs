namespace Contracts
{
    public class Base64Encoder : IBase64Encoder
    {
        public string Decode(string value)
        {
            return value.Base64Decode();
        }

        public string Encode(string value)
        {
            return value.Base64Encode();
        }
    }
}
