using System;
namespace Ledger
{
    public class LogoutCommand : Command
    {
        public LogoutCommand()
        {
        }

        public override LedgerState Invoke(string[] args, LedgerState previous, DatabaseClient client)
        {
            if (previous.CurrUser < 0)
            {
                previous.phase = "NOT_LOGGED_IN";
                return previous;
            }
            else
                previous.Logout();

            return previous;
        }
    }
}
