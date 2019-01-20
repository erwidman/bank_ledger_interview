/*
 * Author  : Eric Richard Widmann
 * Date    : 1/18/2019
 * Description :
 *      Simple Bank ledger CLI written for AltSource interview process.
 * 
 */
using System;
using System.Configuration;
using System.Security.Cryptography;
using ArtisanCode.SimpleAesEncryption;

namespace Ledger
{
    class MainClass
    {
   
        public static void Main(string[] args)
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
                dispatcher.DispatchCommand(currentText,currentState,dbClient);
            }

        }
    }
}
