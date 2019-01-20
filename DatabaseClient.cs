using System;
using MySql.Data.MySqlClient;
namespace Ledger
{
    public class DatabaseClient
    {

        private MySqlConnection conn = null;

        private readonly string connectionString = @"server=eric-widmann-dev.cps5d6vvbslb.us-east-1.rds.amazonaws.com;userid=widmann;password=developmentDB;database=widmann_dev";
        public DatabaseClient()
        {
        }

        public Boolean Connect(LedgerState previous)
        {
            Boolean success = true;
            if (!(conn is null))
                return success;
            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();
            }
            catch(MySqlException e)
            {
                success = false;
                Console.WriteLine(e.ToString());
            }
            if (success)
                Console.WriteLine("+++++Successfully connected to DB!");
            else
                previous.phase = "NO_CONNECTION";
            return success;
        }

        public Boolean Close()
        {
         
            Boolean success = true;
            if (conn is null)
                return success;
            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Close();
                conn = null;
            }
            catch (MySqlException e)
            {
                success = false;
                Console.WriteLine(e.ToString());
            }
            if (success)
                Console.WriteLine("+++++Successfully closed DB client!");
            return success;
        }

        public Boolean Execute(MySqlCommand cmd)
        {
            if (conn is null)
                return false;
            Boolean success = true;

            try
            {

                cmd.Connection = conn;
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                success = false;
                Console.Write(ex.ToString());
            }
            return success;
        }

        public MySqlDataReader SelectQuery(MySqlCommand cmd)
        {
            if (conn is null)
                return null;

            MySqlDataReader rdr = null;
            try
            {
          
                cmd.Connection = conn;
                cmd.Prepare();
                rdr = cmd.ExecuteReader();

            }
            catch(MySqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            return rdr;
        }
    }
}
