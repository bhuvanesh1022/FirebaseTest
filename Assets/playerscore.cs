using System.Collections;
using System.Collections.Generic;

using Proyecto26;
using FullSerializer;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class playerscore : MonoBehaviour
{
    public Text scoretext,Display_txt;
    public InputField getscoretext;
    public InputField nametext;

    public InputField emailtext;
    public InputField usernametext;
    public InputField passwordtext;


    private System.Random random = new System.Random();
    public static int playscore;
    public static string playername;
    user user = new user();

    public static string localid;
    private string idtoken;

    private string getLocalId;

    private string Authkey = "AIzaSyBUfg4avo3uIxInDSOYDnGeiMDCSu4B6C0";
    private string databaseURL = "https://test-project-bcd07.firebaseio.com/users";

    public static fsSerializer serializer = new fsSerializer();
    void Start()
    {
        playscore = random.Next(0,100);
        scoretext.text = "score:" + playscore;
    }

    public void onsubmit()
    {
        posttodatabase();
    }
    public void ongetscore()
    {
        GetlocalId();
    }
    private void updatescore()
    {
        scoretext.text = "score" + user.userscore;
    }
  private void posttodatabase(bool emptyscore=false)
    {
        user user = new user();
        if(emptyscore )
        {
            user.userscore = 0;
        }
         RestClient.Put(url:databaseURL+"/"+localid+ ".json", user);
    }
    private void retrivefromdatabase()
    {
        RestClient.Get<user>(url:databaseURL + "/" + getLocalId + ".json").Then(onResolved:response=>
        {
            user = response;
            updatescore();
            print("44444");
        });
    }
    public void signInUserButton()
    {
        signInuser(emailtext.text, passwordtext.text);
    }
    public void signUpUserButton()
    {
        signUpuser(emailtext.text, usernametext.text, passwordtext.text);
    }

    private void signUpuser(string email, string username, string password)
    {
         string userdata = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>(url:"https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=" + Authkey, bodyString:userdata).Then(
            onResolved:response =>  
            {
                idtoken = response.idToken;
                localid = response.localId;
                playername = username;
                posttodatabase(emptyscore:true);
                Display_txt.text = "Welcome";
                Invoke("stopmsg", 1f);
            }).Catch(error =>
            {
                Debug.Log(error);
                Display_txt.text = "Wrong credential";
                Invoke("stopmsg", 1f);
            });
    }
    private void signInuser(string email, string password)
    {
        string userdata = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>(url:"https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + Authkey, bodyString: userdata).Then(
            onResolved: response =>
            {
                idtoken = response.idToken;
                localid = response.localId;
                GetUsername();
                Display_txt.text = "Welcome";
                Invoke("stopmsg", 1f);

            }).Catch(error =>
            {
                Debug.Log(error);
                Display_txt.text = "Wrong credential";
                Invoke("stopmsg",1f);

            });
    }
    void stopmsg()
    {
        Display_txt.text = "";

    }
    private void GetUsername()
    {
        RestClient.Get<user>(url:databaseURL + "/" + localid + ".json").Then(onResolved: response =>
        {
            playername= response.userName;
        });
    }

    private void GetlocalId()
    {

         RestClient.Get(url:databaseURL + ".json").Then(onResolved: response =>

        {
            var username = getscoretext.text;
               print("111111");

            fsData userdata = fsJsonParser.Parse(response.Text);
            Dictionary<string, user> users = null;
            serializer.TryDeserialize(userdata,ref users);

            foreach(var user in users.Values)
            {
                print("2222");

                if (user.userName==username)
                {
                    print("3333");
                    getLocalId = user.localid;
                    retrivefromdatabase();
                    Display_txt.text = "welcome";
                    Invoke("stopmsg", 1f);
                    break;
                }
                else
                {
                    Display_txt.text = "Incorrect username";
                    Invoke("stopmsg", 1f);
                }
            }
        }).Catch(error =>
        {
            Debug.Log(error);
            Display_txt.text = "Wrong credential";
            Invoke("stopmsg", 1f);

        }
        );

    }

}
//?auth=" + idtoken