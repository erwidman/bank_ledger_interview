using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
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

        private MySqlDataReader ExecuteQuery(int uid,DatabaseClient client)
        {
            MySqlCommand cmd = Command.BindStmt(
                    new string[] { "@id" },
                    new object[] { uid },
                    "select action, delta, time from History where id=@id and display=1 order by actionID desc limit 100"
               );
            return client.SelectQuery(cmd);
        }



        private class History
        {
            public string action;
            public float delta;
            public string time;
            public History(string action, float delta, string time)
            {
                this.action = action;
                this.delta = delta;
                this.time = time;
            }
        }
        public string GetHistory(int uid, DatabaseClient client)
        {
          
            MySqlDataReader rdr = ExecuteQuery(uid, client);
            if (rdr is null)
                return null;
            else
            {
                List<History> buffer = new List<History>();
                while (rdr.Read())
                {
                    string action = rdr.GetString(0);
                    string time = rdr.GetString(2);
                    float delta = rdr.GetFloat(1);
                    buffer.Add(new History(action, delta, time));
                }

                Dictionary<string, List<History>> final = new Dictionary<string, List<History>>();
                final.Add("data", buffer);
                return JsonConvert.SerializeObject(final);
            }
        }

        public override LedgerState Invoke(string[] args, LedgerState previous, DatabaseClient client)
        {
            if (previous.CurrUser < 0)
                previous.phase = "NOT_LOGGED_IN";
            else
            {
                if (!client.Connect(previous))
                    return previous;
               
                float currAmt = Command.GetAmount(previous.CurrUser, client);
                MySqlDataReader rdr = ExecuteQuery(previous.CurrUser, client);
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
