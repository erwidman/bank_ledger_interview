using System;
using MySql.Data.MySqlClient;
using Ledger.Security;

namespace Ledger
{
    public class CreateAccountCommand : Command
    {
        public CreateAccountCommand()
        {

        }
        private bool UserExist(string uname,DatabaseClient client)
        {
            MySqlCommand cmd = Command.BindStmt(
                    new string[] { "@uname" },
                    new string[] { uname },
                    "select count(*) from Account where uname=@uname"
                );
            MySqlDataReader rdr = client.SelectQuery(cmd);
            rdr.Read();
            int id = rdr.GetInt32(0);
            rdr.Close();
            return id > 0;


            
        }

        private void CreateAccount(string uname, LedgerState state, DatabaseClient client)
        {
            if (!client.Connect(state))
                return;
            //check
            bool alreadyExist = UserExist(uname,client);
            if (alreadyExist)
                state.phase = "USER_ALREADY_EXIST";
            else
            {
                string pass = Command.CollectPassword();
                pass = Encryptor.Encrypt(pass);
                MySqlCommand cmd = Command.BindStmt(
                        new string [] {"@uname","@pass"},
                        new string [] {uname, pass},
                        "call createAccount(@uname,@pass)"
                    );
                client.Execute(cmd);
                alreadyExist = UserExist(uname, client);
                if (!alreadyExist)
                    state.phase = "FAILED_TO_CREATE_USER";
                else
                    state.phase = "USER_CREATED";
            }


            client.Close();
        }

        public override LedgerState Invoke(string[] args, LedgerState previous, DatabaseClient client)
        {
           
            if (args[1].Length > 0)
            {
                CreateAccount(args[1],previous,client);
            }
            else
                previous.phase = "NO_USERNAME_PROVIDED";
            return previous;
        }
    }
}
