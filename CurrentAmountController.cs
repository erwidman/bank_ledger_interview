using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Ledger.WebAPI
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CurrentAmountController : ApiController
    {
        public IHttpActionResult Post(HttpRequestMessage msg)
        {
            DatabaseClient client = new DatabaseClient();
            string auth = msg.Headers.Authorization.ToString();
            client.Connect();
            bool validAuth = WebUtil.VerifyAuth(auth, client);
            int uid = Command.GetUID(WebUtil.DecodeAuth(auth)[0], client);
            if (!validAuth || uid < 0)
            {
                client.Close();
                return Unauthorized();
            }
            float previousAmt = Command.GetAmount(uid, client);
            client.Close();
            if (previousAmt >= 0)
                return Content(HttpStatusCode.OK, previousAmt);
            else
                return InternalServerError();
        }
    }
}
