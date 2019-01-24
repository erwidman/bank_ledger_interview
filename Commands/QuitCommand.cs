/*
    Author      : Eric Richard Widmann
    Date        : 1/23/2019
    Description :
        Implementation of |quit| command.
*/
using System;
namespace Ledger
{
    public class QuitCommand : Command
    {
        //See commandCS
        public override LedgerState Invoke(string[] args, LedgerState previous, DatabaseClient client)
        {
            previous.Alive = false;
            return previous;
           
        }
    }
}
