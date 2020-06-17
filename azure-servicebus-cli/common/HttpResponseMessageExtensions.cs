using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;

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
        }
        public static string PrettyJson(this HttpResponseMessage httpResponseMessage, ISerializer serializer)
        {
            var handle = new HttpResponseMessageHandle
            {
                StatusCode = httpResponseMessage.StatusCode,
                Headers = new List<HeaderHandle>()
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
