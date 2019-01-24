/*
 * Author : Eric Richard Widmann
 * Date   : 1/18/2019
 * Description :
 *      Abstract class for all command types. Also contains static utilities used by several command variations.
 * 
 */
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Ledger
{
    public abstract class Command
    {
        public abstract LedgerState Invoke(string [] args,LedgerState previous,DatabaseClient client);

        //Allowed threshold for comparisons involving floating point 
        public static readonly float EPSILON = .009999f;
        public static readonly float MAX_TRANSACTION = 10000000;

        public static MySqlCommand BindStmt(string [] fields, object [] values,string stmt)
        {
            if (fields.Length != values.Length)
                throw new Exception("Parallel arrays field and values do not have the same length.");
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = stmt;
            for (int i = 0; i<fields.Length;i++)
            {
                cmd.Parameters.AddWithValue(fields[i], values[i]);
            }
            return cmd;
        }

        //Referenced https://stackoverflow.com/questions/3404421/password-masking-console-application
        public static string CollectPassword()
        {
            Console.Write("Password: ");
            ConsoleKeyInfo key;
            string pass = "";
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Substring(0, (pass.Length - 1));
                        Console.Write("\b \b");
                    }
                }


            } while (key.Key != ConsoleKey.Enter);
            Console.Write("\n");
            return pass;
        }
        
        public static float ParseAmount(string amount, string failState,LedgerState state)
        {
            if (state.CurrUser == -1)
            {
                state.phase = "NOT_LOGGED_IN";
                return -1;
            }
 

            float amt = -1;

            if (amount.Length > 0)
            {
                bool success = float.TryParse(amount, out amt);
                if (!success || amt>MAX_TRANSACTION)
                {
                    state.phase = failState;
                    amt = -1;
                }
            };
            return amt;
        }

        public static float ParseAmount(string amount)
        {

            float amt = -1;

            if (amount.Length > 0)
            {
                bool success = float.TryParse(amount, out amt);
                if (!success || amt > MAX_TRANSACTION)
                    amt = -1;

            };
            return amt;
        }

        public static bool CompareAmount(float previousAmt, float newAmt, float amt)
        {
            return newAmt >= 0 && previousAmt >= 0 && Math.Abs(newAmt - previousAmt - amt) < EPSILON;
        }


        public static float GetAmount(int id, DatabaseClient client)
        {
            MySqlCommand cmd = Command.BindStmt(
                new string[] { "@id" },
                new object[] { id },
                "select amount from Balance where uid=@id"
            );

            MySqlDataReader rdr = client.SelectQuery(cmd);
            if (!(rdr is null))
            {
                rdr.Read();
                float tmp = rdr.GetFloat(0);
                rdr.Close();
                return tmp;
            }

            return -1;
        }

        public static int GetUID(string username, DatabaseClient client)
        {
            MySqlCommand cmd = Command.BindStmt(
                    new string[] { "@uname" },
                    new string[] { username },
                    "select id from Account where uname=@uname"

                );
            MySqlDataReader rdr = client.SelectQuery(cmd);
            if (!(rdr is null) && rdr.Read())
            {
                int tmp = rdr.GetInt32(0);
                rdr.Close();
                return tmp;

            }
            return -1;
        }
    }
}
