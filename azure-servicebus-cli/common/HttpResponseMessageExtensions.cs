using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common
{
    public static class HttpResponseMessageExtensions
    {
        class HeaderHandle
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
        class HttpResponseMessageHandle
        {
            public HttpStatusCode StatusCode { get; set; }
            public List<HeaderHandle> Headers { get; set; }
            public object Content { get; internal set; }
        }
        public static async Task<string> PrettyJsonAsync(this HttpResponseMessage httpResponseMessage, ISerializer serializer)
        {
            var jsonContent = await httpResponseMessage.Content.ReadAsStringAsync();
      
          
            var handle = new HttpResponseMessageHandle
            {
                StatusCode = httpResponseMessage.StatusCode,
                Headers = new List<HeaderHandle>(),
                Content = serializer.Deserialize<object>(jsonContent)
            };
         
            foreach(var header in httpResponseMessage.Headers)
            {
                foreach(var hv in header.Value)
                {

                    handle.Headers.Add(new HeaderHandle
                    {
                        Key = header.Key,Value=hv
                    });
                }
               
            }
            var json = serializer.Serialize(handle, indent: true);
            return json;

        }
    }
}
