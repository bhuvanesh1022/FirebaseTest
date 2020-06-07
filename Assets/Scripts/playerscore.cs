using System.Collections;
using System.Collections.Generic;

using Proyecto26;
using FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System;
using Newtonsoft.Json;
using System.Text;
using System.Reflection;
using Newtonsoft.Json.Linq;
using DecisionFramework;

public class playerscore : MonoBehaviour
{
    public static playerscore _pScore;
   
    public UserGameData ugd;
    public string usergamedata;
    public string lastTraits;

    public int gameSceneId;
    public Text Display_txt;
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

    public static string playername;
    public user user = new user();//arthi---

    public static string localid;
    private string idtoken;
    private string getLocalId;

    private string Authkey = "AIzaSyBUfg4avo3uIxInDSOYDnGeiMDCSu4B6C0";
    private string databaseURL = "https://test-project-bcd07.firebaseio.com/users";
    public static fsSerializer serializer = new fsSerializer();

    public GameObject page1, page2, page3;

    public void Awake()
    {
        ReignslikeGameManager.OnSessionEnded += DoSubmit;
    }

    public void DoSubmit(UserGameData traitResults)
    {
        ugd = traitResults;

        if (ugd.Records.Count != 0)
            foreach (var record in ugd.Records)
                if (record.value != 0)
                    usergamedata += (string.IsNullOrEmpty(usergamedata) ? "" : "\n") + string.Format("{0} : {1}", record.name, record.value);
                else
                    return;

        Debug.Log(usergamedata);
    }

    private void OnEnable()
    {
        if (_pScore == null) _pScore = this;
        else
        {
            if (_pScore != this) Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        page1.SetActive(true);
        page2.SetActive(false);
        page3.SetActive(false);
    }

    public void onsubmit()
    {
        posttodatabase(true);
        Debug.Log("scoreSubmitted");
    }

    public void ongetscore()
    {
        Debug.Log("gettingScore");
        GetlocalId();
    }

    private void posttodatabase(bool isScoreSubmit)
    {
        try
        {
            List<string> gameDatas = new List<string>();

            user user = new user();

            RestClient.Get(url: databaseURL + "/" + localid + ".json").Then(onResolved: response =>
            {
                UserDetails ud = new UserDetails();
                if (response != null)
                {
                    ud = JsonConvert.DeserializeObject<UserDetails>(response.Text);

                    if (ud != null && ud.traits != null)
                    {
                        gameDatas = ud.traits;
                    }
                }
                

                if (isScoreSubmit)
                {
                    if (gameDatas!=null && gameDatas.Count == 10)
                    {
                        gameDatas.RemoveAt(0);
                    }

                    gameDatas.Add(usergamedata);
                }
                    
                user.userName = playername;

                user.localid = localid;

                user.traits = gameDatas;

                RestClient.Put(url: databaseURL + "/" + localid + ".json", user);

                print("tesy---" + playername);

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
         });
    }

    public void signInUserButton()
    {
        PlayerPrefs.SetString("Email", emailtext.text);
        PlayerPrefs.SetString("Password", passwordtext.text);

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
                 user.userName = playername;
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
    {                                                                       //edit by Bhuvanesh

        SceneManager.LoadScene(gameSceneId);
      
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

             Invoke("ongetscore", 1f);

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

            getLocalId = userdata.localid;

            if (userdata.traits != null && userdata.traits.Count > 0)
            {
                lastTraits = userdata.traits[userdata.traits.Count - 1];

            }
           
            Debug.Log(lastTraits);
            Invoke("stopmsg", 1f);
            Invoke("Shownextpage", 1f);

        }).Catch(error =>
        {
            Debug.Log(error);
            Display_txt.text = "Wrong credential";
            Invoke("stopmsg", 1f);

        }
       );

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
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