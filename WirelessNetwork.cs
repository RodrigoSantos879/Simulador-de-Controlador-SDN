using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace LTILAB1
{
    public class WirelessNetwork
    {
        [JsonPropertyName(".id")]
        public string Id { get; set; }

        [JsonPropertyName("area")]
        public string Area { get; set; }

        [JsonPropertyName("arp")]
        public string Arp { get; set; }

        [JsonPropertyName("arp-timeout")]
        public string ArpTimeout { get; set; }

        [JsonPropertyName("bridge-mode")]
        public string BridgeMode { get; set; }

        [JsonPropertyName("default-ap-tx-limit")]
        public string DefaultApTxLimit { get; set; }

        [JsonPropertyName("default-authentication")]
        public string DefaultAuthentication { get; set; }

        [JsonPropertyName("default-client-tx-limit")]
        public string DefaultClientTxLimit { get; set; }

        [JsonPropertyName("default-forwarding")]
        public string DefaultForwarding { get; set; }

        [JsonPropertyName("disable-running-check")]
        public string DisableRunningCheck { get; set; }

        [JsonPropertyName("disabled")]
        public string Disabled { get; set; }

        [JsonPropertyName("hide-ssid")]
        public string HideSsid { get; set; }

        [JsonPropertyName("interface-type")]
        public string InterfaceType { get; set; }

        [JsonPropertyName("interworking-profile")]
        public string InterworkingProfile { get; set; }

        [JsonPropertyName("keepalive-frames")]
        public string KeepaliveFrames { get; set; }

        [JsonPropertyName("l2mtu")]
        public string L2mtu { get; set; }

        [JsonPropertyName("mac-address")]
        public string MacAddress { get; set; }

        [JsonPropertyName("master-interface")]
        public string MasterInterface { get; set; }

        [JsonPropertyName("max-station-count")]
        public string MaxStationCount { get; set; }

        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        [JsonPropertyName("mtu")]
        public string Mtu { get; set; }

        [JsonPropertyName("multicast-buffering")]
        public string MulticastBuffering { get; set; }

        [JsonPropertyName("multicast-helper")]
        public string MulticastHelper { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("running")]
        public string Running { get; set; }

        [JsonPropertyName("security-profile")]
        public string SecurityProfile { get; set; }

        [JsonPropertyName("ssid")]
        public string Ssid { get; set; }

        [JsonPropertyName("station-bridge-clone-mac")]
        public string StationBridgeCloneMac { get; set; }

        [JsonPropertyName("station-roaming")]
        public string StationRoaming { get; set; }

        [JsonPropertyName("update-stats-interval")]
        public string UpdateStatsInterval { get; set; }

        [JsonPropertyName("vlan-id")]
        public string VlanId { get; set; }

        [JsonPropertyName("vlan-mode")]
        public string VlanMode { get; set; }

        [JsonPropertyName("wds-cost-range")]
        public string WdsCostRange { get; set; }

        [JsonPropertyName("wds-default-bridge")]
        public string WdsDefaultBridge { get; set; }

        [JsonPropertyName("wds-default-cost")]
        public string WdsDefaultCost { get; set; }

        [JsonPropertyName("wds-ignore-ssid")]
        public string WdsIgnoreSsid { get; set; }

        [JsonPropertyName("wds-mode")]
        public string WdsMode { get; set; }

        [JsonPropertyName("wmm-support")]
        public string WmmSupport { get; set; }

        [JsonPropertyName("wps-mode")]
        public string WpsMode { get; set; }
    }
}
