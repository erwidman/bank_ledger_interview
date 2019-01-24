/*
    Author      : Eric Richard Widmann
    Date        : 1/23/2019
    Description :
        Implementation of |create account <username>| command.
*/
using System;
using MySql.Data.MySqlClient;
using Ledger.Security;
using System.Text.RegularExpressions;

namespace Ledger
{
    public class CreateAccountCommand : Command
    {

        /*
            Description : 
                Confirms that username and password are 6 to 30 alphanumeric characters
            Params:
                string username - 
                    Username to validate.
                string password -
                    Password to validate.
            Return:
                True if username and password are valid.


        */
        private static bool ValidateUsernameAndPassword(string username,string password){
            Regex r = new Regex("^[a-zA-Z0-9]*$");
            return username.Length >= 6 && password.Length >= 6 && username.Length <= 30 && password.Length <= 30 && r.Match(username).Success && r.Match(password).Success; 

        }

        /*
           Description:
                Queries Account table searching for parameter uname.
           Params:
                string uname -
                    Username to search for.
                DatabaseClient client -
                    Client which will perform the search
            Return:
                True if the user exist.

        */
        private bool UserExist(string uname, DatabaseClient client)
        {
            //execute command
            MySqlCommand cmd = Command.BindStmt(
                    new string[] { "@uname" },
                    new string[] { uname },
                    "select count(*) from Account where uname=@uname"
                );
            MySqlDataReader rdr = client.SelectQuery(cmd);
            int id = -1;
            if(rdr.Read()){
                id = rdr.GetInt32(0);
                rdr.Close();
            }
            return id > 0;



        }

        /*
            Description:
                Executes createAccount stored procedure
            Params:
                string uname -
                    Username to insert.
                string pass -
                    Password to insert.
                DatabaseClient client -
                    Client which will perform the query.
            Return:
                void
        */
        private void ExecuteCreate(string uname, string pass,DatabaseClient client)
        {
            pass = Encryptor.Encrypt(pass);
            MySqlCommand cmd = Command.BindStmt(
                    new string[] { "@uname", "@pass" },
                    new string[] { uname, pass },
                    "call createAccount(@uname,@pass)"
                );
            client.Execute(cmd);

        }


        /*
            Description:
                Helper function called by Invoke to create user.
            Params:
                string uname -
                    Username to insert.
                LedgerState state -
                    LedgerState to modify.
                DatabaseClient client -
                    Client which will perform the query.
            Return:
                void
    
        */
        private void CreateAccount(string uname, LedgerState state, DatabaseClient client)
        {
            if (!client.Connect(state))
            {
                state.phase = "UNABLE_TO_CONNECT";
                return;
            }
            //check if user already exist, set state
            bool alreadyExist = UserExist(uname, client);
            if (alreadyExist)
                state.phase = "USER_ALREADY_EXIST";
            else
            {
                string pass = Command.CollectPassword();
                Console.WriteLine("Enter password again.");
                string pass2 = Command.CollectPassword();
                if (!pass.Equals(pass2))
                {
                    state.phase = "PASSWORD_MISMATCH";
                    return;
                }
                if (!ValidateUsernameAndPassword(uname, pass))
                {
                    state.phase = "INVALID_USER_OR_PASS";
                    return;
                }
                ExecuteCreate(uname, pass, client);
                alreadyExist = UserExist(uname, client);
                if (!alreadyExist)
                    state.phase = "FAILED_TO_CREATE_USER";
                else
                    state.phase = "USER_CREATED";
            }


            client.Close();
        }

        /*
            Description:
                Function used by webapi to create account.
            Params:
                string uname -
                    Username to insert.
                string pass -
                    Encrypted cyphertext to insert.
                DatabaseClient client-
                    Client which will perform the query.
            Return:
                True if query was executed.

        */
        public bool CreateAccount(string uname, string pass,DatabaseClient client)
        { 
            bool alreadyExist = UserExist(uname, client);
            if (!alreadyExist)
            {
                ExecuteCreate(uname, pass,client);
                return true;
            }
            else
                return false;

        }

        //see Command.cs
        public override LedgerState Invoke(string[] args, LedgerState previous, DatabaseClient client)
        {

            if (args[1].Length > 0)
                CreateAccount(args[1], previous, client);
            
            else
                previous.phase = "NO_USERNAME_PROVIDED";
            return previous;
        }
    }
}