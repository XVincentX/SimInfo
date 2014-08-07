using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WindAuth.Models
{
    public class NotificationUri
    {
        public string ChannelUri { get; set; }
        [Key, StringLength(400), MaxLength(400)]
        public string stringNumbers { get; set; }

        [NotMapped]
        public IEnumerable<PushLogData> Numbers
        {
            get
            {
                return JsonConvert.DeserializeObject<IEnumerable<PushLogData>>(stringNumbers);
            }
        }
    }
}