/*
    Author      : Eric Richard Widmann
    Date        : 1/23/2019
    Description :
        Implementation of |create account <username>| command.
*/
using System;
namespace Ledger
{
    public class BalanceCommand : Command
    {
        //see Commands/Command.cs
        public override LedgerState Invoke(string[] args, LedgerState state, DatabaseClient client)
        {
            //if a user is not logged in, set state
            if(state.CurrUser == -1)
                state.phase = "NOT_LOGGED_IN";
            else
            {
                //attempt connection to DB
                if (!client.Connect(state))
                {
                    state.phase = "UNABLE_TO_CONNECT";
                    return state;
                }

                //get current amount
                float balance = Command.GetAmount(state.CurrUser, client);
                if (balance >= 0)
                {
                    Console.WriteLine("Your balance is ${0:F2}", balance);
                    state.phase = "BALANCE_SUCCESS";
                }
                else
                    state.phase = "BALANCE_FAIL";
                client.Close();

            }


            return state;
        
        }
    }
}
