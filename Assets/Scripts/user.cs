using System;
using System.Collections;
using System.Collections.Generic;
using DecisionFramework;
using UnityEngine;

[Serializable]
public class user 
{
    public string userName;
    public string localid;
   
    public List<string> traits;                          

    public user()
    {
        userName = playerscore.playername;
        localid = playerscore.localid;
    }

}
public class UserDetails
{
    public string userName { get; set; }
    public string localid { get; set; }
    public List<int> userscore { get; set; }
    public List<Color> color { get; set; }

    public List<string> traits { get; set; }
}

public class Error
{
    public int code { get; set; }
    public string message { get; set; }
}

public class Example
{
    public Error error { get; set; }
}

