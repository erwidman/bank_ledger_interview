/*
    Author      : Eric Richard Widmann
    Date        : 1/23/2019
    Description :
        Implementation of |history| command.
*/
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
namespace Ledger
{
    public class HistoryCommand : Command
    {

        /*
            Description:
                Prints table of getHistory query.
            Params:
                MySqlDataReader rdr-
                    Data reader returned from execution of history query.
            Return:
                void
        */
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

        /*
            Description:
                Queries Database for last 100 actions of user identified by uid.
            Params :
                int uid -
                    Unique identifier of user.
                DatabaseClient client -
                    Client which will perform the query.
            Return:
                A MySqlDataReader resulting from query execution.

        */
        private MySqlDataReader ExecuteGetHistory(int uid,DatabaseClient client)
        {
            MySqlCommand cmd = Command.BindStmt(
                    new string[] { "@id" },
                    new object[] { uid },
                    "select action, delta, time from History where id=@id and display=1 order by actionID desc limit 100"
               );
            return client.SelectQuery(cmd);
        }



        /*
            Used to serialize history results into JSON format
        */
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

        /*  
            Description:
                Function used by webapi to retrieve history as a JSON string string.
            Params:
                int uid -
                    Unique identifier of user.
                DatabaseClient client -
                    Client which will perform the query.

        */
        public string GetHistory(int uid, DatabaseClient client)
        {
          
            MySqlDataReader rdr = ExecuteGetHistory(uid, client);
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

        //see Command.cs
        public override LedgerState Invoke(string[] args, LedgerState state, DatabaseClient client)
        {
            //if user is not logged in
            if (state.CurrUser < 0)
                state.phase = "NOT_LOGGED_IN";
            else
            {
                // client cannot connect
                if (!client.Connect(state))
                {
                    state.phase = "UNABLE_TO_CONNECT";
                    return state;
                }
                
                
                float currAmt = Command.GetAmount(state.CurrUser, client);
                MySqlDataReader rdr = ExecuteGetHistory(state.CurrUser, client);
                if (!(rdr is null))
                {
                    Console.WriteLine("|Current Balance : ${0:F2}", currAmt);
                    PrintHistory(rdr);
                    state.phase = "HISTORY_SUCCESS";
                }
                else
                    state.phase = "HISTORY_FAIL";
                client.Close();
            }


            return state;
        }
    }
}
