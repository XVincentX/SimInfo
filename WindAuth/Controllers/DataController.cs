using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Permissions;
using System.Web.Http;

namespace WindAuth.Controllers
{
    [Authorize]
    public class DataController : ApiController
    {
        public string Get()
        {
            return "";
        }

        
        public string Post()
        {
            return "";
        }
    }
}