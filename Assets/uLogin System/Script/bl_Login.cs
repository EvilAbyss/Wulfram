using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class bl_Login : MonoBehaviour {

    public string LoginPHP_URL = "";
    public string NextLevel = "";
    [Space(5)]
    public string m_User = "";
    public string m_Password = "";
    public bool KeepMe = false;
    [Space(5)]
    public Toggle mToggle = null;
    public InputField m_UserInput = null;
    public InputField m_PassInput = null;
    //Private
    private bool isLogin = false;
    private bool AlredyLogin = false;

    /// <summary>
    /// Get Name in Init
    /// </summary>
    void Awake()
    {
        if (mToggle != null)
        {
            if (PlayerPrefs.GetString(bl_LoginManager.SavedUser) != string.Empty)
            {
                mToggle.isOn = true;
                if (m_User != null)
                {
                    m_UserInput.text = PlayerPrefs.GetString(bl_LoginManager.SavedUser);
                }
            }
            else
            {
                mToggle.isOn = false;
            }
        }
    }

	/// <summary>
	/// 
	/// </summary>
    void FixedUpdate()
    {
        if (mToggle != null)
        {
            KeepMe = mToggle.isOn;
        }
        if (m_UserInput != null)
        {
            m_User = m_UserInput.text;
        }
        if (m_PassInput != null)
        {
            m_Password = m_PassInput.text;
        }
    }

    /// <summary>
    /// Login detect if user and password is not emty
    /// and startcorrutine for  connection with DB
    /// </summary>
    public void Login()
    {
        if (isLogin || AlredyLogin)
            return;

        if (m_User != string.Empty && m_Password != string.Empty)
        {
            StartCoroutine(LoginProcess());
        }
        else
        {
            if (m_User == string.Empty)
            {
                Debug.LogWarning("User Name is Emty");
            }
            if (m_Password == string.Empty)
            {
                Debug.LogWarning("Password is Emty");
            }
            bl_LoginManager.UpdateDescription("complete all fields");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator LoginProcess()
    {
        if (isLogin || AlredyLogin)
            yield return null;

        isLogin = true;
        bl_LoginManager.UpdateDescription("Login...");

        bl_LoginManager.LoadingCache.ChangeText("Request Login...", true);
        // Create instance of WWWForm
        WWWForm mForm = new WWWForm();
        //sets the mySQL query to the amount of rows to load
        mForm.AddField("name", m_User);
        mForm.AddField("password", m_Password);
        //Creates instance to run the php script to access the mySQL database
        WWW www = new WWW(LoginPHP_URL, mForm);
        //Wait for server response...
        yield return www;
        bl_LoginManager.LoadingCache.ChangeText("Getting response from server...", true,1.2f);
        string result = www.text;
        //Separate information
        string[] mSplit = result.Split("|"[0]);

        if (mSplit[0] == "Login Done")
        {
            Debug.Log("Login " + mSplit[1]);
            bl_LoginManager.LoadingCache.ChangeText("Successfully logged.", true, 0.8f);
            AlredyLogin = true;
            //get vals
            string t_name = mSplit[1];
            int kills = int.Parse(mSplit[2]);
            int deaths = int.Parse(mSplit[3]);
            int score = int.Parse(mSplit[4]);
            int status = int.Parse(mSplit[5]);

            //Get status of player
            switch (status)
            {
                case 2:
                    bl_SaveInfo.isAdmin = true;
                    break;
                case 3:
                    bl_SaveInfo.isModerator = true;
                    break;
            }

            bl_LoginManager.UpdateDescription("Welcome "+RichColor(mSplit[1],"orange")+"\n Kills: <color=orange>" + mSplit[2]+"</color> \n Deaths: <color=orange>"+
                mSplit[3] + "</color> \n Score: <color=orange>" + mSplit[4] + "</color>");
            if (KeepMe)
            {
                PlayerPrefs.SetString(bl_LoginManager.SavedUser, m_User);
            }
            else
            {
                PlayerPrefs.SetString(bl_LoginManager.SavedUser, string.Empty);
            }
            //Send information to SaveInfo.
            bl_LoginManager.OnLoginEvent(t_name, kills, deaths, score, status);
            yield return new WaitForSeconds(1f);
            //Load next level
            //If you want detect banned players.
            if (this.GetComponent<bl_LoginManager>().DetectBan)
            {
                //if already check is banned or not.
                if (bl_BanDetect.isChecked)
                {
                    //if not banned can load next level
                    if (!bl_BanDetect.isBanned)
                    {
                        //last verification, see if this account is not in the BanList.
                        if (!this.GetComponent<bl_BanDetect>().isBanAccount(t_name))
                        {
                            //Pass all verification, can load next level.
                            Application.LoadLevel(NextLevel);
                        }                      
                    }
                }
                else//if not check if is banned
                {
                    //then, wait for check for load next level.
                    this.GetComponent<bl_BanDetect>().LoadWhenCheck = true;
                }
            }
            else
            {
                Application.LoadLevel(NextLevel);
            }
           
        }
        else//Wait, have a error?, please contact me for help with the result of next debug logwarning.
        {
            //Some error with the server.
            Debug.LogWarning(www.text);
            bl_LoginManager.UpdateDescription(www.text);
        }
        isLogin = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    private string RichColor(string text, string color)
    {

        return "<color=" + color + ">" + text + "</color>";
    }
}
