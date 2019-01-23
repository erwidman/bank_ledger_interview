/*
 * Author  : Eric Richard Widmann
 * Date    : 1/18/2019
 * Description :
 *      Simple Bank ledger CLI written for AltSource interview process.
 * 
 */
using System;
using System.Configuration;
using System.Text.RegularExpressions;
using Microsoft.Owin.Hosting;



namespace Ledger
{
    class MainClass
    {
   
        private static void BeginCLILoop()
        {
            //maintains state associated with bank ledger
            LedgerState currentState = new LedgerState();
            //used to parse user input and invoke proper function
            CommandDispatch dispatcher = new CommandDispatch();
            //wrapper for mySql client
            DatabaseClient dbClient = new DatabaseClient();
            //While the app has not been quit
            while (currentState.Alive)
            {
                //print text associated with current state
                currentState.PrintStateText();
                Console.Write("\n>>> ");
                //collect user input
                string currentText = Console.ReadLine();
                //parse input and invoke related command
                dispatcher.DispatchCommand(currentText, currentState, dbClient);
            }


        }

        public static void Main(string[] args)
        {
            Console.WriteLine(args.Length);

           //if (args.Length >= 1 && Regex.Match(args[0], @"^--webapi$").Success)
          if(true)
            {
                string addr = "http://localhost:9000/";
                using (WebApp.Start(url: addr))
                {
                    Console.WriteLine("Service hosted on {0}",addr);
                    System.Threading.Thread.Sleep(-1);

                }
            }
            else
                BeginCLILoop();


       

        }
    }
}
