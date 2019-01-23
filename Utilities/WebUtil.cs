using System;
using System.Text;
using Ledger.Security;
using MySql.Data.MySqlClient;

namespace Ledger.WebAPI
{
    public static class WebUtil
    {

        //public static readonly DatabaseClient webCleint = new DatabaseClient();

        public static string[] ParseAuth(string auth)
        {

            return null;
        }


        public static string[] DecodeAuth(string auth)
        {
            string uname = null;
            string pass = null;
            try
            {
                auth = auth.Substring("Begin ".Length).Trim();
                Encoding enc = Encoding.GetEncoding("iso-8859-1");
                auth = enc.GetString(Convert.FromBase64String(auth));
                int colonIndex = auth.IndexOf(':');
                uname = auth.Substring(0, colonIndex);
                pass = auth.Substring(colonIndex + 1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
            return new string[] { uname, pass };
        }


        private static int QueryAuth(string uname, string pass,DatabaseClient webClient)
        {

            MySqlCommand cmd =
              Command.BindStmt(
                  new string[] { "@uname" },
                  new string[] { uname },
                  "select password,id from Account where uname=@uname"
              );
            MySqlDataReader rdr = webClient.SelectQuery(cmd);
            if (!(rdr is null) && rdr.Read())
            {
                string readPassword = rdr.GetString(0);
                int success = rdr.GetInt32(1);
                rdr.Close();
                if (Encryptor.Decrypt(readPassword).Equals(pass))
                    return success;


            }
            
            return -1;
        }


 

        public static bool VerifyAuth(string authString,DatabaseClient webClient)
        {

            String[] usernamePassword = DecodeAuth(authString);
            if (usernamePassword is null)
                return false;
        

       
            return QueryAuth(usernamePassword[0], usernamePassword[1],webClient) > 0;

        }
    }
}

