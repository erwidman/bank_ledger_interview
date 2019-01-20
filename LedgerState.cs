using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;




public class LedgerState
{


    private int currUser = -1;

    public int CurrUser
    {
        get
        {
            return currUser;
        }
    }

    public bool Alive { get => alive; set => alive = value; }

    private string uname = "";
    public string phase = "START";

    private Boolean alive = true;
    public Dictionary<string, string> stateText;





    public LedgerState()
    {
        string jsonString = File.ReadAllText("../../stateText.json").Replace("\"", "\'");
        //Console.WriteLine(jsonString);
        stateText = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);

    }



    public void Login(int id, string uname)
    {
        this.currUser = id;
        this.uname = uname;
        this.phase = "LOGIN_SUCCESS";
    }

    public void Logout()
    {
        this.currUser = -1;
        this.uname = null;
        this.phase = "LOGOUT_SUCCESS";
    }


    public void PrintStateText()
    {
        try
        {
            Console.WriteLine(stateText[phase]);
        }
        catch (Exception e)
        {
            Console.Write("!!!ERROR : MISSING TEXT FOR {0} STATE", phase);
        }
    }

 




}

