using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace LTILAB1
{
    public class SecurityProfile
    {
        [JsonPropertyName(".id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        [JsonPropertyName("authentication-types")]
        public string AuthenticationTypes { get; set; }

        [JsonPropertyName("unicast-ciphers")]
        public string UnicastCiphers { get; set; }

        [JsonPropertyName("group-ciphers")]
        public string GroupCiphers { get; set; }

        [JsonPropertyName("wpa-pre-shared-key")]
        public string WpaPreSharedKey { get; set; }

        [JsonPropertyName("wpa2-pre-shared-key")]
        public string Wpa2PreSharedKey { get; set; }

        [JsonPropertyName("management-protection")]
        public string ManagementProtection { get; set; }

        [JsonPropertyName("management-protection-key")]
        public string ManagementProtectionKey { get; set; }

        [JsonPropertyName("group-key-update")]
        public string GroupKeyUpdate { get; set; }

        [JsonPropertyName("interim-update")]
        public string InterimUpdate { get; set; }

        [JsonPropertyName("disable-pmkid")]
        public string DisablePmkid { get; set; }

        [JsonPropertyName("default")]
        public string IsDefault { get; set; }

        [JsonPropertyName("supplicant-identity")]
        public string SupplicantIdentity { get; set; }

        [JsonPropertyName("eap-methods")]
        public string EapMethods { get; set; }

        [JsonPropertyName("tls-mode")]
        public string TlsMode { get; set; }

        [JsonPropertyName("tls-certificate")]
        public string TlsCertificate { get; set; }
    }
}
