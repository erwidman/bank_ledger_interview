using System;
using MySql.Data.MySqlClient;

namespace Ledger
{
    public class HistoryCommand : Command
    {
        public HistoryCommand()
        {
        }

        private void PrintHistory(MySqlDataReader rdr)
        {
            string action,time = null;
            float delta = -1;
            Console.WriteLine("|Action           |Amount             |Time                        |");
            Console.WriteLine("+------------------------------------------------------------------+");
            while (rdr.Read())
            {
                action = rdr.GetString(0);
                time = rdr.GetString(2);
                delta = rdr.GetFloat(1);
                Console.WriteLine("|{0,-17}|${1,-18}|{2,-18}", action.ToUpper(), delta.ToString("F"), time);
            }
            rdr.Close();
        }


        public override LedgerState Invoke(string[] args, LedgerState previous, DatabaseClient client)
        {
            if (previous.CurrUser < 0)
                previous.phase = "NOT_LOGGED_IN";
            else
            {
                if (!client.Connect(previous))
                    return previous;
                MySqlCommand cmd = Command.BindStmt(
                     new string [] {"@id"},
                     new object [] {previous.CurrUser},
                     "select action, delta, time from History where id=@id and display=1 order by actionID desc limit 100"
                );
                float currAmt = Command.GetAmount(previous.CurrUser, client);
                MySqlDataReader rdr = client.SelectQuery(cmd);
                if (!(rdr is null))
                {
                    Console.WriteLine("|Current Balance : ${0:F2}", currAmt);
                    PrintHistory(rdr);
                    previous.phase = "HISTORY_SUCCESS";
                }
                else
                    previous.phase = "HISTORY_FAIL";
                client.Close();
            }


            return previous;
        }
    }
}
