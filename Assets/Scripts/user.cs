using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class user 
{
    public string userName;
    public List<int> userscore;
    public string localid;
    // public Color color = new Color();

                                                        // edit by Bhuvanesh
    public List<int> Strategy;
    public List<int> Ethics;
    public List<int> Silliness;
    public List<int> SelfControl;
    public List<int> Kindness;
    public List<int> Risk;
    public List<int> Courage;
    public List<int> Practicality;
    public List<int> Responsibility;

                                                        // edit by Bhuvanesh

    public user()
    {
        userName = playerscore.playername;
       // userscore = playerscore.playscore;
        localid = playerscore.localid;
    }

}
public class UserDetails
{
    public string userName { get; set; }
    public string localid { get; set; }
    public List<int> userscore { get; set; }
    public List<Color> color { get; set; }

}
//public class Error
//{
//    public string message { get; set; }
//    public string domain { get; set; }
//    public string reason { get; set; }
//}

public class Error
{
    public int code { get; set; }
    public string message { get; set; }
  //  public IList<Error> errors { get; set; }
}

public class Example
{
    public Error error { get; set; }
}

