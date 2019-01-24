/*
    Author      : Eric Richard Widmann
    Date        : 1/23/2019
    Description :
        Implementation of |deposit <amount>| command.
*/
using System;
using MySql.Data.MySqlClient;

namespace Ledger
{
    public class DepositCommand : Command
    {
    
        /*
            Description:
                Executes deposit stored procedure.
            Params:
                int id -
                    Userid uniquely identifying a user.
                float amt -
                    Amount to deposit.
                DatabaseClient client -
                    Client which will perform the query.
            Return:
                void
        */
        private void ExecuteDeposit(int id, float amt, DatabaseClient client)
        {
            MySqlCommand cmd = Command.BindStmt(
              new string[] { "@id", "@amt" },
              new object[] { id, amt },
              "call deposit(@id,@amt)"
            );
            client.Execute(cmd);

        }

 

        /*
            Description:
                Helper function used by invoke to perform deposit.
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
        private void AttemptDeposit(float amt,LedgerState state, DatabaseClient client)
        {

            if (!client.Connect(state))
            {
                state.phase = "UNABLE_TO_CONNECT";
                return;
            }
            float previousAmt = Command.GetAmount(state.CurrUser, client);
            ExecuteDeposit(state.CurrUser, amt, client);
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

        /*
            Description:
                Function used by webapi to deposit.
            Params:
                int id - 
                    Unique identifier of user
                float amt -
                    Amount to deposit
                DatabaseClient client -
                    Client which will perform the query.
            Returns:
                True if successful.

        */
        public bool Deposit(int id, float amt, DatabaseClient client)
        {
            float previousAmt = Command.GetAmount(id, client);
            ExecuteDeposit(id, amt, client);
            float newAmt = Command.GetAmount(id, client);
            return Command.CompareAmount(previousAmt,newAmt,amt);
        }



        //See command.cs
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
