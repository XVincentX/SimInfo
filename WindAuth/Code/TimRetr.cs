using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;
using WindAuth.Models.LoffTim;
using WindDataLib;


namespace WindAuth.Code
{
    public class TimRetr : BaseCreditInfoRetr
    {

        public TimRetr(bool UseProxy)
            : base(UseProxy)
        {

        }
        private CreditInfo cr = new CreditInfo() { NumberInfos = new System.Collections.ObjectModel.ObservableCollection<NumberInfo>() };
        public async override System.Threading.Tasks.Task<WindDataLib.CreditInfo> Get(string username, string password, string type, Guid dev_id)
        {
            httpclient.DefaultRequestHeaders.Clear();
            httpclient.DefaultRequestHeaders.Add("User-Widget", "iPhone");
            httpclient.DefaultRequestHeaders.Add("Widget-Version", "2.0");
            httpclient.DefaultRequestHeaders.Add("User-Name", username);
            httpclient.DefaultRequestHeaders.Add("Password", password);
            httpclient.DefaultRequestHeaders.Add("Client-Device-Id", username);
            httpclient.DefaultRequestHeaders.Add("Referer", "file:///Applications/Install/55D00839-D0A9-4CAB-B0F9-94DC75ECD835/Install/");
            var respone = await httpclient.GetAsync("https://camp.tim.it/widgetCamp/login.do");
            var res = await respone.Content.ReadAsStreamAsync();
            var strres = await respone.Content.ReadAsStringAsync();
            WindAuth.Models.LoffTim.Login.RESPONSE r;
            XmlSerializer xs = new XmlSerializer(typeof(WindAuth.Models.LoffTim.Login.RESPONSE));
            r = xs.Deserialize(res) as WindAuth.Models.LoffTim.Login.RESPONSE;

            if (r.RESULT.code != 200)
                throw new WrongLoginDataException("Username o password errati");

            var rdata = await httpclient.GetAsync("https://camp.tim.it/widgetCamp/info.do");

            var sinfo = await (rdata).Content.ReadAsStringAsync();
            var tr = new StringReader(sinfo.Replace('&', 'e'));

            XmlSerializer xst = new XmlSerializer(typeof(WindAuth.Models.LoffTim.Info.RESPONSE));
            var infoObj = xst.Deserialize(tr) as WindAuth.Models.LoffTim.Info.RESPONSE;

            cr.Username = username;
            cr.Password = password;
            cr.Type = type;

            var nm = new NumberInfo { ExpirationDate = DateTime.MaxValue, Number = username, Credit = infoObj.CONTENT.INFO.PROFILE.PREPAID != null ? (float)infoObj.CONTENT.INFO.PROFILE.PREPAID.CREDIT.value : (float)infoObj.CONTENT.INFO.PROFILE.CONTRACT.BALANCE.value };



            if (infoObj.CONTENT.INFO.PROMOTIONS.INTERFACELIST != null)
                foreach (var offer in infoObj.CONTENT.INFO.PROMOTIONS.INTERFACELIST.INTERFACE.Where(x => !x.url.Contains("OD994") && !x.url.Contains("DR600") && !x.url.Contains("TD095") && !x.url.Contains("ODB67") && !x.url.Contains("ODC70")))
                {

                    var data = await (await httpclient.GetAsync(offer.url)).Content.ReadAsStringAsync();
                    var srRead = new StringReader(data.Replace('&', 'e'));
                    var dsr = new XmlSerializer(typeof(WindAuth.Models.LoffTim.InterfaceData.RESPONSE));
                    var fdata = dsr.Deserialize(srRead) as WindAuth.Models.LoffTim.InterfaceData.RESPONSE;
                    if (fdata.CONTENT.PROMODETAIL == null || fdata.CONTENT.PROMODETAIL.DESCRIPTIONS.DESCRIPTION == null)
                        continue;

                    nm.ExpirationDate = new DateTime(Math.Min(nm.ExpirationDate.Ticks, DateTime.ParseExact(fdata.CONTENT.PROMODETAIL.EXPIRATION_DATE.value, "dd/MM/yyyy", CultureInfo.InvariantCulture).Ticks));

                    var promodescs = fdata.CONTENT.PROMODETAIL.PROMO_DESC.value.Split('-').SelectMany(x => x.Split('.')).Where(x => !x.Contains("esauri"));

                    foreach (var match in Regex.Matches(string.Join("-", promodescs), @"\d{1,}\b(\w*)\s(\w+)|\d{1,}(\w*)").Cast<Match>())
                    {
                        if (match.Value.ToUpper().Contains("SMS"))
                        {
                            nm.SMSTotal += int.Parse(Regex.Match(match.Value, @"\d+").Value);
                        }
                        else if (match.Value.ToUpper().Contains("MINUT"))
                        {
                            nm.MinutesTotal += int.Parse(Regex.Match(match.Value, @"\d+").Value);
                        }
                        else if (match.Value.ToUpper().Contains("MB"))
                        {
                            nm.GigabytesTotal = int.Parse(Regex.Match(match.Value, @"\d+").Value);
                        }
                        else if (match.Value.ToUpper().Contains("GB"))
                        {
                            nm.GigabytesTotal = int.Parse(Regex.Match(match.Value, @"\d+").Value) * 1000;
                        }
                    }


                    foreach (var promo in fdata.CONTENT.PROMODETAIL.DESCRIPTIONS.DESCRIPTION.Where(x => !x.value.Contains("effettuato") && !x.value.Contains("inviato")))
                    {
                        var numbers = Regex.Matches(promo.value, @"\d+");

                        if (promo.value.ToLower().Contains("sms"))
                        {
                            nm.SMS += int.Parse(numbers[0].Value);
                            continue;
                        }
                        if (promo.value.ToLower().Contains("minuti"))
                        {
                            nm.Minutes += int.Parse(numbers[0].Value);
                            continue;
                        }
                        if (promo.value.ToLower().Contains("mb"))
                        {
                            var number = Regex.Matches(promo.value, @"\d+.").Cast<Match>().First(x => x.Value.Contains("."));
                            if (nm.GigabytesTotal % 111111 == 0)
                                nm.GigabytesTotal = nm.Gigabytes = 100;
                            else
                                nm.Gigabytes = (int)((100.0f * float.Parse(number.Value)) / nm.GigabytesTotal);
                        }

                        if (nm.SMS % 111111 == 0)
                            nm.SMS = -1;

                        if (nm.Minutes % 111111 == 0)
                            nm.Minutes = -1;
                    }
                }

            if (nm.SMSTotal == 111111)
                nm.SMSTotal = nm.SMS;

            if (nm.MinutesTotal == 111111)
                nm.MinutesTotal = nm.Minutes;

            if (nm.GigabytesTotal == 111111)
                nm.GigabytesTotal = nm.Gigabytes;
            nm.LastUpdate = DateTime.Now;
            cr.NumberInfos.Add(nm);


            await httpclient.GetAsync("https://camp.tim.it/widgetCamp//logout.do"); //Logout at the end.
            return cr;
        }

        public override string Type
        {
            get { return "T"; }
        }

        public override object Clone()
        {
            return new TimRetr(_useProxy);
        }
    }
}