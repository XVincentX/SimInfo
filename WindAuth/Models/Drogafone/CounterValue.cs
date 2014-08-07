using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WindAuth.Models.Drogafone
{
    public class Value
    {
        public string label { get; set; }
        public double value { get; set; }
        public double threshold { get; set; }
        public string unit { get; set; }
        public string period_start { get; set; }
        public string period_end { get; set; }
    }

    public class Threshold
    {
        public int size { get; set; }
        public List<Value> values { get; set; }
    }

    public class CounterValue
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string period_start { get; set; }
        public string period_end { get; set; }
        public Threshold threshold { get; set; }
    }

    public class CounterValueResult
    {
        public CounterValue counter { get; set; }
    }

    public class CounterValueObj
    {
        public int code { get; set; }
        public string description { get; set; }
        public CounterValueResult result { get; set; }
    }
}