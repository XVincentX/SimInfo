using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WindAuth.Models.Drogafone
{
    public class Counter
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string loading_type { get; set; }
        public string info_url { get; set; }
    }

    public class Result
    {
        public List<Counter> counters { get; set; }
    }

    public class Counters
    {
        public int code { get; set; }
        public string description { get; set; }
        public Result result { get; set; }
    }
}