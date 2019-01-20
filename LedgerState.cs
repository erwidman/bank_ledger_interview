/*
 * Author : Eric Richard Widmann
 * Date   : 1/18/2019
 * Description :
 *      Object used to encapsulate program state.
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;




public class LedgerState
{


    //Variable containing id of user currently logged in
    private int currUser = -1;
    public int CurrUser
    {
        get
        {
            return currUser;
        }
    }

    //Variable indicating if program should continue or exit.
    private Boolean alive = true;
    public bool Alive { get => alive; set => alive = value; }

    //username of current user
    private string uname = "";

    //string used to represent state of the program, matches with JSON object containing textual outputs loaded into stateText Dictionary.
    public string phase = "START";


    //Dictionary containing text used for all program states.
    public Dictionary<string, string> stateText;




    //When instantiated, loads stateText.json into a Dictionary for future printing.
    public LedgerState()
    {
        string jsonString = File.ReadAllText("../../stateText.json").Replace("\"", "\'");
        //Console.WriteLine(jsonString);
        stateText = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);

    }


    /*
     * Description:
     *      Function used to update state object with login information.
     * Params:
     *      int id -
     *          The ID of the user who logged in (is a field in the DB).
     *      string uname -
     *          Username of user who logged in.
     *     
     * Return:
     *      void
     * 
     */    
    public void Login(int id, string uname)
    {
        this.currUser = id;
        this.uname = uname;
        this.phase = "LOGIN_SUCCESS";
    }


    /*
     * Description:
     *      Updates program state after a logout.    
     * Params:
     *      none
     * Return:
     *      void    
     * 
     */
    public void Logout()
    {
        this.currUser = -1;
        this.uname = null;
        this.phase = "LOGOUT_SUCCESS";
    }

    /*
     * Description:
     *      Prints the text stored in stateText Dictionary for current state, called in main prog loop.    
     * Params:
     *      none
     * Return :
     *      void    
     * 
     */
    public void PrintStateText()
    {
        try
        {
            Console.WriteLine(stateText[phase]);
        }
        catch (Exception)
        {
            Console.Write("!!!ERROR : MISSING TEXT FOR {0} STATE", phase);
        }
    }

 




}

