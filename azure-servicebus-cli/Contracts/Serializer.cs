using System.Text.Json;

namespace Contracts
{
    public class Serializer : ISerializer
    {
        public T Deserialize<T>(string text) where T : class
        {
            return JsonSerializer.Deserialize<T>(text);
        }

        public string Serialize<T>(T obj) where T : class
        {
            return JsonSerializer.Serialize(obj);
        }
    }
}
