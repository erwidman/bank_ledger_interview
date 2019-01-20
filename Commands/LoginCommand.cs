using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Ledger.Security;

namespace Ledger
{
    public class LoginCommand : Command
    {
        public LoginCommand()
        {

   
        }




        private int AttemptLogin(string uname, string pass,DatabaseClient client)
        {
            
            MySqlCommand cmd = 
                Command.BindStmt(
                    new string[] { "@uname"},
                    new string [] {uname},
                    "select password,id from Account where uname=@uname"
                );
            MySqlDataReader rdr =  client.SelectQuery(cmd);
            if(!(rdr is null) && rdr.Read())
            {
                string readPassword = rdr.GetString(0);
                int success = rdr.GetInt32(1);
                rdr.Close();
                if (comparePasswords(pass,readPassword))
                { 
                    Console.WriteLine(success);
                    return success;
                }

            }

            return -1;


        }

        private bool comparePasswords(string pass, string readPassword)
        {
            return Encryptor.Decrypt(readPassword).Equals(pass);
        }

        public override LedgerState Invoke(string [] args,LedgerState previous, DatabaseClient client)
        {

            if (previous.CurrUser >= 0)
            {
                previous.phase="ALREADY_LOGGED";
                return previous;
            }


            string uname = null;
            if (args[1].Length>0)
                uname = args[1];
            else if (args[1].Length==0)
                previous.phase = "NO_USER_NAME";
            if(!(uname is null))
            {
                if (!client.Connect(previous))
                    return previous;

                string pass = Command.CollectPassword();

                int uid = AttemptLogin(uname, pass, client);
                if (uid < 0)
                    previous.phase = "LOGIN_FAIL";
                else
                {
                    previous.phase = "LOGIN_SUCCESS";
                    previous.Login(uid, uname);
                }
            }

            client.Close();

            return previous;
            
        }
    }
}
