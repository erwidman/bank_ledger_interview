/*
 * Author : Eric Richard Widmann
 * Date   : 1/18/2019
 * Description :
 *      Wrapper for interfacing with mysql DB.
 * 
 */
using System;
using MySql.Data.MySqlClient;
using Ledger.Security;
namespace Ledger
{
    public class DatabaseClient
    {
        //raw connection obj for wrapper
        private MySqlConnection conn = null;

        //encrypted login information
        private readonly string connectionString = @"7rz0nzc0Sgwf+TPU2fxyUfygEKCfgxldg1S47WqBjL07kWh/QVtl3FY9KMuyYk7oKUw+Q0kwGj4p39/dHInLGp56gdkfGknEYOJpYuz+PE/TgHKkKtH+QXgIlWdUQFon3xqaTjo3NIGjODSlXM9RLkUflTUhQ0YV7dujZ2FTyu8=??l3tBbGetUyXQPUKEmo2NqA==";

        /*
         * Description:
         *      Makes raw connection to mysql DB.
         * Params:
         *      LedgerState previous - 
         *          In the event a connection cannot be made, will set the prog state to NO_CONNECTION
         * Return:
         *      If connection was successful, true : otherwise false.  
         */        
        public Boolean Connect(LedgerState state)
        {
            Boolean success = true;
            //if the conn already exist, return true
            if (!(conn is null))
                return success;
            try
            {
                conn = new MySqlConnection(Encryptor.Decrypt(connectionString));
                conn.Open();
            }
            catch (MySqlException e)
            {
                success = false;
                Console.WriteLine(e.ToString());
            }
            if (success)
                Console.WriteLine("+++++Successfully connected to DB!");
            else
                state.phase = "NO_CONNECTION";
            return success;
        }

        /*
         * Description:
         *      If the raw DB connection object has been made – attempts to close it.
         * Params:
         *      none
         * Return:
         *      True if connection is now closed.  
         */
        public Boolean Close()
        {

            Boolean success = true;
            //if connection does not exist, return true
            if (conn is null)
                return success;
            try
            {
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

        /*
         * Description:
         *      Performs stored procedures.
         * Params:
         *      MySqlCommand cmd - 
         *          Command object to be sent to DB.
         * Return:
         *       True if cmd was accepted without raising an error.
         */
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

        /*
         * Description:
         *      Executes queries expected to return rows.
         * Params:
         *      MySqlCommand cmd - 
         *          Command to be sent to DB.
         * Return:
         *       A MySqlDataReader obj containing data or null if error occurs.
         */
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
            catch (MySqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            return rdr;
        }
    }
}
