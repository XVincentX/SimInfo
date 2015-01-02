using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using WindDataLib;

namespace WindAuth.Code
{
    public class WindRetr : BaseCreditInfoRetr
    {

        public WindRetr(bool UseProxy)
            : base(UseProxy)
        {

        }
        private CreditInfo cr = new CreditInfo();
        private List<Task> tasks = new List<Task>();
        public async override Task<WindDataLib.CreditInfo> Get(string username, string password, string type, Guid dev_id)
        {
            httpclient.DefaultRequestHeaders.Clear();
            var logresposne = new JsonLogin();
            ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => { return true; }; //Validate any certificate. Who cares about security.
            using (var content = new StringContent(string.Format("handset-os=Android+4.3&handset-model=HTC+HTC+One&username={0}&password={1}", username, password)))
            {

                httpclient.DefaultRequestHeaders.Add("Accept", "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                httpclient.DefaultRequestHeaders.ExpectContinue = false;
                using (var httpstrAuthJson = await httpclient.PostAsync("https://authserv.infostrada.it/155/auth/new/LoginUidPwd", content))
                {
                    var strAuthJson = await httpstrAuthJson.Content.ReadAsStringAsync();

                    try
                    {
                        logresposne = JsonConvert.DeserializeObject<JsonLogin>(strAuthJson);
                        if (logresposne == null || logresposne.Response.Status != "0")
                            throw new WrongLoginDataException(logresposne.Response.Reason);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }

            }

            cr.Username = username;
            cr.Password = password;
            cr.Type = type;


            var continueAction = new Action<Task<HttpResponseMessage>>(async t =>
            {
                try
                {
                    using (var response = await t)
                    {
                        if (t.Status == TaskStatus.RanToCompletion && t.Exception == null)
                        {
                            var str = await response.Content.ReadAsStringAsync();
                            response.RequestMessage.Content.Dispose();
                            DeserializeInOutput(str, response.RequestMessage.RequestUri);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            });

            cr.NumberInfos = new ObservableCollection<NumberInfo>();
            foreach (var line in logresposne.Login.Lines)
            {
                var content = new StringContent(string.Format("sessionid={0}&msisdn={1}&contract-code={2}&customer-code={3}", logresposne.Login.Session.Id, line.Msisdn, line.ContractCode, logresposne.CustomerCode));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                foreach (var url in (new[] { "https://authserv.infostrada.it/155/line/LineSummary", "https://authserv.infostrada.it/155/line/BonusInfo", "https://authserv.infostrada.it/155/line/CreditBalance", "https://authserv.infostrada.it/155/text/TextCatalog", "https://authserv.infostrada.it/155/recharge/DebitsCredits", "https://authserv.infostrada.it/155/traffic/TrafficDetails", "https://authserv.infostrada.it/155/traffic/TrafficSummary", "https://authserv.infostrada.it/155/customer/CustomerDetails" }).Take(1))
                    tasks.Add(httpclient.PostAsync(new Uri(url, UriKind.Absolute), content).ContinueWith(continueAction));
            }


            Task.WaitAll(tasks.ToArray());
            if (!cr.NumberInfos.Any())
                throw new Exception("Impossibile trovare SIM disponibili");
            return cr;

        }


        private void CopyValues<T>(T target, T source)
        {
            Type t = typeof(T);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source, null);
                if (value != null)
                    prop.SetValue(target, value, null);
            }
        }

        private bool AllNull<T>(T Target)
        {
            foreach (var item in typeof(T).GetProperties().Where(prop => prop.CanRead && prop.CanWrite))
            {
                if (item.GetValue(Target) != null)
                    return false;
            }
            return true;

        }

        private void DeserializeInOutput(string str, Uri uri)
        {

            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
                return;
            var uriString = uri.ToString();

            if (uriString.Contains("LineSummary"))
            {
                var obj = JsonConvert.DeserializeObject<JsonTrafficSummary>(str);
                //ROPZ4150 All inclusive Europa e USA
                LineOption lo = new LineOption { BonusInfoDetails = new BonusInfoDetails() { Expiry = null } };


                if (obj.LineOptions != null)
                {
                    foreach (var lop in obj.LineOptions.Reverse().Where(x => x.OptionCode != "ROPZ4150"))
                        if (lop.BonusInfoDetails != null)
                            CopyValues(lo.BonusInfoDetails, lop.BonusInfoDetails);
                }

                if (obj.LineTariffplan.CheckIncludedTrafficInfoDetails != null && AllNull(lo.BonusInfoDetails))
                    lo.BonusInfoDetails = obj.LineTariffplan.CheckIncludedTrafficInfoDetails;

                if (obj.ShapingDetails == null)
                    obj.ShapingDetails = new ShapingDetails { PercVolTot = "-1", PeriodEndDate = null };

                var ni = new NumberInfo
                {
                    Credit = -1,
                    Number = obj.LineTariffplan.Msisdn,
                    Minutes = string.IsNullOrEmpty(lo.BonusInfoDetails.ResiduoMin) ? -1 : int.Parse(lo.BonusInfoDetails.ResiduoMin.Trim().Split(' ')[0]),
                    MinutesTotal = (string.IsNullOrEmpty(lo.BonusInfoDetails.ResiduoMin) || string.IsNullOrEmpty(lo.BonusInfoDetails.ConsumoMin)) ? -1 : int.Parse(lo.BonusInfoDetails.ResiduoMin.Trim().Split(' ')[0]) + int.Parse(lo.BonusInfoDetails.ConsumoMin.Trim().Split(' ')[0]),
                    Gigabytes = (int)(100 * float.Parse(obj.ShapingDetails.PercVolTot, new NumberFormatInfo { CurrencyDecimalSeparator = ".", NumberDecimalSeparator = "." })),
                    GigabytesTotal = 100,
                    SMS = string.IsNullOrEmpty(lo.BonusInfoDetails.ResiduoSms) ? -1 : int.Parse(lo.BonusInfoDetails.ResiduoSms),
                    SMSTotal = string.IsNullOrEmpty(lo.BonusInfoDetails.ResiduoSms) || string.IsNullOrEmpty(lo.BonusInfoDetails.ConsumoSms) ? -1 : int.Parse(lo.BonusInfoDetails.ResiduoSms.Trim()) + int.Parse(lo.BonusInfoDetails.ConsumoSms.Trim()),
                    LastUpdate = DateTime.Now,
                    ExpirationDate = DateTime.MinValue
                };

                float fTmp;
                DateTime dTmp;
                if (float.TryParse(obj.CreditBalanceValue, NumberStyles.Number, new NumberFormatInfo { CurrencyDecimalSeparator = ",", NumberDecimalSeparator = "," }, out fTmp))
                    ni.Credit = fTmp;

                if (DateTime.TryParseExact(obj.ShapingDetails.PeriodEndDate, "yyyyMMddhhmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dTmp) || DateTime.TryParseExact(lo.BonusInfoDetails.Expiry, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dTmp))
                    ni.ExpirationDate = dTmp;


                cr.NumberInfos.Add(ni);

            }

        }

        public override string Type
        {
            get { return "W"; }
        }

        public override object Clone()
        {
            return new WindRetr(_useProxy);
        }
    }
}