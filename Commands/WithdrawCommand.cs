using System;
using MySql.Data.MySqlClient;

namespace Ledger
{
    public class WithdrawCommand : Command
    {
        public WithdrawCommand()
        {
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
                MySqlCommand cmd = Command.BindStmt(
                    new string[] { "@id", "@amt" },
                    new object[] { state.CurrUser, amt },
                    "call withdrawal(@id,@amt)"
                );
                bool success = client.Execute(cmd);
                float newAmt = Command.GetAmount(state.CurrUser, client);
                Console.WriteLine("{0} here",Math.Abs(previousAmt - amt - newAmt));
                if (success && Math.Abs(previousAmt - amt - newAmt) < Command.EPSILON)
                {
                    state.phase = "WITHDRAWAL_SUCCESS";
                    Console.WriteLine("Old balance: ${0:F2}\nNew balance: ${1:F2}", previousAmt, newAmt);
                }
                else
                    state.phase = "WITHDRAWAL_FAILED";

            }
            client.Close();

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
