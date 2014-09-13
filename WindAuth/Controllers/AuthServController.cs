using Microsoft.ApplicationInsights;
using Microsoft.ServiceBus.Notifications;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using WindAuth;
using WindAuth.Code;
using WindAuth.Controllers;
using WindAuth.Data;
using WindAuth.Models;
using WindDataLib;
using WindDataLib.Model;

public class AuthServController : ApiController
{
    public CreditInfo cr = new CreditInfo();
    static int RequestCount = 0;
    private IEnumerable<ICreditInfoRetr> _retrievers;
    private List<Task<CreditInfo>> _tsk;
    private List<Exception> _exc;
    private DataContext _context;
    private NotificationHubClient _notificationClient;
    private MobileServiceClient _mobileClient;

    public AuthServController(IEnumerable<ICreditInfoRetr> retrievers, DataContext context, NotificationHubClient notificationClient, MobileServiceClient mobileCLient)
    {
        _retrievers = retrievers;
        _tsk = new List<Task<CreditInfo>>(retrievers.Count());
        _exc = new List<Exception>(retrievers.Count());
        _context = context;
        _notificationClient = notificationClient;
        _mobileClient = mobileCLient;
    }

    [AllowAnonymous, HttpGet]
    public async Task<IHttpActionResult> Get([FromUri]string q, [FromUri]string x)
    {
        RequestCount++;

        try
        {


            List<Task> tasks = new List<Task>();

            var cookieJar = new CookieContainer();
            var logresposne = new JsonLogin();
            using (var handler = new HttpClientHandler { AllowAutoRedirect = true, UseCookies = true, CookieContainer = cookieJar })
            {
                using (var httpclient = new HttpClient(handler))
                {
                    ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => { return true; }; //Validate any certificate. Who cares about security.
                    using (var content = new StringContent(string.Format("handset-os=Android+4.3&handset-model=HTC+HTC+One&username={0}&password={1}", q, x)))
                    {

                        httpclient.DefaultRequestHeaders.Add("Accept", "application/json");
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                        httpclient.DefaultRequestHeaders.ExpectContinue = false;
                        httpclient.BaseAddress = new Uri("https://authserv.infostrada.it", UriKind.Absolute);
                        using (var httpstrAuthJson = await httpclient.PostAsync("/155/auth/new/LoginUidPwd", content))
                        {
                            var strAuthJson = await httpstrAuthJson.Content.ReadAsStringAsync();

                            try
                            {
                                logresposne = JsonConvert.DeserializeObject<JsonLogin>(strAuthJson);
                                if (logresposne == null || logresposne.Response.Status != "0")
                                    return BadRequest(logresposne.Response.Reason);
                            }
                            catch (Exception)
                            {
                                return BadRequest("Il servizio Wind non è attualmente disponibile.");
                            }
                        }

                    }

                    cr.Username = q;
                    cr.Password = x;


                    Expression<Func<LoggedUser, bool>> whereExp = tx => tx.Username == q;

                    try
                    {
                        var res = _context.LoggedUsers.Where(whereExp);
                        if (res.Count() == 0)
                            _context.LoggedUsers.Add(new LoggedUser { Username = q, Password = x });
                        else
                            _context.LoggedUsers.First(whereExp).Password = x;

                        _context.SaveChanges();
                    }
                    catch (Exception)
                    {
                        //Se non salva pazienza, su somee non c'è base di dati.
                    }


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
                        foreach (var url in (new[] { "/155/line/LineSummary", "/155/line/BonusInfo", "/155/line/CreditBalance", "/155/text/TextCatalog", "/155/recharge/DebitsCredits", "/155/traffic/TrafficDetails", "/155/traffic/TrafficSummary", "/155/customer/CustomerDetails" }).Take(1))
                            tasks.Add(httpclient.PostAsync(new Uri(url, UriKind.Relative), content).ContinueWith(continueAction));
                    }

                    Task.WaitAll(tasks.ToArray());


                }



                return Ok(cr);
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [AllowAnonymous, Route("api/AuthServ/GetData"), HttpPost, HttpGet]
    public async Task<IHttpActionResult> GetData([FromBody]Q_X d1, [FromUri]Q_X d2)
    {
        using (var telemetery = new TelemetryContext())
        {

            Q_X data = d1 ?? d2;

            IHttpActionResult httpActionResult;
            AuthServController.RequestCount = AuthServController.RequestCount + 1;

            try
            {

                Expression<Func<LoggedUser, bool>> whereExp = tx => tx.Username == data.q;

                try
                {
                    var res = _context.LoggedUsers.Where(whereExp);

                    var user = await _context.LoggedUsers.FirstOrDefaultAsync(whereExp);
                    if (user == null)
                        _context.LoggedUsers.Add(new LoggedUser { Username = data.q, Password = data.x, device_id = data.dev_id, Type = data.t, LastLogin = DateTime.Now });
                    else
                    {
                        user.Password = data.x;
                        user.device_id = data.dev_id;
                        user.Type = data.t;
                        user.LastLogin = DateTime.Now;
                    }

                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    //Se non salva pazienza.
                }

                if (string.IsNullOrEmpty(data.t))
                    data.t = "W"; //Old customers.

                var _retriever = _retrievers.Where(x => x.Type == data.t);
                if (_retriever.Count() == 0)
                    return BadRequest("No retriever found for you");


                this._tsk.Add(_retriever.First().Get(data.q, data.x, data.t, data.dev_id));
                foreach (Task<CreditInfo> task in this._tsk)
                {
                    try
                    {
                        this.cr = await task;
                        await usrTask;
                        httpActionResult = this.Ok<CreditInfo>(this.cr);
                        return httpActionResult;
                    }
                    catch (Exception exception)
                    {
                        this._exc.Add(exception);
                    }
                }
                httpActionResult = this.BadRequest(this._exc.First<Exception>().Message);
                telemetery.TrackEvent(data.t);
                return httpActionResult;
            }
            catch (Exception exception1)
            {
                telemetery.TrackException(exception1);
                return this.BadRequest(exception1.Message);
            }
        }
    }

    [AllowAnonymous, HttpGet, Route("api/AuthServ/Timestamp")]
    public DateTime Timestamp()
    {
        return DateTime.Now;
    }


    [HttpPost, AllowAnonymous, Route("api/AddPayment")]
    public async Task<IHttpActionResult> AddPayment(PayingUser user)
    {

        _context.PayingUsers.AddOrUpdate(user);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [Route("api/WhoPayed/{username?}"), AllowAnonymous, HttpGet]
    public async Task<IHttpActionResult> WhoPayed(string username = null)
    {
        var data = _context.PayingUsers.Select(x => x.Username);
        if (!string.IsNullOrEmpty(username))
            data = data.Where(x => x == username);

        return Ok(await data.ToListAsync());
    }

    [Route("api/HowManyPayed/{username}"), AllowAnonymous, HttpGet]
    public async Task<IHttpActionResult> HowManyPayed(string username = null)
    {
        var data = _context.PayingUsers
            .Where(x => x.Username == username)
            .Select(x => x.Count);

        return Ok(await data.ToArrayAsync());
    }

    [HttpPost, Route("api/PushUri")]
    public async Task<IHttpActionResult> PushUri([FromUri]string Uri, [FromBody]string value)
    {


        try
        {
            _context.NotificationUris.AddOrUpdate(new NotificationUri { ChannelUri = Uri, stringNumbers = value });
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception)
        {
            return InternalServerError();
        }

    }

    static readonly IList<CreditInfo> alreadyRetrieved = new List<CreditInfo>();
    private Task<Task<Task<int>>> usrTask;
    [Route("api/TriggerCheck"), AllowAnonymous, HttpGet]
    public async Task<IHttpActionResult> TriggerCheck([FromUri]int start = 0)
    {
        var sw = Stopwatch.StartNew();
        int registrationCount = 0;
        const string toastTemplate = "<?xml version=\"1.0\" encoding=\"utf-8\"?><wp:Notification xmlns:wp=\"WPNotification\"><wp:Toast><wp:Text1>{0}</wp:Text1><wp:Text2>{1}</wp:Text2><wp:Param>{2}</wp:Param></wp:Toast></wp:Notification>";


        IList<NotificationOutcome> result = new List<NotificationOutcome>();
        IList<MpnsNotification> notifications = new List<MpnsNotification>();

        var registrations = (await _notificationClient.GetAllRegistrationsAsync(Int32.MaxValue)).Where(x => x.Tags != null).Skip(start);
        var preferences = _mobileClient.GetTable<PlainNumberInfo>();

        if (start == 0)
            alreadyRetrieved.Clear();

        foreach (var user in registrations)
        {
            notifications.Clear();
            foreach (var number in user.Tags)
            {
                try
                {

                    bool dirty = false;
                    var data = number.Split('_');
                    if (data.Any(x => string.IsNullOrEmpty(x)))
                    {
                        await _notificationClient.DeleteRegistrationAsync(user); //Tanto non saprei cosa cazzo farci.
                        continue;
                    }

                    CreditInfo content = null;

                    if (!alreadyRetrieved.Any(x => x.Username == data[0] && x.Type == data[data.Length - 1] && x.Password == string.Join("_", data.Skip(1).Take(data.Length - 2))))
                    {
                        content = await _retrievers.First(x => x.Type == data[data.Length - 1]).Get(data[0], string.Join("_", data.Skip(1).Take(data.Length - 2)), data[data.Length - 1], Guid.Empty); //this.GetData(null, new Q_X { q = data[0], x = data[1], t = data[2] });
                        alreadyRetrieved.Add(content);
                    }
                    else
                    {
                        content = alreadyRetrieved.First(x => x.Username == data[0] && x.Type == data[2] && x.Password == data[1]);
                        dirty = true;
                    }

                    foreach (var accountNumber in content.NumberInfos)
                    {
                        IEnumerable<PlainNumberInfo> userPreference = await preferences.Where(x => x.Number == accountNumber.Number).ToEnumerableAsync();
                        if (userPreference.Count() == 0)
                            await preferences.InsertAsync(new PlainNumberInfo { Number = accountNumber.Number, FriendlyName = accountNumber.Number, Gigabytes = accountNumber.Gigabytes, Minutes = accountNumber.Minutes, SMS = accountNumber.SMS, Credit = accountNumber.Credit, Brush = "#000000" });
                        else
                        {
                            var dataPreference = userPreference.First();
                            accountNumber.Brush = dataPreference.Brush;
                            if (string.IsNullOrEmpty(dataPreference.Brush))
                                accountNumber.Brush = "#000000"; //Nero

                            accountNumber.FriendlyName = dataPreference.FriendlyName;

                            if (dataPreference.NotifyEnabled)
                            {
                                if (accountNumber.SMS < dataPreference.SMSLimit && dataPreference.smsShowed == false)
                                {
                                    dirty = true;
                                    dataPreference.smsShowed = true;
                                    notifications.Add(new MpnsNotification(string.Format(toastTemplate, "Warning", string.Format("{0} : raggiunto limite SMS", dataPreference.FriendlyName), string.Format("/DataPage.xaml?number={0}", accountNumber.Number))));
                                }
                                else if (accountNumber.SMS >= dataPreference.SMSLimit)
                                    dataPreference.smsShowed = false;
                                if (accountNumber.Minutes < dataPreference.MinutesLimit && dataPreference.minShowed == false)
                                {
                                    dirty = true;
                                    dataPreference.minShowed = true;
                                    notifications.Add(new MpnsNotification(string.Format(toastTemplate, "Warning", string.Format("{0} : raggiunto limite minuti", dataPreference.FriendlyName), string.Format("/DataPage.xaml?number={0}", accountNumber.Number))));
                                }
                                else if (accountNumber.Minutes >= dataPreference.MinutesLimit)
                                    dataPreference.minShowed = false;

                                if (accountNumber.Gigabytes < dataPreference.GigabytesLimit && dataPreference.gigaShowed == false)
                                {
                                    dirty = true;
                                    dataPreference.gigaShowed = true;
                                    notifications.Add(new MpnsNotification(string.Format(toastTemplate, "Warning", string.Format("{0} : raggiunto limite traffico", dataPreference.FriendlyName), string.Format("/DataPage.xaml?number={0}", accountNumber.Number))));
                                }
                                else if (accountNumber.Gigabytes >= dataPreference.GigabytesLimit)
                                    dataPreference.gigaShowed = false;

                                if (accountNumber.Credit < dataPreference.CreditLimit && dataPreference.clShowed == false)
                                {
                                    dirty = true;
                                    dataPreference.clShowed = true;
                                    notifications.Add(new MpnsNotification(string.Format(toastTemplate, "Warning", string.Format("{0} : raggiunto limite credito", dataPreference.FriendlyName), string.Format("/DataPage.xaml?number={0}", accountNumber.Number))));
                                }
                                else if (accountNumber.Credit >= dataPreference.CreditLimit)
                                    dataPreference.clShowed = false;



                                accountNumber.CreditLimit = dataPreference.CreditLimit;
                                accountNumber.SMSLimit = dataPreference.SMSLimit;
                                accountNumber.GigabytesLimit = dataPreference.GigabytesLimit;
                                accountNumber.MinutesLimit = dataPreference.MinutesLimit;
                            }

                            if (dataPreference.Credit != accountNumber.Credit
                                || dataPreference.SMS != accountNumber.SMS
                                || dataPreference.Gigabytes != accountNumber.Gigabytes
                                || dataPreference.Minutes != accountNumber.Minutes)
                            {
                                dataPreference.Credit = accountNumber.Credit;
                                dataPreference.SMS = accountNumber.SMS;
                                dataPreference.Gigabytes = accountNumber.Gigabytes;
                                dataPreference.Minutes = accountNumber.Minutes;

                                dirty = true;
                            }

                            if (dirty)
                            {
                                dataPreference.LastUpdate = DateTime.Now;
                                await preferences.UpdateAsync(dataPreference);
                            }


                        }


                        if (dirty)
                        {
                            var thread = new Thread(new ThreadStart(() => { XamlRendering.ConvertToJpg(accountNumber); }));

                            thread.SetApartmentState(ApartmentState.STA);
                            thread.Start();
                            thread.Join();

                            var notificationPayload =
                                string.Format(PushTemplates.NotificationTemplate,
                                    string.Format("/DataPage.xaml?number={0}", accountNumber.Number),
                                    string.Format("http://wauth.apphb.com/Tiles/{0}_159.jpg", accountNumber.Number),
                                    string.Format("http://wauth.apphb.com/Tiles/{0}_336.jpg", accountNumber.Number),
                                    string.Format("http://wauth.apphb.com/Tiles/{0}_691.jpg", accountNumber.Number));

                            var notification = new MpnsNotification(notificationPayload, new Dictionary<string, string> { { "X-WindowsPhone-Target", "token" }, { "X-NotificationClass", "1" } });
                            notifications.Add(notification);
                        }
                    }
                }
                catch (WrongLoginDataException)
                {
                    notifications.Add(new MpnsNotification(string.Format(toastTemplate, "Warning", "Dati di login errati.", string.Empty)));
                    _notificationClient.DeleteRegistrationAsync(user).Wait(); //Addio. Rientra nell'app e inserisci i dati giusti.
                }
                catch (Exception)
                {
                    continue;
                }

                var tasks = notifications.Select(x => _notificationClient.SendNotificationAsync(x, user.Tags));

                foreach (var item in tasks)
                    result.Add(await item);
            }

            registrationCount++;
            if (100 - sw.Elapsed.TotalSeconds <= 5)
                break;
        }

        return Ok(new NotificationProcessInfo { Registrations = registrationCount, Notifications = result.Count });
    }

    [HttpGet, AllowAnonymous, Route("api/ServiceBusRegistrations")]
    public async Task<IHttpActionResult> ServiceBusData()
    {
        return Ok(await _notificationClient.GetAllRegistrationsAsync(Int32.MaxValue));
    }

    [HttpGet, AllowAnonymous, Route("api/ServiceBusClear")]
    public async Task<IHttpActionResult> ServiceBusClear()
    {
        foreach (var item in await _notificationClient.GetAllRegistrationsAsync(Int32.MaxValue))
        {
            await _notificationClient.DeleteRegistrationAsync(item.RegistrationId);
        }

        return Ok();
    }

    #region Pattume

    [NonAction]
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

    [NonAction]
    private bool AllNull<T>(T Target)
    {
        foreach (var item in typeof(T).GetProperties().Where(prop => prop.CanRead && prop.CanWrite))
        {
            if (item.GetValue(Target) != null)
                return false;
        }
        return true;

    }

    [NonAction]
    private void DeserializeInOutput(string str, Uri uri)
    {
#if DEBUG
        Debug.WriteLine(str);
#endif
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
    #endregion
}

