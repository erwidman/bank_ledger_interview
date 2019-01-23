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
    public class CreateAccountController : ApiController
    {

        public IHttpActionResult Post(HttpRequestMessage msg)
        {
            DatabaseClient webClient = new DatabaseClient();
            CreateAccountCommand command = new CreateAccountCommand();
            string auth = msg.Headers.Authorization.ToString();
            string[] splitAuth = WebUtil.DecodeAuth(auth);
            webClient.Connect();
            bool success = command.CreateAccount(splitAuth[0], splitAuth[1],webClient);
            webClient.Close();
            if (success)
                return Content(HttpStatusCode.OK,"ACCOUNT_CREATED");
            
            else
                return BadRequest();
        }
    }
}
