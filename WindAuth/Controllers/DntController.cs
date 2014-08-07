using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WindAuth.Models;

namespace WindAuth.Controllers
{
    public class DntController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Redirect("https://www.paypal.com/it/cgi-bin/webscr?cmd=_send-money&nav=1&email=vincenz.chianese@yahoo.it");
        }

    }
}
