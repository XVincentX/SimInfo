using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WindAuth.Models.Drogafone;
using WindDataLib;

namespace WindAuth.Code
{
    public class VodafoneRetr : BaseCreditInfoRetr
    {
        private CreditInfo inf = new CreditInfo() { NumberInfos = new ObservableCollection<NumberInfo>() };
        private string SecretKey = string.Empty;
        private string InstallationID = string.Empty;
        private string devUDID;
        private string SessionID;
        private List<Task> _tasks = new List<Task>(3);

        public VodafoneRetr(bool UseProxy)
            : base(UseProxy)
        {
        }
        public override async Task<CreditInfo> Get(string username, string password, string type, Guid dev_id)
        {
            inf.Username = username;
            inf.Password = password;
            inf.Type = type;
            devUDID = dev_id.ToString();
            httpclient.DefaultRequestHeaders.Clear();
            await RegisterInstallation();
            await StartSession();

            httpclient.DefaultRequestHeaders.Add("X-Bwb-SessionId", SessionID);
            using (var res = await httpclient.PostAsJsonAsync("https://my190.vodafone.it/api/v1/session/login", new { username = username, password = password }))
            {
                LoginResult result = await res.Content.ReadAsAsync<LoginResult>();
                if (result.description != "RESPONSE_OK")
                {
                    throw new WrongLoginDataException(result.message);
                }

                foreach (var sim in result.result.sims)
                {
                    var counters = await httpclient.GetAsync(string.Format("https://my190.vodafone.it/api/v1/sim/{0}/counters", sim.msisdn));
                    var balance = httpclient.GetAsync(string.Format("https://my190.vodafone.it/api/v1/sim/{0}/balance", sim.msisdn));

                    Counters cnts = await counters.Content.ReadAsAsync<Counters>();

                    var ninfo = new NumberInfo { Number = sim.msisdn, LastUpdate = DateTime.Now };


                    var v = new List<Value>();
                    if (cnts.result != null)
                        foreach (var counter in cnts.result.counters.Where(x =>
                            !x.id.StartsWith("3616-S")/*Soglia*/
                            && !x.id.StartsWith("502-S") /*Soglia internet*/
                            && !x.id.StartsWith("6298-S")/*Dettagli chiamate*/
                            && !x.id.StartsWith("1477-P") /*You and me*/
                            && !x.id.StartsWith("796-T")/*Hystogram*/ &&
                            !x.id.StartsWith("412-T")/*Nata cos*/))
                        {

                            var rs = await httpclient.GetAsync(string.Format("https://my190.vodafone.it/api/v1/sim/{0}/counter/{1}", sim.msisdn, counter.id));
                            var str = await rs.Content.ReadAsAsync<CounterValueObj>();

                            if (str.result != null && str.result.counter.threshold != null && str.result.counter.threshold.values != null)
                            {
                                foreach (var value in str.result.counter.threshold.values)
                                {
                                    switch (value.label.ToLower())
                                    {
                                        case "chiamate":
                                            ninfo.Minutes += (int)value.threshold - (int)value.value;
                                            ninfo.MinutesTotal += (int)value.threshold;
                                            ninfo.ExpirationDate = DateTime.ParseExact(value.period_end, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                            break;
                                        case "messaggi":
                                            ninfo.SMS += Math.Min((int)value.threshold - (int)value.value, 999);
                                            ninfo.SMSTotal += Math.Min((int)value.threshold, 999);
                                            ninfo.ExpirationDate = DateTime.ParseExact(value.period_end, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                            break;
                                        case "internet":
                                            ninfo.Gigabytes = (int)(100.0f / value.threshold * (value.threshold - value.value));
                                            ninfo.GigabytesTotal = 100;
                                            ninfo.ExpirationDate = DateTime.ParseExact(value.period_end, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }
                        }


                    Task.WaitAll(_tasks.ToArray());
                    Balance blnc = await (await balance).Content.ReadAsAsync<Balance>();
                    ninfo.Credit = blnc.result != null && blnc.result.balance.HasValue ? blnc.result.balance.Value : -1.0f;
                    inf.NumberInfos.Add(ninfo);
                }
            }


            return inf;

        }
        private async Task StartSession()
        {
            string StartSessionURL = "https://my190.vodafone.it/api/v1/session/start";

            httpclient.DefaultRequestHeaders.Add("X-Bwb-InstallationId", InstallationID);
            httpclient.DefaultRequestHeaders.Add("Authorization", Authorization);

            string postData = "{\"app_id\":\"MyVodafone\",\"app_platform\":\"Android\"," +
                "\"app_version\":\"4.03\",\"os_name\":\"Android\",\"os_version\":\"2.3.4\"," +
                "\"device_vendor\":\"" + "IPhone" + "\",\"device_model\":\"" + "3gs" +
                "\",\"screen_width\":\"320\"," +
                "\"screen_height\":\"480\",\"screen_ratio\":\"1\",\"push_provider\":\"Android\"," +
                "\"push_types\":\"broadcast-simple\",\"device_is_tablet\":false,\"device_udid\":\"" +
                devUDID + "\"}";

            var res = await httpclient.PostAsync(StartSessionURL, new StringContent(postData, Encoding.UTF8, "application/json"));
            var tmp = await res.Content.ReadAsStringAsync();

            if (tmp.Contains("RESPONSE_OK"))
            {
                SessionID = QnDJSONElement(tmp, "session_id");
            }
            else
                throw new UnauthorizedAccessException(tmp);
        }
        private async Task RegisterInstallation()
        {
            string RegistrationURL = "https://my190.vodafone.it/api/v1/client/register";
            string postData = "{\"app_id\":\"MyVodafone\",\"device_udid\":\"" +
                devUDID + "\"," +
                "\"app_platform\":\"Android\",\"app_version\":\"4.03\"}";


            var res = await httpclient.PostAsync(RegistrationURL, new StringContent(postData, Encoding.UTF8, "application/json"));
            var tmp = await res.Content.ReadAsStringAsync();
            if (tmp.Contains("RESPONSE_OK"))
            {
                InstallationID = QnDJSONElement(tmp, "installation_id");
                SecretKey = QnDJSONElement(tmp, "secret_key");
            }
            else
                throw new UnauthorizedAccessException(tmp);
        }
        private string Authorization
        {
            get
            {
                using (HMACSHA1 hs = new HMACSHA1(Encoding.UTF8.GetBytes(SecretKey)))
                {
                    byte[] bID = Encoding.UTF8.GetBytes("iid=" + InstallationID);

                    return "MYVF_V1 " + Convert.ToBase64String(
                        hs.TransformFinalBlock(bID, 0, bID.Length));
                }
            }

        }
        private static string QnDJSONElement(string json, string elemName)
        {
            json = json.Substring(json.IndexOf(elemName) + elemName.Length)
                .TrimStart('"', ':', ' ');

            return json.Substring(0, json.IndexOf('"'));
        }



        public override string Type
        {
            get { return "V"; }
        }

        public override object Clone()
        {
            return new VodafoneRetr(_useProxy);
        }
    }
}
