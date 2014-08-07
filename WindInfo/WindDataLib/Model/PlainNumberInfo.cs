using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindDataLib.Model
{
    [JsonObject(Title = "UserPreferences")]
    public class PlainNumberInfo
    {

        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("number")]
        public string Number { get; set; }
        [JsonProperty("credit")]
        public float Credit { get; set; }
        [JsonProperty("creditlimit")]
        public float CreditLimit { get; set; }
        [JsonProperty("minutes")]
        public int Minutes { get; set; }
        [JsonProperty("minuteslimit")]
        public int MinutesLimit { get; set; }
        [JsonProperty("sms")]
        public int SMS { get; set; }
        [JsonProperty("smslimit")]
        public int SMSLimit { get; set; }
        [JsonProperty("gigabytes")]
        public int Gigabytes { get; set; }
        [JsonProperty("gigabyteslimit")]
        public float GigabytesLimit { get; set; }
        [JsonProperty("notifyenabled")]
        public bool NotifyEnabled { get; set; }
        [JsonProperty("brush")]
        public string Brush { get; set; }
        [JsonProperty("friendlyname")]
        public string FriendlyName { get; set; }
        [JsonProperty("clshowed")]
        public bool clShowed { get; set; }
        [JsonProperty("smsshowed")]
        public bool smsShowed { get; set; }
        [JsonProperty("gigashowed")]
        public bool gigaShowed { get; set; }
        [JsonProperty("minshowed")]
        public bool minShowed { get; set; }
    }
}
