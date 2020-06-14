using System;
using System.Text.Json.Serialization;

namespace Common
{

    [Serializable()]
    public class Job
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime IssuedTime { get; set; }
    }
}
