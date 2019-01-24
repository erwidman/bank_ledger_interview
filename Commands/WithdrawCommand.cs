/*
    Author      : Eric Richard Widmann
    Date        : 1/23/2019
    Description :
        Implementation of |withdraw <amount>| command.
*/
using System;
using MySql.Data.MySqlClient;

namespace Ledger
{
    public class WithdrawCommand : Command
    {
       

        /*
            Description:
                Executes withdraw stored procedure.
            Params:
                int uid -
                    Unique id of user.
                float amt -
                    Amount to withdraw.
                DatabaseClient client -
                    Client which will perform the query.
            Return:
                void

        
        */
        private void ExecuteWithdraw(int uid, float amt, DatabaseClient client)
        {
            MySqlCommand cmd = Command.BindStmt(
                   new string[] { "@id", "@amt" },
                   new object[] { uid, amt },
                   "call withdrawal(@id,@amt)"
               );
            client.Execute(cmd);
        }

     


        /*
            Description:
                Helper function for invoke to perform withdraw.
            Params:
                float amt -
                    Amount to deposit.
                LedgerState state -
                    LedgerState to modify.
                DatabaseClient client -
                    Client which will perform the query.
            Return:
                void
        */
        private void AttemptWithdrawal(float amt, LedgerState state, DatabaseClient client)
        {
            
            //user already logged in
            if (state.CurrUser < 0)
            {
                state.phase="ALREADY_LOGGED";
                return;
            }

            if (!client.Connect())
            {
                state.phase = "UNABLE_TO_CONNECT";
                return;
            }

            float previousAmt = Command.GetAmount(state.CurrUser, client);
     
            if (Math.Abs(previousAmt - amt)<-Command.EPSILON)
                state.phase = "INSUFFICENT_FUNDS";
            else
            {

                ExecuteWithdraw(state.CurrUser, amt, client);
                float newAmt = Command.GetAmount(state.CurrUser, client);
                if (Command.CompareAmount(newAmt,previousAmt,amt))
                {
                    state.phase = "WITHDRAWAL_SUCCESS";
                    Console.WriteLine("Old balance: ${0:F2}\nNew balance: ${1:F2}", previousAmt, newAmt);
                }
                else
                    state.phase = "WITHDRAWAL_FAILED";

            }
            client.Close();
        }
        /*
            Description:
                Function used by webapi to perform withdraw
            Params:
                int id -
                    Unique id of user.
                float amt -
                    Amount to withdraw.
                float previousAmt-
                    Amount previously in balance.
                DatabaseClient client -
                    Client which will perform the query.
        */
        public bool Withdraw(int id, float amt, float previousAmt, DatabaseClient client)
        {
            ExecuteWithdraw(id,amt,client);
            float newAmt = Command.GetAmount(id, client);
            return Command.CompareAmount(newAmt,previousAmt,amt);
        }

        //See Command.cs
        public override LedgerState Invoke(string[] args, LedgerState state, DatabaseClient client)
        {
            float amt = Command.ParseAmount(args.Length>1 ? args[1] : "",
                         "INVALID_WITHDRAW_ARGUMENT", state);
            if (amt > 0)
                AttemptWithdrawal(amt,state,client);



            return state;
        }
    }
}
