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
