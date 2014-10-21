using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace WindAuth.Data
{
    [Table("LoggedUsers")]
    public class LoggedUser
    {
        public LoggedUser()
        {
            LastLogin = DateTime.Now;
        }

        [Key, StringLength(30)]
        public string Username { get; set; }
        [StringLength(20)]
        public string Password { get; set; }

        [StringLength(1)]
        public string Type { get; set; }

        public Guid device_id { get; set; }

        public DateTime LastLogin { get; set; }
    }
}
