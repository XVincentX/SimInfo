using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using WindDataLib;

namespace WindAuth.Code
{
    public class TiscaliRetr : BaseCreditInfoRetr
    {
        public TiscaliRetr(bool UseProxy)
            : base(UseProxy)
        {

        }
        public async override Task<CreditInfo> Get(string username, string password, string type, Guid dev_id)
        {
            httpclient.BaseAddress = new Uri("https://webservices.tiscali.it/unit/ecare/");
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["entry"] = "login";
            query["mail"] = username;
            query["pwd"] = password;

            var res = await (await httpclient.GetAsync("it_smobile?" + query.ToString())).Content.ReadAsStreamAsync();

            var xDoc = XDocument.Load(res);

            dynamic logData = new ExpandoObject();
            XmlToDynamic.Parse(logData, xDoc.Elements().First());

            if (!(logData as IDictionary<string, object>).ContainsKey("authentication"))
                throw new WrongLoginDataException("Username o password errate");

            query = HttpUtility.ParseQueryString(string.Empty);

            var _idKey = logData.authentication.keys.id;
            var _idCnum = logData.authentication.keys.cnum;

            query["entry"] = "getAllOffers3";
            query["id"] = _idKey;
            query["cnum"] = _idCnum;
            query["name"] = username;
            query["vispi"] = "tiscali";

            res.Dispose();
            res = await (await httpclient.GetAsync("it_smobile?" + query.ToString())).Content.ReadAsStreamAsync();

            xDoc = XDocument.Load(res);

            logData = new ExpandoObject();
            XmlToDynamic.Parse(logData, xDoc.Elements().First());


            res.Dispose();

            var msisdn = ((logData.result.data.offer as IDictionary<string, object>).First().Value as IDictionary<string, object>)["attr_Numeroditelefono"].ToString();

            query = HttpUtility.ParseQueryString(string.Empty);

            query["entry"] = "getSIMOptions";
            query["id"] = _idKey;
            query["cnum"] = _idCnum;
            query["name"] = username;
            query["msisdn"] = msisdn;
            query["total"] = "1";

            res = await (await httpclient.GetAsync("it_smobile?" + query.ToString())).Content.ReadAsStreamAsync();
            xDoc = XDocument.Load(res);
            res.Dispose();
            XmlToDynamic.Parse(logData, xDoc.Elements().First());


            CreditInfo cr = new CreditInfo() { Username = username, Password = password, Type = type };
            cr.NumberInfos = new System.Collections.ObjectModel.ObservableCollection<NumberInfo>();

            var ni = new NumberInfo { Number = msisdn, Credit = float.Parse(logData.result.data.CreditAmount), ExpirationDate = DateTime.Parse(logData.result.data.BundleList.Bundle.PeriodEndDate), LastUpdate = DateTime.Now };


            foreach (dynamic bundle in logData.result.data.BundleList.Bundle.BundleElement)
            {
                switch ((string)bundle.TrafficType)
                {
                    case "SMS":
                        ni.SMS = int.Parse(bundle.AvailableAmount);
                        ni.SMSTotal = int.Parse(bundle.CatThreshold);
                        break;
                    case "VOCE":
                        ni.Minutes = int.Parse(bundle.AvailableAmount) / 60;
                        ni.MinutesTotal = int.Parse(bundle.CatThreshold) / 60;
                        break;
                    case "DATI":
                        ni.Gigabytes = (int)(100.0 / float.Parse(bundle.CatThreshold) * (float.Parse(bundle.AvailableAmount)));
                        ni.GigabytesTotal = 100;
                        break;
                    default:
                        break;
                }
            }
            cr.NumberInfos.Add(ni);

            return cr;
        }

        public override string Type
        {
            get { return "Z"; }
        }

        public override object Clone()
        {
            return new TiscaliRetr(false);
        }
    }
}
