/*
    Author      : Eric Richard Widmann
    Date        : 1/23/2019
    Description :
        Handles post calls to /api/Login.
*/
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
            //create a db client
            DatabaseClient webCleint = new DatabaseClient();
            HttpResponseMessage output;
            //if connection fails, sent internal error
            if (!webCleint.Connect())
            {
                output = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                output.Content = new StringContent("DB_CONNECTION_FAILURE");
                return output;
            }
            //check if auth is valid
            bool valid = WebUtil.VerifyAuth(msg.Headers.Authorization.ToString(),webCleint);
            if (valid)
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
