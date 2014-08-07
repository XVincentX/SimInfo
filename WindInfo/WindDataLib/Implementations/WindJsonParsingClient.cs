using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Diagnostics;
using Newtonsoft.Json.Bson;

namespace WindDataLib.Implementations
{
    public class WindJsonParsingClient : CreditInfoRetriever
    {

        class MessageError
        {
            public string Message { get; set; }
        }

        public override async Task<CreditInfo> RetrieveCreditInfo(string username, string password, string Type, Guid dev_id)
        {

            var dsrz = new JsonSerializer();
            using (var httpclient = new HttpClient())
            {
                httpclient.DefaultRequestHeaders.ExpectContinue = false;
                httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/bson"));
                //using (var httpstrAuthJson = await httpclient.GetAsync(string.Format("https://wauth.apphb.com/api/AuthServ/Data/?q={0}&x={1}", username, password)))
                using (var httpstrAuthJson = await httpclient.PostAsync("https://wauth.apphb.com/api/AuthServ/GetData", new StringContent(JsonConvert.SerializeObject(new { q = username, x = password, t = Type, dev_id = dev_id }), Encoding.UTF8, "application/json")))
                {
                    using (var strData = await httpstrAuthJson.Content.ReadAsStreamAsync())
                    {

                        var bson = new BsonReader(strData);

                        if (httpstrAuthJson.StatusCode == HttpStatusCode.BadRequest)
                        {
                            var obj = dsrz.Deserialize<MessageError>(bson);
                            throw new Exception(obj.Message);
                        }

                        if (httpstrAuthJson.StatusCode == HttpStatusCode.NotFound)
                        {
                            var obj = new { Message = "Error during server connection." };
                            throw new Exception(obj.Message);
                        }

                        httpstrAuthJson.RequestMessage.Content.Dispose();
                        return dsrz.Deserialize<CreditInfo>(bson);
                    }


                }
            }
        }
    }
}
