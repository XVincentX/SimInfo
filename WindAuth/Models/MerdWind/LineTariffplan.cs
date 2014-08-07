﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WindDataLib
{

    public class LineTariffplan
    {

        [JsonProperty("activation-date")]
        public string ActivationDate { get; set; }

        [JsonProperty("activation-date-plan")]
        public string ActivationDatePlan { get; set; }

        [JsonProperty("buttons")]
        public string Buttons { get; set; }

        [JsonProperty("consensus-mobile")]
        public string ConsensusMobile { get; set; }

        [JsonProperty("consensus-others")]
        public string ConsensusOthers { get; set; }

        [JsonProperty("consensus-wind")]
        public string ConsensusWind { get; set; }

        [JsonProperty("contract-code")]
        public string ContractCode { get; set; }

        [JsonProperty("msisdn")]
        public string Msisdn { get; set; }

        [JsonProperty("payment-type")]
        public string PaymentType { get; set; }

        [JsonProperty("rowid")]
        public string Rowid { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("tariffplan-code")]
        public string TariffplanCode { get; set; }

        [JsonProperty("tariffplan-name")]
        public string TariffplanName { get; set; }

        [JsonProperty("contract-crm-id")]
        public string ContractCrmId { get; set; }

        [JsonProperty("crm-id")]
        public long CrmId { get; set; }

        [JsonProperty("CheckIncludedTraffic")]
        public string CheckIncludedTraffic { get; set; }

        [JsonProperty("ShapingInfo")]
        public string ShapingInfo { get; set; }

        [JsonProperty("CheckIncludedTrafficInfoDetails")]
        public BonusInfoDetails CheckIncludedTrafficInfoDetails { get; set; }

        [JsonProperty("line-type")]
        public string LineType { get; set; }

        [JsonProperty("parameters")]
        public Parameter[] Parameters { get; set; }

        [JsonProperty("ShapingInfoDetails")]
        public ShapingInfoDetails ShapingInfoDetails { get; set; }
    }

}
