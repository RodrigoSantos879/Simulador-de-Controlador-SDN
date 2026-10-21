using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTILAB1
{
    public class Route
    {
        [System.Text.Json.Serialization.JsonPropertyName(".id")]
        public string Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("dst-address")]
        public string DstAddress { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("gateway")]
        public string Gateway { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("distance")]
        public string Distance { get; set; }
    }
}
