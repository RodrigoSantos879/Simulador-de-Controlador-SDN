using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTILAB1
{
    public class Address
    {
        [System.Text.Json.Serialization.JsonPropertyName(".id")]
        public string Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("address")]
        public string address { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("interface")]
        public string iNTERFACE { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("network")]
        public string network { get; set; }

    }
}
