using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LTILAB1
{
    public class DHCPServer
    {
        [JsonPropertyName(".id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("interface")]
        public string Interface { get; set; }

        [JsonPropertyName("address-pool")]
        public string AddressPool { get; set; }

        [JsonPropertyName("lease-time")]
        public string LeaseTime { get; set; }

        [JsonPropertyName("disabled")]
        public string Disabled { get; set; } // MikroTik returns "true"/"false" as strings

        [JsonPropertyName("dynamic")]
        public string Dynamic { get; set; }

        [JsonPropertyName("invalid")]
        public string Invalid { get; set; }

        [JsonPropertyName("address-lists")]
        public string AddressLists { get; set; }

        [JsonPropertyName("dynamic-lease-identifiers")]
        public string DynamicLeaseIdentifiers { get; set; }

        [JsonPropertyName("lease-script")]
        public string LeaseScript { get; set; }

        [JsonPropertyName("support-broadband-tr101")]
        public string SupportBroadbandTr101 { get; set; }

        [JsonPropertyName("use-radius")]
        public string UseRadius { get; set; }

        [JsonPropertyName("use-reconfigure")]
        public string UseReconfigure { get; set; }

    }
}
