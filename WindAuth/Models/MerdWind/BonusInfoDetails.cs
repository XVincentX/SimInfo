﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WindDataLib
{

    public class BonusInfoDetails
    {

        [JsonProperty("consumo-dati")]
        public object ConsumoDati { get; set; }

        [JsonProperty("consumo-euro")]
        public object ConsumoEuro { get; set; }

        [JsonProperty("consumo-min")]
        public string ConsumoMin { get; set; }

        [JsonProperty("consumo-sms")]
        public string ConsumoSms { get; set; }

        [JsonProperty("expiry")]
        public string Expiry { get; set; }

        [JsonProperty("residuo-dati")]
        public object ResiduoDati { get; set; }

        [JsonProperty("residuo-euro")]
        public object ResiduoEuro { get; set; }

        [JsonProperty("residuo-min")]
        public string ResiduoMin { get; set; }

        [JsonProperty("residuo-sms")]
        public string ResiduoSms { get; set; }
    }

}
