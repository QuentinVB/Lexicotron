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
    /// <summary>
    /// The model of Babel Edge from BabelNET API, it describe extending concept (synset) from a requested synset
    /// </summary>
    public partial class BabelEdge
    {
        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("pointer")]
        public Pointer Pointer { get; set; }

        [JsonProperty("target")]//synset
        public string Target { get; set; }

        [JsonProperty("weight")]
        public long Weight { get; set; }

        [JsonProperty("normalizedWeight")]
        public long NormalizedWeight { get; set; }
    }
    /// <summary>
    /// Sub mode for Babel Edge, it describe the targeted concept of the edge (a synset)
    /// </summary>
    public partial class Pointer
    {
        [JsonProperty("fSymbol")]
        public string FSymbol { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("shortName")]
        public string ShortName { get; set; }

        [JsonProperty("relationGroup")]
        /* Possible group relation :
            HYPERNYM
            OTHER
            HOLONYM
            HYPONYM
        */
        public string RelationGroup { get; set; }

        [JsonProperty("isAutomatic")]
        public bool IsAutomatic { get; set; }
    }
}
///JSON Example
/*
 {
        "language": "EN",
        "pointer": {
            "fSymbol": "+",
            "name": "Derivationally related form",
            "shortName": "deriv",
            "relationGroup": "OTHER",
            "isAutomatic": false
        },
        "target": "bn:00059674n",
        "weight": 0.0,
        "normalizedWeight": 0.0
    },
 */
