using System;
namespace Ledger
{
    public class BalanceCommand : Command
    {
        public BalanceCommand()
        {
        }

        public override LedgerState Invoke(string[] args, LedgerState previous, DatabaseClient client)
        {
            if(previous.CurrUser == -1)
                previous.phase = "NOT_LOGGED_IN";
            else
            {
                if (!client.Connect(previous))
                    return previous;

                float balance = Command.GetAmount(previous.CurrUser, client);
                if (balance >= 0)
                {
                    Console.WriteLine("Your balance is ${0:F2}", balance);
                    previous.phase = "BALANCE_SUCCESS";
                }
                else
                    previous.phase = "BALANCE_FAIL";
                client.Close();

            }


            return previous;
        
        }
    }
}
