using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LTILAB1
{
    public class BridgePort
    {
        [JsonPropertyName(".id")]
        public string Id { get; set; }

        [JsonPropertyName(".nextid")]
        public string NextId { get; set; }

        [JsonPropertyName("auto-isolate")]
        public string AutoIsolate { get; set; }

        [JsonPropertyName("bpdu-guard")]
        public string BpduGuard { get; set; }

        [JsonPropertyName("bridge")]
        public string BridgeName { get; set; }

        [JsonPropertyName("broadcast-flood")]
        public string BroadcastFlood { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("disabled")]
        public string Disabled { get; set; }

        [JsonPropertyName("dynamic")]
        public string Dynamic { get; set; }

        [JsonPropertyName("edge")]
        public string Edge { get; set; }

        [JsonPropertyName("fast-leave")]
        public string FastLeave { get; set; }

        [JsonPropertyName("frame-types")]
        public string FrameTypes { get; set; }

        [JsonPropertyName("horizon")]
        public string Horizon { get; set; }

        [JsonPropertyName("inactive")]
        public string Inactive { get; set; }

        [JsonPropertyName("ingress-filtering")]
        public string IngressFiltering { get; set; }

        [JsonPropertyName("interface")]
        public string InterfaceName { get; set; }

        [JsonPropertyName("learn")]
        public string Learn { get; set; }

        [JsonPropertyName("managed")]
        public string Managed { get; set; }

        [JsonPropertyName("multicast-router")]
        public string MulticastRouter { get; set; }

        [JsonPropertyName("mvrp-applicant-state")]
        public string MvrpApplicantState { get; set; }

        [JsonPropertyName("mvrp-registrar-state")]
        public string MvrpRegistrarState { get; set; }

        [JsonPropertyName("point-to-point")]
        public string PointToPoint { get; set; }

        [JsonPropertyName("priority")]
        public string Priority { get; set; }

        [JsonPropertyName("pvid")]
        public string Pvid { get; set; }

        [JsonPropertyName("restricted-role")]
        public string RestrictedRole { get; set; }

        [JsonPropertyName("restricted-tcn")]
        public string RestrictedTcn { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("tag-stacking")]
        public string TagStacking { get; set; }

        [JsonPropertyName("trusted")]
        public string Trusted { get; set; }

        [JsonPropertyName("trusted-ra")]
        public string TrustedRa { get; set; }

        [JsonPropertyName("unknown-multicast-flood")]
        public string UnknownMulticastFlood { get; set; }

        [JsonPropertyName("unknown-unicast-flood")]
        public string UnknownUnicastFlood { get; set; }
    }
}
