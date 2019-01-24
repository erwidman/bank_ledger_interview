/*
 * Author : Eric Richard Widmann
 * Date   : 1/18/2019
 * Description :
 *      Singleton used to parse user input and invoke associated commands.
 * 
 */
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Ledger;


class CommandDispatch
{

    /*
    *   Tuple array containing regex objects used to test user input and paring
    *   command object used for command execution on a match. 
    */   
    private readonly Tuple<Regex, Command> [] commandRouter;

  
    /*
     * Description:
     *      Instantiates commandRouter object pairing regex expression with their functionality.
     *      If further commands are added to the program, their regex must be added to the list and
     *      matching Command object variation.
     * Params:
     *      void
     * Return:
     *      (is a constructor)    
     * 
     */    
    public CommandDispatch()
    {

        //regex obj's should ignore case and be compiled for faster processing
        RegexOptions tmpOption = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline;

    
        commandRouter = new Tuple<Regex, Command>[]{
            new Tuple<Regex, Command>(
                    new Regex(@"^login\b\s+(.*?)$",tmpOption), 
                    new LoginCommand()
                ),
            new Tuple<Regex, Command>(
                    new Regex(@"^help\b\s*$",tmpOption),
                    new HelpCommand()
                ),
            new Tuple<Regex, Command>(
                    new Regex(@"^deposit\b\s+([0-9]*(\.[0-9]{1,2})?)?$",tmpOption),
                    new DepositCommand()
                ),
            new Tuple<Regex, Command>(
                    new Regex(@"^create\b\s\baccount\b\s+(.*)$",tmpOption),
                    new CreateAccountCommand()
                ),
             new Tuple<Regex, Command>(
                    new Regex(@"^withdraw\b\s+([0-9]*(\.[0-9]{1,2})?)?$",tmpOption),
                    new WithdrawCommand()
                ),
             new Tuple<Regex, Command>(
                    new Regex(@"^quit\b\s*",tmpOption),
                    new QuitCommand()
                ),
             new Tuple<Regex, Command>(
                    new Regex(@"^balance\b\s*$",tmpOption),
                    new BalanceCommand()
                ),
             new Tuple<Regex, Command>(
                    new Regex(@"^history\b\s*$",tmpOption),
                    new HistoryCommand()
                ),
             new Tuple<Regex, Command>(
                    new Regex(@"\blogout\b\s*$",tmpOption),
                    new LogoutCommand()
                ),

        };

    }


    /*
     * Description:
     *      For commands that utilize arguments such as deposit and withdrawal, this function will collect
     *      the values of group's specified in regex for later use.
     * Params:
     *      Match currMatch -
     *          Match object generated from Regex.Matches.
     * Returns:
     *      A string list containing the group values of the match in order of their iteration via currMatch.Groups    
     */    
    private List<string> CollectCommandArguments(Match currMatch)
    {
        List<string> returnVal = new List<string>();
        foreach (Group currGroup in currMatch.Groups)
            returnVal.Add(currGroup.Value);
        return returnVal;
    }


    /*
     * Description:
     *      Called in the main program loop – is responsible for testing the current user input against regex
     *      in commandRouter.
     * Params:
     *      string input -
     *          The input of the user.
     *      LedgerState state-
     *          The LedgerState the command should operate on.
     *      DatabaseClient dbCleint -
     *          The db client the command should utilize.
     * Return:
     *      void    
     * 
     */    
    public void DispatchCommand(string input,LedgerState state, DatabaseClient dbClient)
    {
        bool isMatch = false;
        //iterate through each regex and test against input
        foreach(Tuple<Regex,Command> router in commandRouter)
        {
            MatchCollection tmp = router.Item1.Matches(input);
            //if the regex creates a match, collect arguments and invoke associated command.
            if (tmp.Count > 0)
            {
                List<string> args = CollectCommandArguments(tmp[0]);
                isMatch = true;
                router.Item2.Invoke(args.ToArray(), state, dbClient);
                break;
            }
        }
        //if there is not match, set the LedgerState to FAIL indicating incorrect input.
        if (!isMatch)
            state.phase = "FAIL";
    }
}

