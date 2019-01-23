using System;
using MySql.Data.MySqlClient;

namespace Ledger
{
    public class DepositCommand : Command
    {
        public DepositCommand()
        {
        }



        private void ExecuteCommand(int id, float amt, DatabaseClient client)
        {
            Console.WriteLine(amt);
            MySqlCommand cmd = Command.BindStmt(
              new string[] { "@id", "@amt" },
              new object[] { id, amt },
              "call deposit(@id,@amt)"
            );
            client.Execute(cmd);

        }

 


        private void AttemptDeposit(float amt,LedgerState state, DatabaseClient client)
        {

            if (!client.Connect(state))
                return;
            float previousAmt = Command.GetAmount(state.CurrUser, client);
            ExecuteCommand(state.CurrUser, amt, client);
            float newAmt = Command.GetAmount(state.CurrUser, client);
            if (Command.CompareAmount(previousAmt,newAmt,amt))
            {
                state.phase = "DEPOSIT_SUCCESS";
                Console.WriteLine("Old Balance : ${0:F2}\nNew Balance : ${1:F2}",previousAmt,newAmt);
            }
            else
                state.phase = "DEPOSIT_FAILED";

            client.Close();

        }

        public bool Deposit(int id, float amt, DatabaseClient client)
        {
            //float previousAmt = Command.GetAmount(id, client);
            ExecuteCommand(id, amt, client);
            //float newAmt = Command.GetAmount(id, client);
            return true;
         
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
