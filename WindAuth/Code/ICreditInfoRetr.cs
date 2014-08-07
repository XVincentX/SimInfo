using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WindDataLib;

namespace WindAuth.Code
{
    public interface ICreditInfoRetr : ICloneable
    {
        Task<CreditInfo> Get(string username, string password, string type, Guid dev_id);

        string Type { get; }
    }

    public abstract class BaseCreditInfoRetr : ICreditInfoRetr, IDisposable
    {
        protected bool _useProxy;
        protected BaseCreditInfoRetr(bool UseProxy)
        {
            handler = new HttpClientHandler() { CookieContainer = new System.Net.CookieContainer(), UseCookies = true };
            if (UseProxy)
            {
                handler.UseProxy = true;
                handler.Proxy = new WebProxy();
            }
            _useProxy = UseProxy;
            httpclient = new HttpClient(handler, true);
            httpclient.DefaultRequestHeaders.ExpectContinue = false;
        }
        public abstract Task<CreditInfo> Get(string username, string password, string type, Guid dev_id);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {

            if (!_disposed)
            {
                if (disposing)
                {
                    if (httpclient != null)
                        httpclient.Dispose();
                    if (handler != null)
                        handler.Dispose();

                }

                httpclient = null;
                handler = null;
                _disposed = true;
            }

        }

        protected HttpClientHandler handler;
        protected HttpClient httpclient;
        private bool _disposed;



        public abstract string Type { get; }

        public abstract object Clone();
    }
}