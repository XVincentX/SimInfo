using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using WindDataLib;
using System.Globalization;
namespace WindAuth.Code
{
    public class FastwebRetr : BaseCreditInfoRetr
    {
        public FastwebRetr(bool UseProxy)
            : base(UseProxy)
        {

        }
        public async override System.Threading.Tasks.Task<WindDataLib.CreditInfo> Get(string username, string password, string type, Guid dev_id)
        {

            CreditInfo cr = new CreditInfo() { NumberInfos = new System.Collections.ObjectModel.ObservableCollection<NumberInfo>(), Password = password, Username = username, Type = type };

            httpclient.BaseAddress = new Uri("https://rest.fastweb.it/myfp-restAPI/restAPI/");
            httpclient.DefaultRequestHeaders.TryAddWithoutValidation("deviceIMEI", (new Random()).Next(int.MinValue, int.MaxValue).ToString());
            var loginResponse = await (await httpclient.PostAsJsonAsync("identity/credential/pass", new { password = password, username = username, generateToken = false })).Content.ReadAsAsync<FastwebLoginResponse>();
            if (loginResponse.errorCode != 0)
                throw new WrongLoginDataException(loginResponse.errorMessage);

            var accountSims = await (await httpclient.GetAsync("account/service/usim?accountNumber=" + loginResponse.authenticationDetails.accountNumber)).Content.ReadAsAsync<UsimList>();

            if (accountSims.errorCode != 0)
                throw new WrongLoginDataException(loginResponse.errorMessage);

            foreach (var item in accountSims.ListOfMobileNumber)
            {
                var data = await (await httpclient.GetAsync("account/service/usim/usage?accountNumber=" + HttpUtility.UrlEncode(loginResponse.authenticationDetails.accountNumber) + "&msisdn=" + HttpUtility.UrlEncode(item.msisdn) + "&queryPeriod=30")).Content.ReadAsAsync<FCreditData>();
                if (data.errorCode != 0)
                    throw new WrongLoginDataException(data.errorMessage);

                cr.NumberInfos.Add(new NumberInfo
                {
                    Number = item.msisdn,
                    ExpirationDate = data.usageServiceInfo.SelectMany(x => x.listOfPlafond).Min(x => DateTime.ParseExact(x.endDateValidity, "dd/MM/yyyy", CultureInfo.GetCultureInfo("it-IT"))),
                    LastUpdate = DateTime.Now,
                    Credit = float.Parse(data.totalConsumption.balance),
                    SMS = data.usageServiceInfo.First(x => x.serviceType.Contains("SMS")).listOfPlafond.Sum(x => int.Parse(x.residual.Split(' ')[0])),
                    SMSTotal = data.usageServiceInfo.First(x => x.serviceType.Contains("SMS")).listOfPlafond.Sum(x => int.Parse(x.total.Split(' ')[0])),
                    Gigabytes = (int)(100.0f / data.usageServiceInfo.First(x => x.serviceType.Contains("DATA")).listOfPlafond.Sum(x => (int)float.Parse(x.total.Split(' ')[0])) * data.usageServiceInfo.First(x => x.serviceType.Contains("DATA")).listOfPlafond.Sum(x => (int)float.Parse(x.residual.Split(' ')[0]))),
                    Minutes = data.usageServiceInfo.First(x => x.serviceType.Contains("VOICE")).listOfPlafond.Where(w => w.description.Contains("Fuel") == false).Sum(x => int.Parse(x.residual.Split(' ')[0])),
                    MinutesTotal = data.usageServiceInfo.First(x => x.serviceType.Contains("VOICE")).listOfPlafond.Where(w => w.description.Contains("Fuel") == false).Sum(x => int.Parse(x.total.Split(' ')[0])),
                    GigabytesTotal = 100,

                });
            }

            return cr;
        }

        public override string Type
        {
            get { return "F"; }
        }

        public override object Clone()
        {
            return new FastwebRetr(_useProxy);
        }
    }


    public class AuthenticationDetails
    {
        public string username { get; set; }
        public string accountNumber { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public bool isPwdAuth { get; set; }
        public bool isIPAuth { get; set; }
        public bool isTokenAuth { get; set; }
        public bool isOTP { get; set; }
        public bool isOTPMail { get; set; }
        public bool isOTPCol { get; set; }
        public bool isPwdExpiring { get; set; }
        public bool isDisclamerAccepted { get; set; }
        public bool isWired { get; set; }
        public bool isMobile { get; set; }
    }

    public class RestServiceAvailability
    {
        public string serviceId { get; set; }
        public string serviceAvailability { get; set; }
    }

    public class FastwebLoginResponse
    {
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
        public bool isUserAuthenticated { get; set; }
        public AuthenticationDetails authenticationDetails { get; set; }
        public List<RestServiceAvailability> restServiceAvailability { get; set; }
    }

    public class ListOfMobileNumber
    {
        public string msisdn { get; set; }
        public string type { get; set; }
    }

    public class UsimList
    {
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
        public List<ListOfMobileNumber> ListOfMobileNumber { get; set; }
    }

    public class TotalConsumption
    {
        public string totalVoice { get; set; }
        public string totalData { get; set; }
        public string totalSMS { get; set; }
        public string lastModTime { get; set; }
        public string consumptionStartDate { get; set; }
        public string consumptionEndDate { get; set; }
        public string balance { get; set; }
    }

    public class ListOfPlafond
    {
        public string description { get; set; }
        public string label { get; set; }
        public string total { get; set; }
        public string residual { get; set; }
        public string used { get; set; }
        public string threshold { get; set; }
        public string startDateValidity { get; set; }
        public string endDateValidity { get; set; }
    }

    public class UsageInfo
    {
        public string lastModDate { get; set; }
        public bool alertEnabled { get; set; }
        public string alertThreshold { get; set; }
        public string alertChannel { get; set; }
    }

    public class ListOfPlafond2
    {
        public string description { get; set; }
        public string label { get; set; }
        public string total { get; set; }
        public string residual { get; set; }
        public string used { get; set; }
        public string threshold { get; set; }
        public string startDateValidity { get; set; }
        public string endDateValidity { get; set; }
    }

    public class UsageServiceInfo
    {
        public string serviceType { get; set; }
        public List<ListOfPlafond2> listOfPlafond { get; set; }
    }

    public class FCreditData
    {
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
        public TotalConsumption totalConsumption { get; set; }
        public List<ListOfPlafond> listOfPlafond { get; set; }
        public UsageInfo usageInfo { get; set; }
        public List<UsageServiceInfo> usageServiceInfo { get; set; }
    }
}