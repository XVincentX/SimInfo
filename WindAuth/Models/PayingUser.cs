using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WindAuth.Models
{
    public class PayingUser
    {
        [Key, StringLength(50), MaxLength(50)]
        public string Username { get; set; }
        public int Count { get; set; }
    }
}