using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using WindDataLib;
using System.Diagnostics;

namespace WindAuth.Code
{

    public class md : MediaTypeHeaderValue
    {
        public md(MediaTypeHeaderValue source)
            : base(source)
        {

        }
        public md(string mediatype) : base(mediatype) { }
        public override string ToString()
        {
            return base.ToString().Replace(" ", string.Empty);
        }
    }
    public class H3GRetr : BaseCreditInfoRetr
    {
        public H3GRetr(bool UseProxy)
            : base(UseProxy)
        {

        }
        public override async System.Threading.Tasks.Task<WindDataLib.CreditInfo> Get(string username, string password, string type, Guid dev_id)
        {
            httpclient.DefaultRequestHeaders.Clear();
            httpclient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", shitAuthorization);
            httpclient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "text/xml;charset=UTF-8");
            httpclient.DefaultRequestHeaders.TryAddWithoutValidation("x-h3g-msisdn", "39" + username);
            httpclient.DefaultRequestHeaders.TryAddWithoutValidation("Date", "Thu, 08 May 2014 20:22:42 GMT");
            httpclient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "NativeHost");
            httpclient.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "file://,/Applications/Install/443D0ECE-0B8F-4D98-AD73-4304A7CAC910/Install/");
            httpclient.DefaultRequestHeaders.TryAddWithoutValidation("jsessionid", "1");
            httpclient.DefaultRequestHeaders.TryAddWithoutValidation("resource", "/serviceExposer/soap/selfcare/credential");
            httpclient.DefaultRequestHeaders.TryAddWithoutValidation("siteid", "11");
            httpclient.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "no-cache");
            httpclient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
            httpclient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "identity");
            var cnt = new StringContent(string.Format(H3GStrings.Login, "39" + username, password), Encoding.UTF8, "text/xml");

            cnt.Headers.Clear();
            cnt.Headers.TryAddWithoutValidation("Content-Type", "text/xml;charset=UTF-8");
            cnt.Headers.ContentType = new md(cnt.Headers.ContentType);

            var response = await httpclient.PostAsync(string.Concat(baseUrl, "api/services/credentialService"), cnt).ConfigureAwait(false);

            cnt.Dispose();

            cnt = new StringContent(string.Format(H3GStrings.InfoRapidService, "39" + username), Encoding.UTF8, "text/xml");

            cnt.Headers.Clear();
            cnt.Headers.TryAddWithoutValidation("Content-Type", "text/xml;charset=UTF-8");
            cnt.Headers.ContentType = new md(cnt.Headers.ContentType);

            response = await httpclient.PostAsync(string.Concat(baseUrl, "api/services/infoRapideService"), cnt);
            var xml = await response.Content.ReadAsStringAsync();
            var xmlDoc = XDocument.Parse(xml);

            float credit = 0;
            if (xmlDoc.Descendants().Any(x => x.Name.LocalName == "totalCreditAmount"))
                credit = float.Parse(xmlDoc.Descendants().First(x => x.Name.LocalName == "totalCreditAmount").Value, CultureInfo.GetCultureInfo("en-US"));

            var number = new NumberInfo { Number = username, LastUpdate = DateTime.Now, Credit = credit, SMS = -1, Gigabytes = -1, Minutes = -1, ExpirationDate = DateTime.MaxValue };
            var UnitsNodes = xmlDoc.XPathSelectElements("//data[key='freetUnits']");
            foreach (var node in UnitsNodes.Descendants().Where(a => a.Attributes().Any(x => x.Name.LocalName == "type" && x.Value == "ns1:FreeUnitItems")))
            {
                foreach (var el in node.Elements())
                {

                    var dataElem = el.Elements().First(x => x.Name.LocalName != "key");
                    var initial = float.Parse(dataElem.Elements().First(x => x.Name.LocalName == "initialAmount").Value, CultureInfo.GetCultureInfo("en-US"));
                    var remaining = float.Parse(dataElem.Elements().First(x => x.Name.LocalName == "remainingAmount").Value, CultureInfo.GetCultureInfo("en-US"));
                    DateTime expiration = DateTime.Parse(dataElem.Elements().First(x => x.Name.LocalName == "endDate").Value, CultureInfo.GetCultureInfo("en-US"));
                    number.ExpirationDate = (expiration > number.ExpirationDate ? number.ExpirationDate : expiration);

#if DEBUG
                    Debug.WriteLine("{0}-{1}-{2}", el.Elements().First(x => x.Name.LocalName == "key").Value, remaining, initial);
#endif

                    switch (el.Elements().First(x => x.Name.LocalName == "key").Value)
                    {
                        case "384":
                        case "501":
                        case "498":
                        case "490":
                        case "531":
                        case "535":
                        case "493":
                        case "504":
                        case "364":
                            number.Gigabytes += (int)remaining;
                            number.GigabytesTotal += initial;
                            break;

                        case "194":
                        case "500":
                        case "497":
                        case "489":
                        case "492":
                        case "503":
                        case "363":
                        case "506":
                            number.SMS += (int)remaining;
                            number.SMSTotal += (int)initial;
                            break;
                        case "193":
                        case "499":
                        case "488":
                        case "491":
                        case "502":
                        case "362":
                        case "496":
                        case "505":
                            number.Minutes += (int)remaining / 60;
                            number.MinutesTotal += (int)initial / 60;
                            break;

                    }
                }

            }

            if (number.GigabytesTotal == 0)
                number.GigabytesTotal = 1;
            number.Gigabytes = 100 * number.Gigabytes / (int)number.GigabytesTotal;
            number.GigabytesTotal = 100;
            number.Credit = (float)Math.Round((decimal)number.Credit, 2, MidpointRounding.AwayFromZero);

            var cr = new CreditInfo { Username = username, Password = password, Type = type, NumberInfos = new System.Collections.ObjectModel.ObservableCollection<NumberInfo>() };

            cr.NumberInfos.Add(number);
            return cr;
        }

        private string baseUrl = "https://areaclienti3.tre.it/";
        private string shitAuthorization = "WDTC64Y08CT453AA453J:wS0Cu8+gdCe966SnZZCIZ4lUwsXWM1MJbJ9VG0BsmQA=";
        public override object Clone()
        {
            return new H3GRetr(_useProxy);
        }

        public override string Type
        {
            get { return "H"; }
        }
    }

}