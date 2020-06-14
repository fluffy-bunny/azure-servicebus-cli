using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Contracts
{
    public static class CommonExtensions
    {
        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Encode<T>(this T obj) 
            where T: class
        {
            var json = obj.ToJson();
            var encoded = json.Base64Encode();
            return encoded;
        }
 
        public static string ToJson<T>(this T obj, bool writeIndented = false) where T: class
        {
            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = writeIndented });
            return json;
        }

        public static T FromJson<T>(this string json)
            where T : class
        {
            var obj = JsonSerializer.Deserialize<T>(json);
            return obj;
        }

        public static T Base64Decode<T>(this string base64EncodedData) 
            where T : class
        {
            var json = base64EncodedData.Base64Decode();
            var obj = json.FromJson<T>();
            return obj;
        }

        

    }
}
