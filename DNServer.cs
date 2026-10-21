using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;

namespace LTILAB1
{
   
    public class DNServer
    {
        [JsonPropertyName("address-list-extra-time")]
        public string AddressListExtraTime { get; set; }

        [JsonPropertyName("allow-remote-requests")]
        public string AllowRemoteRequests { get; set; } // Vem como string "true"/"false"

        [JsonPropertyName("cache-max-ttl")]
        public string CacheMaxTtl { get; set; }

        [JsonPropertyName("cache-size")]
        public string CacheSize { get; set; }

        [JsonPropertyName("cache-used")]
        public string CacheUsed { get; set; }

        [JsonPropertyName("doh-max-concurrent-queries")]
        public string DohMaxConcurrentQueries { get; set; }

        [JsonPropertyName("doh-max-server-connections")]
        public string DohMaxServerConnections { get; set; }

        [JsonPropertyName("doh-timeout")]
        public string DohTimeout { get; set; }

        [JsonPropertyName("dynamic-servers")]
        public string DynamicServers { get; set; }

        [JsonPropertyName("max-concurrent-queries")]
        public string MaxConcurrentQueries { get; set; }

        [JsonPropertyName("max-concurrent-tcp-sessions")]
        public string MaxConcurrentTcpSessions { get; set; }

        [JsonPropertyName("max-udp-packet-size")]
        public string MaxUdpPacketSize { get; set; }

        [JsonPropertyName("mdns-repeat-ifaces")]
        public string MdnsRepeatIfaces { get; set; }

        [JsonPropertyName("query-server-timeout")]
        public string QueryServerTimeout { get; set; }

        [JsonPropertyName("query-total-timeout")]
        public string QueryTotalTimeout { get; set; }

        [JsonPropertyName("servers")]
        public string Servers { get; set; }

        [JsonPropertyName("use-doh-server")]
        public string UseDohServer { get; set; }

        [JsonPropertyName("verify-doh-cert")]
        public string VerifyDohCert { get; set; }

        [JsonPropertyName("vrf")]
        public string Vrf { get; set; }
    }
}
//".id":  "*2",
//                      "address":  "10.20.139.137",
//                      "disabled":  "false",
//                      "dynamic":  "false",
//                      "name":  "lti.com.pt",
//                      "ttl":  "1d",
//                      "type":  "A"