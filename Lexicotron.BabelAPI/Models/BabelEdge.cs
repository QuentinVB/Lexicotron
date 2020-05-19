using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lexicotron.BabelAPI.Models
{
    public partial class BabelEdge
    {
        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("pointer")]
        public Pointer Pointer { get; set; }

        [JsonProperty("target")]
        public string Target { get; set; }

        [JsonProperty("weight")]
        public long Weight { get; set; }

        [JsonProperty("normalizedWeight")]
        public long NormalizedWeight { get; set; }
    }

    public partial class Pointer
    {
        [JsonProperty("fSymbol")]
        public string FSymbol { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("shortName")]
        public string ShortName { get; set; }

        [JsonProperty("relationGroup")]
        public string RelationGroup { get; set; }

        [JsonProperty("isAutomatic")]
        public bool IsAutomatic { get; set; }
    }
}
