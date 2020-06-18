using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Common.Models
{
    public class VirtualMachineScaleSetHandle
    {
        public class SkuHandle
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("tier")]
            public string Tier { get; set; }

            [JsonPropertyName("capacity")]
            public long Capacity { get; set; }
        }

        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("location")]
        public string Location { get; set; }
        [JsonPropertyName("sku")]
        public SkuHandle Sku { get; set; }
    }

   
}
