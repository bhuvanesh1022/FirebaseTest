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

