using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LTILAB1
{
    public class WGInterface
    {
        [JsonPropertyName(".id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("disabled")]
        public string Disabled { get; set; } // MikroTik returns "true"/"false" as strings

        [JsonPropertyName("listen-port")]
        public string ListenPort { get; set; }

        [JsonPropertyName("mtu")]
        public string Mtu { get; set; }

        [JsonPropertyName("private-key")]
        public string PrivateKey { get; set; }

        [JsonPropertyName("public-key")]
        public string PublicKey { get; set; }

        [JsonPropertyName("running")]
        public string Running { get; set; }
    }
}
