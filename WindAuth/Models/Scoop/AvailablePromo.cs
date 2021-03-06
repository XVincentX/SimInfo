﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WindDataLib.Scoop
{

    public class AvailablePromo
    {

        [JsonProperty("AppliedFee")]
        public decimal AppliedFee { get; set; }

        [JsonProperty("RegimeFee")]
        public int RegimeFee { get; set; }

        [JsonProperty("ActivationFee")]
        public decimal ActivationFee { get; set; }

        [JsonProperty("DerivedPromo")]
        public int DerivedPromo { get; set; }

        [JsonProperty("Tag")]
        public object Tag { get; set; }

        [JsonProperty("Suspendible")]
        public object Suspendible { get; set; }

        [JsonProperty("PromoID")]
        public int PromoID { get; set; }

        [JsonProperty("DiscountMonths")]
        public object DiscountMonths { get; set; }

        [JsonProperty("RecurringFee")]
        public decimal RecurringFee { get; set; }

        [JsonProperty("PromoName")]
        public string PromoName { get; set; }
    }

}
