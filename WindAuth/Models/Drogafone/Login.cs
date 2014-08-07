using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WindAuth.Models.Drogafone
{
    public class Sim
    {
        public string msisdn { get; set; }
    }

    public class IntResult
    {
        public string username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public List<Sim> sims { get; set; }
    }

    public class LoginResult
    {
        public int code { get; set; }
        public string description { get; set; }
        public string message { get; set; }
        public IntResult result { get; set; }
    }
}