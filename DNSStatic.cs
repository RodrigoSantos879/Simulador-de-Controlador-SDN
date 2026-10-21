using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LTILAB1
{
    public class DNSStatic
    {
        [JsonPropertyName(".id")]
        public string Id { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("disabled")]
        public string Disabled { get; set; } // "true" ou "false"

        [JsonPropertyName("dynamic")]
        public string Dynamic { get; set; } // "true" ou "false"

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("ttl")]
        public string Ttl { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } // Ex: "A", "AAAA", "CNAME"
    }
}
