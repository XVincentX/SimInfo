using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WindAuth.Models
{
    public class LoginData
    {
        public string username { get; set; }
        public string password { get; set; }
        public bool remember { get;set; }
    }
}