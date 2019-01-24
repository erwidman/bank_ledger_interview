/*
    Author      : Eric Richard Widmann
    Date        : 1/23/2019
    Description :
        Implementation of |Login <username>| command.
*/
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Ledger.Security;

namespace Ledger
{
    public class LoginCommand : Command
    {

        /*
            Description:
                Helper function utilized by invoke to login.
            Params:
                string uname - 
                    Username of login attempt.
                string pass-
                    Unencrypted password to compare against cyphertext.
                DatabaseClient client -
                    Client which will perform the query.
            Return:
                Id of user if successful, or -1.

        */
        private int AttemptLogin(string uname, string pass,DatabaseClient client)
        {
            
            MySqlCommand cmd = 
                Command.BindStmt(
                    new string[] { "@uname"},
                    new string [] {uname},
                    "select password,id from Account where uname=@uname"
                );
            MySqlDataReader rdr =  client.SelectQuery(cmd);
            //if query was successful
            if(!(rdr is null) && rdr.Read())
            {
                string readPassword = rdr.GetString(0);
                int id = rdr.GetInt32(1);
                rdr.Close();
                if (ComparePasswords(pass,readPassword))
                    return id;

            }

            return -1;


        }

        /*
            Description:
                Function used to compare unencryped password against cyphertext from DB
            Params:
                string pass-
                    Plaintext password inputed
                string readPassword - 
                    Cyphertext password from DB.
        */
        private bool ComparePasswords(string pass, string readPassword)
        {
            return Encryptor.Decrypt(readPassword).Equals(pass);
        }

        //See Command.cs
        public override LedgerState Invoke(string [] args,LedgerState state, DatabaseClient client)
        {

            //user already logged in
            if (state.CurrUser >= 0)
            {
                state.phase="ALREADY_LOGGED";
                return state;
            }


            string uname = null;
            if (args[1].Length>0)
                uname = args[1];
            else if (args[1].Length==0)
                state.phase = "NO_USER_NAME";
            if(!(uname is null))
            {
                // client cannot connect
                if (!client.Connect(state))
                {
                    state.phase = "UNABLE_TO_CONNECT";
                    return state;
                }

                string pass = Command.CollectPassword();

                int uid = AttemptLogin(uname, pass, client);
                if (uid < 0)
                    state.phase = "LOGIN_FAIL";
                else
                {
                    state.phase = "LOGIN_SUCCESS";
                    state.Login(uid, uname);
                }
            }

            client.Close();

            return state;
            
        }
    }
}
