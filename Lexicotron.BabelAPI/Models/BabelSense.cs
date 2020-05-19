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
    public partial class BabelSense
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }
    }

    public partial class Properties
    {
        [JsonProperty("fullLemma")]
        public string FullLemma { get; set; }

        [JsonProperty("simpleLemma")]
        public string SimpleLemma { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("senseKey")]
        //[JsonConverter(typeof(ParseStringConverter))]
        public long SenseKey { get; set; }

        [JsonProperty("frequency")]
        public long Frequency { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("pos")]
        public string Pos { get; set; }

        [JsonProperty("synsetID")]
        public SynsetId SynsetId { get; set; }

        [JsonProperty("translationInfo")]
        public string TranslationInfo { get; set; }

        [JsonProperty("pronunciations")]
        public Pronunciations Pronunciations { get; set; }

        [JsonProperty("bKeySense")]
        public bool BKeySense { get; set; }

        [JsonProperty("idSense")]
        public long IdSense { get; set; }
    }

    public partial class Pronunciations
    {
        [JsonProperty("audios")]
        public object[] Audios { get; set; }

        [JsonProperty("transcriptions")]
        public string[] Transcriptions { get; set; }
    }

    public partial class SynsetId
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("pos")]
        public string Pos { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }
    }
}

/*
Lexicotron.BabelAPI.Models 
     {
        "type": "BabelSense",
        "properties": {
            "fullLemma": "ostéopathie",
            "simpleLemma": "ostéopathie",
            "source": "WIKI",
            "senseKey": "1282469",
            "frequency": 0,
            "language": "FR",
            "pos": "NOUN",
            "synsetID": {
                "id": "bn:00059675n",
                "pos": "NOUN",
                "source": "BABELNET"
            },
            "translationInfo": "",
            "pronunciations": {
                "audios": [],
                "transcriptions": [
                    "[/ɔs.te.ɔ.pa.ti/]"
                ]
            },
            "bKeySense": false,
            "idSense": 38629179
        }
    },
     */
