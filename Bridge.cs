using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LTILAB1
{
    public class Bridge
    {
        [JsonPropertyName(".id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("actual-mtu")]
        public string ActualMtu { get; set; }

        [JsonPropertyName("ageing-time")]
        public string AgeingTime { get; set; }

        [JsonPropertyName("arp")]
        public string Arp { get; set; }

        [JsonPropertyName("arp-timeout")]
        public string ArpTimeout { get; set; }

        [JsonPropertyName("auto-mac")]
        public string AutoMac { get; set; }

        [JsonPropertyName("dhcp-snooping")]
        public string DhcpSnooping { get; set; }

        [JsonPropertyName("disabled")]
        public string Disabled { get; set; }

        [JsonPropertyName("dynamic")]
        public string Dynamic { get; set; }

        [JsonPropertyName("fast-forward")]
        public string FastForward { get; set; }

        [JsonPropertyName("forward-delay")]
        public string ForwardDelay { get; set; }

        [JsonPropertyName("igmp-snooping")]
        public string IgmpSnooping { get; set; }

        [JsonPropertyName("l2mtu")]
        public string L2Mtu { get; set; }

        [JsonPropertyName("mac-address")]
        public string MacAddress { get; set; }

        [JsonPropertyName("managed")]
        public string Managed { get; set; }

        [JsonPropertyName("max-learned-entries")]
        public string MaxLearnedEntries { get; set; }

        [JsonPropertyName("max-message-age")]
        public string MaxMessageAge { get; set; }

        [JsonPropertyName("mlag-heartbeat")]
        public string MlagHeartbeat { get; set; }

        [JsonPropertyName("mlag-peer-port")]
        public string MlagPeerPort { get; set; }

        [JsonPropertyName("mlag-priority")]
        public string MlagPriority { get; set; }

        [JsonPropertyName("mtu")]
        public string Mtu { get; set; }

        [JsonPropertyName("port-cost-mode")]
        public string PortCostMode { get; set; }

        [JsonPropertyName("priority")]
        public string Priority { get; set; }

        [JsonPropertyName("protocol-mode")]
        public string ProtocolMode { get; set; }

        [JsonPropertyName("ra-guard")]
        public string RaGuard { get; set; }

        [JsonPropertyName("running")]
        public string Running { get; set; }

        [JsonPropertyName("transmit-hold-count")]
        public string TransmitHoldCount { get; set; }

        [JsonPropertyName("vlan-filtering")]
        public string VlanFiltering { get; set; }
    }
}

