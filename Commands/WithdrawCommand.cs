using System;
using MySql.Data.MySqlClient;

namespace Ledger
{
    public class WithdrawCommand : Command
    {
       

        private void ExecuteCommand(int uid, float amt, DatabaseClient client)
        {
            MySqlCommand cmd = Command.BindStmt(
                   new string[] { "@id", "@amt" },
                   new object[] { uid, amt },
                   "call withdrawal(@id,@amt)"
               );
            client.Execute(cmd);
        }

     


        private void AttemptWithdrawal(float amt, LedgerState state, DatabaseClient client)
        {
            if (!client.Connect(state))
                return;
            float previousAmt = Command.GetAmount(state.CurrUser, client);
     
            if (Math.Abs(previousAmt - amt)<-Command.EPSILON)
                state.phase = "INSUFFICENT_FUNDS";
            else
            {

                ExecuteCommand(state.CurrUser, amt, client);
                float newAmt = Command.GetAmount(state.CurrUser, client);

                if (Command.CompareAmount(previousAmt,newAmt,amt))
                {
                    state.phase = "WITHDRAWAL_SUCCESS";
                    Console.WriteLine("Old balance: ${0:F2}\nNew balance: ${1:F2}", previousAmt, newAmt);
                }
                else
                    state.phase = "WITHDRAWAL_FAILED";

            }
            client.Close();
        }

        public bool Withdraw(int id, float amt, float previousAmt, DatabaseClient client)
        {
            ExecuteCommand(id,amt,client);
            //float newAmt = Command.GetAmount(id, client);
            return true;
        }

        public override LedgerState Invoke(string[] args, LedgerState previous, DatabaseClient client)
        {
            float amt = Command.ParseAmount(args.Length>1 ? args[1] : "",
                         "INVALID_WITHDRAW_ARGUMENT", previous);
            if (amt > 0)
                AttemptWithdrawal(amt,previous,client);



            return previous;
        }
    }
}
