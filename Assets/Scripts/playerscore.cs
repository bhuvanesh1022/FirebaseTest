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

public class playerscore : MonoBehaviour
{
    public static playerscore _pScore;
    public int c1;
    public int e1;
    public int k1;
    public int p1;
    public int r1;
    public int r2;
    public int s1;
    public int s2;
    public int s3;
    public string lastTraits;

    public int gameSceneId;
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
    public user user = new user();//arthi---

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

    private void OnEnable()
    {
        if (_pScore == null)
        {
            _pScore = this;
        }
        else
        {
            if (_pScore != this)
            {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        page1.SetActive(true);
        page2.SetActive(false);
        page3.SetActive(false);

        //if (PlayerPrefs.HasKey("Email"))
        //{
        //    signInuser(PlayerPrefs.GetString("Email"), PlayerPrefs.GetString("Password"));
        //}

        // playscore = random.Next(0,100);
        // scoretext.text = "score:" + playscore;
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
    private void updatescore()
    {
        scoretext.text = "Score:" + user.userscore;
    }
    private void posttodatabase(bool isScoreSubmit)
    {
        try
        {

            List<int> scr = new List<int>();

            List<int> courage = new List<int>();
            List<int> ethics = new List<int>();
            List<int> kindness = new List<int>();
            List<int> practicality = new List<int>();
            List<int> responsibility = new List<int>();
            List<int> risk = new List<int>();
            List<int> selfcontrol = new List<int>();
            List<int> silliness = new List<int>();
            List<int> strategy = new List<int>();

            user user = new user();

            RestClient.Get(url: databaseURL + "/" + localid + ".json").Then(onResolved: response =>
            {
                UserDetails ud = new UserDetails();
                if (response != null)
                {
                    ud = JsonConvert.DeserializeObject<UserDetails>(response.Text);
                    if (ud != null && ud.userscore != null)
                    {
                        scr = ud.userscore;
                        courage = ud.Courage;
                        ethics = ud.Ethics;
                        kindness = ud.Kindness;
                        practicality = ud.Practicality;
                        responsibility = ud.Responsibility;
                        risk = ud.Responsibility;
                        selfcontrol = ud.SelfControl;
                        silliness = ud.Silliness;
                        strategy = ud.Strategy;

                    }

                }
                

                if (isScoreSubmit)
                {
                    if (scr!=null && scr.Count == 10)
                    {
                        scr.RemoveAt(0);

                        courage.RemoveAt(0);
                        ethics.RemoveAt(0);
                        kindness.RemoveAt(0);
                        practicality.RemoveAt(0);
                        responsibility.RemoveAt(0);
                        risk.RemoveAt(0);
                        selfcontrol.RemoveAt(0);
                        silliness.RemoveAt(0);
                        strategy.RemoveAt(0);
                    }

                    //scr.Add(Convert.ToInt32(_slider.value));
                    scr.Add(1);

                    courage.Add(c1);
                    ethics.Add(e1);
                    kindness.Add(k1);
                    practicality.Add(p1);
                    responsibility.Add(r1);
                    risk.Add(r2);
                    selfcontrol.Add(s1);
                    silliness.Add(s2);
                    strategy.Add(s3);
                }
                    // playername = usernametext.text;//---page2
                user.userName = playername;

                user.localid = localid;
                user.userscore = scr;

                user.Courage = courage;
                user.Ethics = ethics;
                user.Kindness = kindness;
                user.Practicality = practicality;
                user.Responsibility = responsibility;
                user.Risk = risk;
                user.SelfControl = selfcontrol;
                user.Silliness = silliness;
                user.Strategy = strategy;

                RestClient.Put(url: databaseURL + "/" + localid + ".json", user);

                //if (isScoreSubmit)
                //{
                //    submittxt.text = "Score submitted";

                //    Invoke("stopmsg", 2f);
                //}
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
              updatescore();
              print("44444");
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
        //page1.SetActive(false);
        //page2.SetActive(false);
        //page3.SetActive(true);
        SceneManager.LoadScene(gameSceneId);
                                                                            //edit by Bhuvanesh
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

            if (userdata.Courage != null && userdata.Courage.Count > 0)
            {
                if (userdata.Courage[userdata.Courage.Count - 1] != 0)
                    lastTraits += (string.IsNullOrEmpty(lastTraits) ? "" : "\n") + string.Format("{0}: {1}", "Courage", userdata.Courage[userdata.Courage.Count - 1].ToString());
            }

            if (userdata.Ethics != null && userdata.Ethics.Count > 0)
            {
                if (userdata.Ethics[userdata.Ethics.Count - 1] != 0)
                    lastTraits += (string.IsNullOrEmpty(lastTraits) ? "" : "\n") + string.Format("{0}: {1}", "Ethics", userdata.Ethics[userdata.Ethics.Count - 1].ToString());
            }

            if (userdata.Kindness != null && userdata.Kindness.Count > 0)
            {
                if (userdata.Kindness[userdata.Kindness.Count - 1] != 0)
                    lastTraits += (string.IsNullOrEmpty(lastTraits) ? "" : "\n") + string.Format("{0}: {1}", "Kindness", userdata.Kindness[userdata.Kindness.Count - 1].ToString());
            }

            if (userdata.Practicality != null && userdata.Practicality.Count > 0)
            {
                if (userdata.Practicality[userdata.Practicality.Count - 1] != 0)
                    lastTraits += (string.IsNullOrEmpty(lastTraits) ? "" : "\n") + string.Format("{0}: {1}", "Practicality", userdata.Practicality[userdata.Practicality.Count - 1].ToString());
            }

            if (userdata.Responsibility != null && userdata.Responsibility.Count > 0)
            {
                if (userdata.Responsibility[userdata.Responsibility.Count - 1] != 0)
                    lastTraits += (string.IsNullOrEmpty(lastTraits) ? "" : "\n") + string.Format("{0}: {1}", "Responsibility", userdata.Responsibility[userdata.Responsibility.Count - 1].ToString());
            }

            if (userdata.Risk != null && userdata.Risk.Count > 0)
            {
                if (userdata.Risk[userdata.Risk.Count - 1] != 0)
                    lastTraits += (string.IsNullOrEmpty(lastTraits) ? "" : "\n") + string.Format("{0}: {1}", "Risk", userdata.Risk[userdata.Risk.Count - 1].ToString());
            }

            if (userdata.SelfControl != null && userdata.SelfControl.Count > 0)
            {
                if (userdata.SelfControl[userdata.SelfControl.Count - 1] != 0)
                    lastTraits += (string.IsNullOrEmpty(lastTraits) ? "" : "\n") + string.Format("{0}: {1}", "SelfControl", userdata.SelfControl[userdata.SelfControl.Count - 1].ToString());
            }

            if (userdata.Silliness != null && userdata.Silliness.Count > 0)
            {
                if (userdata.Silliness[userdata.Silliness.Count - 1] != 0)
                    lastTraits += (string.IsNullOrEmpty(lastTraits) ? "" : "\n") + string.Format("{0}: {1}", "Silliness", userdata.Silliness[userdata.Silliness.Count - 1].ToString());
            }

            if (userdata.Strategy != null && userdata.Strategy.Count > 0)
            {
                if (userdata.Strategy[userdata.Strategy.Count - 1] != 0)
                    lastTraits += (string.IsNullOrEmpty(lastTraits) ? "" : "\n") + string.Format("{0}: {1}", "Strategy", userdata.Strategy[userdata.Strategy.Count - 1].ToString());
            }

            Debug.Log(lastTraits);
            Invoke("stopmsg", 1f);
            Invoke("Shownextpage", 1f);
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

    //public void SignOutUser()
    //{
    //    RestClient.
    //}
}
//?auth=" + idtoken

   // ".write": "auth.uid !=null"