using System.Collections;
using System.Collections.Generic;

using Proyecto26;
using FullSerializer;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System;
using Newtonsoft.Json;
using System.Text;
using System.Reflection;
using Newtonsoft.Json.Linq;

public class playerscore : MonoBehaviour
{
    public Text scoretext, Display_txt, Previous_scoretxt;
    public Text getscoretext, submittxt;

    //public InputField getscoretext;
    public InputField nametext;

    // signin
    public InputField emailtext;
    //public InputField usernametext;
    public InputField passwordtext;
    //signup
    public InputField emailtext1;
    public InputField usernametext1;
    public InputField passwordtext1;
    public Text disUsername;

    private System.Random random = new System.Random();

    public static float playscore;
    public static string playername;
    user user = new user();//arthi---

    public static string localid;
    private string idtoken;
    private string getLocalId;

    private string Authkey = "AIzaSyBUfg4avo3uIxInDSOYDnGeiMDCSu4B6C0";
    private string databaseURL = "https://test-project-bcd07.firebaseio.com/users";
    public static fsSerializer serializer = new fsSerializer();

    [Header("scoretxt")]
    // score details
    public Text slider_txt;
    public float slider_val;
    public Slider _slider;

    public GameObject page1, page2, page3;
    void Start()
    {
        page1.SetActive(true);
        page2.SetActive(false);
        page3.SetActive(false);

        // playscore = random.Next(0,100);
        // scoretext.text = "score:" + playscore;
    }

    public void onsubmit()
    {
        posttodatabase(true);
    }
    public void ongetscore()
    {
        GetlocalId();
    }
    private void updatescore()
    {
        scoretext.text = "Score:" + user.userscore;
    }
    private void posttodatabase(bool isScoreSubmit)
    {
        try
        {

            List<int> scr = new List<int>();
            user user = new user();

            RestClient.Get(url: databaseURL + "/" + localid + ".json").Then(onResolved: response =>
            {
                UserDetails ud = new UserDetails();
                if (response != null)
                {
                    ud = JsonConvert.DeserializeObject<UserDetails>(response.Text);
                    if (ud != null && ud.userscore!=null)
                        scr = ud.userscore;
                }
                

                if (isScoreSubmit)
                {
                    if (scr!=null && scr.Count == 10)
                    {
                        scr.RemoveAt(0);
                    }
                    scr.Add(Convert.ToInt32(_slider.value));
                }
                    // playername = usernametext.text;//---page2
                    user.userName = playername;
                print("tesy---" + playername);
                user.localid = localid;
                user.userscore = scr;
                RestClient.Put(url: databaseURL + "/" + localid + ".json", user);

                if (isScoreSubmit)
                {
                    submittxt.text = "Score submitted";

                    Invoke("stopmsg", 2f);
                }
            }).Catch(error =>
            {
                Debug.Log(error);

            });


        }
        catch (Exception ex)
        {

        }

    }

    private void retrivefromdatabase()
    {
        RestClient.Get<user>(url: databaseURL + "/" + localid + ".json").Then(onResolved: response =>
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
        signUpuser(emailtext1.text, usernametext1.text, passwordtext1.text);
    }

    private void signUpuser(string email, string username, string password)
    {
        string userdata = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>(url: "https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=" + Authkey, bodyString: userdata).Then(
            onResolved: response =>
             {
                 idtoken = response.idToken;
                 localid = response.localId;
                 playername = username;
                 posttodatabase( false);
                 Display_txt.text = "Welcome " + playername;
                 Invoke("stopmsg", 1f);
                 Invoke("Shownextpage", 1f);

             }).Catch(error =>
             {
                 RequestException rr = new RequestException();
                 rr = (RequestException)error;
                 string js = rr.Response;
                 var err = JsonConvert.DeserializeObject<Example>(js);
                 Debug.Log(error);
                 Display_txt.text = err.error.message;
                 Invoke("stopmsg", 3f);

             });
    }
    private void signInuser(string email, string password)
    {
        string userdata = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";

        RestClient.Post<SignResponse>(url: "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + Authkey, bodyString: userdata).Then(
            onResolved: response =>
            {
                print("In----");
                idtoken = response.idToken;
                localid = response.localId;
                Display_txt.text = "Loading.... ";
                GetUsername();


            }).Catch(error =>
            {
                RequestException rr = new RequestException();
                rr = (RequestException)error;
                string js = rr.Response;
                var err = JsonConvert.DeserializeObject<Example>(js);
                Debug.Log(error);
                Display_txt.text = err.error.message;
                Invoke("stopmsg", 3f);

            });
    }
    void stopmsg()
    {
        submittxt.text = "";
        Display_txt.text = "";

    }
    void Shownextpage()
    {
        page1.SetActive(false);
        page2.SetActive(false);
        page3.SetActive(true);
        print("page2---" + playername);
        disUsername.text = "Welcome " + playername;

    }
    private void GetUsername()
    {
        print("1----");
        RestClient.Get<user>(url: databaseURL + "/" + localid + ".json").Then(onResolved: response =>
         {
             print("getusername----");
             playername = response.userName;
             print("s-------" + response.userName);
            // playername=usernametext.text;
            user.userName = playername;//----
            Display_txt.text = "Welcome " + playername;
             Invoke("stopmsg", 1f);
             Invoke("Shownextpage", 1f);

         }).Catch(error =>
         {
             RequestException rr = new RequestException();
             rr = (RequestException)error;
             string js = rr.Response;
             var err = JsonConvert.DeserializeObject<Example>(js);
             Debug.Log(error);
             Display_txt.text = err.error.message;
             Invoke("stopmsg", 3f);

         });
    }

    private void GetlocalId()
    {

        RestClient.Get(url: databaseURL + "/" + localid + ".json").Then(onResolved: response =>

        {
            var username = getscoretext.text;

            var userdata = JsonConvert.DeserializeObject<UserDetails>(response.Text);

            // if (userdata.userName==username)
            //{
            print("3333");
            getLocalId = userdata.localid;
            var sb = new StringBuilder();
            if (userdata.userscore != null && userdata.userscore.Count > 0)
            {
                foreach (var i in userdata.userscore)
                {
                    sb.Append(Convert.ToString(i));
                    sb.Append("\n");

                }
            }
            Previous_scoretxt.text = "Previous score:" + "\n" + Convert.ToString(sb);// getscore
                                                                                     // retrivefromdatabase();

            //}
            //else
            //{
            //    Display_txt.text = "Incorrect username";
            //    Invoke("stopmsg", 1f);
            //}
        }).Catch(error =>
        {
            Debug.Log(error);
            Display_txt.text = "Wrong credential";
            Invoke("stopmsg", 1f);

        }
       );

    }
    // adding score
    public void _sliderfun()
    {
        playscore = _slider.value;
        scoretext.text = "Score:" + playscore;
    }
    //
    void Update()
    {
        //if(Input.GetMouseButtonDown(0))
        //{
        //    Display_txt.text = "";

        //}
    }
    // 
    public void SignUpPage1()
    {
        page1.SetActive(false);
        page2.SetActive(true);
        page3.SetActive(false);
    }
}
//?auth=" + idtoken

   // ".write": "auth.uid !=null"