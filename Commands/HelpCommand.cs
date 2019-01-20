using System;
namespace Ledger
{
    public class HelpCommand : Command
    {
        public HelpCommand()
        {
        }

        public override LedgerState Invoke(string[] args, LedgerState previous, DatabaseClient client)
        {
            previous.phase = "SHOW_HELP";
            return previous;
        }
    }
}
