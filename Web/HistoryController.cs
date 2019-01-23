using System.Web.Http;
using System.Net.Http;
using System.Web.Http.Cors;

namespace Ledger.WebAPI
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HistoryController : ApiController
    {
        public IHttpActionResult Post(HttpRequestMessage msg)
        {
            DatabaseClient client = new DatabaseClient();
            HistoryCommand command = new HistoryCommand();
            client.Connect();
            string auth = msg.Headers.Authorization.ToString();
            bool validAuth = WebUtil.VerifyAuth(auth, client);
            int uid = Command.GetUID(WebUtil.DecodeAuth(auth)[0], client);
            if (!validAuth || uid < 0)
            {
                client.Close();
                return Unauthorized();
            }
            string json = command.GetHistory(uid, client);
            client.Close();
            if (json is null)
                return Content(System.Net.HttpStatusCode.InternalServerError, "HISTORY_FAILED");

            return Content(System.Net.HttpStatusCode.OK,json);

        }
    }
}
