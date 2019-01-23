using System;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Collections.Generic;
using System.Net.Http;
using Ledger.Security;
using System.Net;

namespace Ledger.WebAPI
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginController : ApiController
    {
       
        public HttpResponseMessage Post(HttpRequestMessage msg)
        {
            DatabaseClient webCleint = new DatabaseClient();
            webCleint.Connect();

            bool pass = WebUtil.VerifyAuth(msg.Headers.Authorization.ToString(),webCleint);
            Console.WriteLine(pass);
            HttpResponseMessage output;

            if (pass)
            {
                output = new HttpResponseMessage(HttpStatusCode.OK);
                output.Content = new StringContent("true");
            }
            else
                output = new HttpResponseMessage(HttpStatusCode.Unauthorized);

            webCleint.Close();
            return output;

        }
    }
}
