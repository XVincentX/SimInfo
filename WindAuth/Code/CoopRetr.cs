using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using WindDataLib;

namespace WindAuth.Code
{
    public class CoopRetr : BaseCreditInfoRetr
    {
        public CoopRetr(bool UseProxy)
            : base(UseProxy)
        {

        }
        private CreditInfo cr = new CreditInfo() { NumberInfos = new System.Collections.ObjectModel.ObservableCollection<NumberInfo>() };
        public override async System.Threading.Tasks.Task<WindDataLib.CreditInfo> Get(string username, string password, string type, Guid dev_id)
        {
            var response = await httpclient.GetAsync(string.Format("https://www.coopvoce.it/jsp/jahia/templates/coop_voce_templates/iphone/json//login.jsp?username={0}&password={1}", username, password));
            var obj = JsonConvert.DeserializeObject<WindDataLib.Scoop.JsonTrafficSummary>(await response.Content.ReadAsStringAsync());

            if (obj.Common.Ack != "OK")
                throw new WrongLoginDataException("Username o password errati");

            cr.Username = username;
            cr.Password = password;
            cr.Type = type;
            foreach (var number in obj.MSISNDs)
            {
                var res = await httpclient.GetAsync(string.Format("https://www.coopvoce.it/p/selfCareAndroid;jsessionid={0}?service=Subscriber/Wallet&MSISDN={1}&APP_ID=CoopVoce&TOKEN=7d83b77c1441879bc3949ed443552252", obj.Common.Jsessionid, number.Msisdn));
                var objt = JsonConvert.DeserializeObject<WindDataLib.Scoop.WalletData>(await res.Content.ReadAsStringAsync());

                if (objt.Common.Ack != "OK")
                    continue;

                double credit = objt.Wallets.First(x => x.Tipo.ToLower().Contains("main")).Valore;

                var nm = new NumberInfo { Number = number.Msisdn.ToString(), Credit = (float)credit };

                res = await httpclient.GetAsync(string.Format("https://www.coopvoce.it//p/selfCareAndroid;jsessionid={0}?service=Promotions/GetPromos&MSISDN={1}&APP_ID=CoopVoce&TOKEN=7d83b77c1441879bc3949ed443552252", obj.Common.Jsessionid, number.Msisdn));
                var objp = JsonConvert.DeserializeObject<WindDataLib.Scoop.OfferData>(await res.Content.ReadAsStringAsync());

                var counters = objp.CurrentPromos.Where(x => x.Counters != null).SelectMany(x => x.Counters);
                foreach (var counter in counters)
                {
                    if (counter.CounterType == "Time") //minuti
                    {
                        nm.Minutes += counter.CounterResidual / 60;
                        nm.MinutesTotal = counter.CounterMaxValue / 60;
                    }
                    else if (counter.CounterType == "Packets")
                    {
                        nm.GigabytesTotal = 100;
                        nm.Gigabytes = (int)(100 * counter.CounterResidual / counter.CounterMaxValue);
                    }
                    else if (counter.CounterType == "Events")
                    {
                        nm.SMS += counter.CounterResidual;
                        nm.SMSTotal = counter.CounterMaxValue;
                    }

                }
                nm.LastUpdate = DateTime.Now;
                if (counters.Any(x => x.NextResetDate != null))
                    nm.ExpirationDate = counters.Where(x => x.NextResetDate != null).Min(x => DateTime.ParseExact(x.NextResetDate, new[] { "dd-MM-yyyy HH:mm", "dd-MM-yyyy" }, CultureInfo.GetCultureInfo("it-IT"), DateTimeStyles.None));
                else if (objp.CurrentPromos.Any(x => x.Counters != null))
                    nm.ExpirationDate = DateTime.ParseExact(objp.CurrentPromos.First(x => x.Counters != null).ExpirationDate, new[] { "dd-MM-yyyy HH:mm", "dd-MM-yyyy" }, CultureInfo.GetCultureInfo("it-IT"), DateTimeStyles.None);
                else
                    nm.ExpirationDate = DateTime.MaxValue;
                cr.NumberInfos.Add(nm);
            }

            return cr;
        }

        public override string Type
        {
            get { return "C"; }
        }

        public override object Clone()
        {
            return new CoopRetr(_useProxy);
        }
    }
}