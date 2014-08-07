﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WindDataLib
{

    public class LineOption
    {

        [JsonProperty("activation-date")]
        public string ActivationDate { get; set; }

        [JsonProperty("buttons")]
        public string Buttons { get; set; }

        [JsonProperty("option-code")]
        public string OptionCode { get; set; }

        [JsonProperty("option-name")]
        public string OptionName { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("parameters")]
        public Parameter[] Parameters { get; set; }

        [JsonProperty("BonusInfoDetails")]
        public BonusInfoDetails BonusInfoDetails { get; set; }

        [JsonProperty("BonusInfo")]
        public string BonusInfo { get; set; }

        [JsonProperty("ShapingInfo")]
        public string ShapingInfo { get; set; }

        [JsonProperty("ShapingInfoDetails")]
        public ShapingInfoDetails ShapingInfoDetails { get; set; }
    }

}
