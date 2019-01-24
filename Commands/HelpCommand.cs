/*
    Author      : Eric Richard Widmann
    Date        : 1/23/2019
    Description :
        Implementation of |help| command.
*/
using System;
namespace Ledger
{
    public class HelpCommand : Command
    {
        //See Command.cs
        public override LedgerState Invoke(string[] args, LedgerState previous, DatabaseClient client)
        {
            previous.phase = "SHOW_HELP";
            return previous;
        }
    }
}
