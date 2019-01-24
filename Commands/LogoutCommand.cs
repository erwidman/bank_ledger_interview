/*
    Author      : Eric Richard Widmann
    Date        : 1/23/2019
    Description :
        Implementation of |logout| command.
*/
using System;
namespace Ledger
{
    public class LogoutCommand : Command
    {
      
        //See Command.cs
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
