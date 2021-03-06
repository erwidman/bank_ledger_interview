﻿/*
    Author      : Eric Richard Widmann
    Date        : 1/23/2019
    Description :
        Handles post calls to /api/Alter.
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
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace Ledger.WebAPI
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AlterController : ApiController
    {

    
        
        private bool ExecuteDeposit(int uid, float amt, DatabaseClient client)
        {

            DepositCommand command = new DepositCommand();
            bool tmp =  command.Deposit(uid,amt,client);
            client.Close();
            return tmp;

        }

        private bool ExecuteWithdraw(int uid, float amt, float previousAmt, DatabaseClient client)
        {
            WithdrawCommand command = new WithdrawCommand();
            bool tmp = command.Withdraw(uid, amt, previousAmt, client);
            client.Close();
            return tmp;
        }



        public async Task<IHttpActionResult> PostAsync(HttpRequestMessage msg)
         {
            //read post body
            string json = await msg.Content.ReadAsStringAsync();
            Dictionary<string,string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            //ensure proper params
            if (!data.ContainsKey("amount") || !data.ContainsKey("action"))
                return Content(HttpStatusCode.BadRequest, "MISSING_PARAMS");

            string amt = data["amount"];
            string action = data["action"];
            float amount = Command.ParseAmount(amt);

            //verify amount is valid
            if (amount > Command.MAX_TRANSACTION)
                return Content(HttpStatusCode.BadRequest, "AMOUNT_GREATER_THAN_MAX_TRANSACTION");
            if (amount <= 0)
                return Content(HttpStatusCode.BadRequest, "INVALID_AMT");

            DatabaseClient client = new DatabaseClient();


            //attempt db connection
            if (!client.Connect())
                return Content(HttpStatusCode.InternalServerError, "DB_CONNECTION_FAILURE");

            //verify auth
            string auth = msg.Headers.Authorization.ToString();
            bool validAuth = WebUtil.VerifyAuth(auth, client);
            int uid = Command.GetUID(WebUtil.DecodeAuth(auth)[0],client);
            if (!validAuth || uid < 0)
            {
                client.Close();
                return Unauthorized();
            }
        

            float previousAmt = Command.GetAmount(uid, client);
            if (action.Equals("withdraw") && previousAmt - amount < -Command.EPSILON)
            {
                client.Close();
                return Content(HttpStatusCode.BadRequest, "INSUFFICENT_FUNDS");
            }

            if (action.Equals("deposit") && ExecuteDeposit(uid, amount, client))
                return Content(HttpStatusCode.OK, "DEPOSIT_COMPLETE");
            else if (action.Equals("deposit"))
                return Content(HttpStatusCode.InternalServerError, "DEPOSIT_FAILED");
            if (action.Equals("withdraw") && ExecuteWithdraw(uid, amount, previousAmt, client)) 
                return Content(HttpStatusCode.OK, "WITHDRAW_SUCCESS");
            else if (action.Equals("withdraw"))
                return Content(HttpStatusCode.InternalServerError, "WITHDRAW_FAILED");

            client.Close();
            return Content(HttpStatusCode.BadRequest,"INVALID_REQUEST");

            
         }
    }
}
