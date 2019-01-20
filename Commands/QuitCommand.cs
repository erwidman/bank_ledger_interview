using System;
namespace Ledger
{
    public class QuitCommand : Command
    {
        public QuitCommand()
        {
        }

        public override LedgerState Invoke(string[] args, LedgerState previous, DatabaseClient client)
        {
            client.Close();
            previous.Alive = false;
            return previous;
           
        }
    }
}
