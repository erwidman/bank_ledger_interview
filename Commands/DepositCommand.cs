using System;
using MySql.Data.MySqlClient;

namespace Ledger
{
    public class DepositCommand : Command
    {
        public DepositCommand()
        {
        }


   

        private void AttemptDeposit(float amt,LedgerState state, DatabaseClient client)
        {

            if (!client.Connect(state))
                return;
            float previousAmt = Command.GetAmount(state.CurrUser, client);
            MySqlCommand cmd = Command.BindStmt(
                new string [] {"@id","@amt"},
                new object [] {state.CurrUser,amt},
                "call deposit(@id,@amt)"
            );
            bool success = client.Execute(cmd);
            float newAmt = Command.GetAmount(state.CurrUser, client);
            if (success && Math.Abs(newAmt - previousAmt - amt) < Command.EPSILON)
            {
                state.phase = "DEPOSIT_SUCCESS";
                Console.WriteLine("Old Balance : ${0:F2}\nNew Balance : ${1:F2}",previousAmt,newAmt);
            }
            else
                state.phase = "DEPOSIT_FAILED";

            client.Close();

        }



        public override LedgerState Invoke(string[] args, LedgerState previous, DatabaseClient client)
        {

            float amt = Command.ParseAmount(args.Length > 1 ? args[1] : "", 
                                "INVALID_DEPOSIT_ARGUMENT", previous);
            if (amt > 0)
                AttemptDeposit(amt,previous,client);
            


            return previous;
        }


    }
}
