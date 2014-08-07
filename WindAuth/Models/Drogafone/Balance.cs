using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WindAuth.Models.Drogafone
{
    public class Action
    {
        public string caption { get; set; }
        public string type { get; set; }
        public string web_url { get; set; }
        public string web_title { get; set; }
    }

    public class BalanceResult
    {
        public string tariffmodel { get; set; }
        public float? balance { get; set; }
        public string currency { get; set; }
        public Action action { get; set; }
        public string expiration_date { get; set; }
    }

    public class Balance
    {
        public int code { get; set; }
        public string description { get; set; }
        public BalanceResult result { get; set; }
    }
}