using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindDataLib.Implementations;

namespace WindDataLib
{
    public abstract class CreditInfoRetriever
    {
        public abstract Task<CreditInfo> RetrieveCreditInfo(string username, string password, string Type, Guid dev_id);

        public static CreditInfoRetriever Get()
        {
            if (_cr == null)
                //_cr = new WindHtmlParsingClient();
                _cr = new WindJsonParsingClient();
            return _cr;
        }

        private static CreditInfoRetriever _cr;
    }
}
