using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using WindDataLib;

namespace WindAuth.Code
{
    public class NovercaRetr : BaseCreditInfoRetr
    {
        public NovercaRetr(bool UseProxy)
            : base(UseProxy)
        {
            ServicePointManager.DefaultConnectionLimit = 5;
        }

        private CreditInfo inf = new CreditInfo() { NumberInfos = new ObservableCollection<NumberInfo>() };
        private readonly string requestUrl = "http://www.noverca.it/get-async-data?sessionVar={0}";
        public async override System.Threading.Tasks.Task<WindDataLib.CreditInfo> Get(string username, string password, string type, Guid dev_id)
        {

            var first = await httpclient.GetAsync("http://www.noverca.it"); //get shitty cookies.
            var response = await httpclient.PostAsync("http://www.noverca.it/user-login?section=", new StringContent(string.Format("section=&username={0}&password={1}&azione=Entra", username, password), Encoding.Default, "application/x-www-form-urlencoded"));
            var htmlPage = await response.Content.ReadAsStringAsync();
            var document = CsQuery.CQ.CreateDocument(htmlPage);
            var sessionVars = document.Select("span[id^='DynContent']");

            if (!sessionVars.Any())
                throw new WrongLoginDataException("Username o password non validi.");

            inf.Username = username;
            inf.Password = password;
            inf.Type = type;

            var number = new NumberInfo { Number = username, LastUpdate = DateTime.Now };

            await ForEachAsync(sessionVars.Where(sv => sv.Id.Contains("Credit") || sv.Id.Contains("ENDDATE") || sv.Id.Contains("SMS") || sv.Id.Contains("WIFI") || sv.Id.Contains("DATA") || sv.Id.Contains("VOICE")), 5, async sessionVar =>
            {
                {
                    var svar = sessionVar.Id.Replace("DynContent", "");

                    var data = await (await httpclient.GetAsync(string.Format(requestUrl, svar))).Content.ReadAsStringAsync();

                    if (svar.Contains("Credit"))
                        number.Credit += float.Parse(data);
                    else if (svar.Contains("ENDDATE"))
                        number.ExpirationDate = DateTime.ParseExact(data, "dd/MM/yyyy", CultureInfo.GetCultureInfo("it-IT"));
                    else if (svar.Contains("SMS"))
                        number.SMS += int.Parse(data);
                    else if (svar.Contains("WIFI") && !svar.Contains("MOBILE_2013_20"))
                        number.Gigabytes += int.Parse(data.Split(':')[0]);
                    else if (svar.Contains("DATA"))
                        if (data.ToUpper().Contains("GB"))
                            number.Gigabytes += (int)(1000.0f * float.Parse(Regex.Match(data, @"[-+]?(\d*[.])?\d+").Value));
                        else
                            number.Gigabytes += int.Parse(Regex.Match(data, @"\d+").Value);
                    else if (svar.Contains("VOICE"))
                        number.Minutes += int.Parse(data.Split(':')[0]);

                }
            });

            if (sessionVars.Any(x => x.Id.Contains("MOBILE_2013_20")))
            {
                //Silverpack
                number.GigabytesTotal = 100;
                number.SMSTotal = 300;
                number.MinutesTotal = 300;
                number.Gigabytes /= 10;
            }
            else if (sessionVars.Any(x => x.Id.Contains("MOBILE_2013_24")))
            {
                //Steelpack
                number.GigabytesTotal = 100;
                number.SMSTotal = 200;
                number.MinutesTotal = 200;
                number.Gigabytes /= 10;
            }
            else if (sessionVars.Any(x => x.Id.Contains("MOBILE_2013_27")))
            {
                //Pressing

                number.GigabytesTotal = 100;
                number.SMSTotal = 100;
                number.MinutesTotal = 200;
                number.Gigabytes = (int)(100.0f / 2000.0f * (float)number.Gigabytes);
            }
            else if (sessionVars.Any(x=>x.Id.Contains("MOBILE_2014_10")))
            {
                //Happy
                number.GigabytesTotal = 100;
                number.SMSTotal = 200;
                number.MinutesTotal = 260;
                number.Gigabytes = (int)(100.0f / 1000.0f * (float)number.Gigabytes);

            }

            inf.NumberInfos.Add(number);

            return inf;
        }


        public override string Type
        {
            get { return "N"; }
        }

        public override object Clone()
        {
            return new NovercaRetr(_useProxy);
        }

        private Task ForEachAsync<T>(IEnumerable<T> source, int dop, Func<T, Task> body)
        {
            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(dop)
                select Task.Run(async delegate
                {
                    using (partition)
                        while (partition.MoveNext())
                            await body(partition.Current);
                }));
        }
    }
}