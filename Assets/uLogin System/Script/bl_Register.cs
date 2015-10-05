using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Security;
using System.Collections;
using System.Text.RegularExpressions; // needed for Regex

public class bl_Register : MonoBehaviour {

    public string SecretKey = "123456";
    public string RegisterPHP_Url = "";
    [Space(5)]
    public InputField UserInput = null;
    public InputField PasswordInput = null;
    public InputField Re_PasswordInput = null;
    [Space(5)]
    public int MaxNameLenght = 15;

    protected string UserName = "";
    protected string Password = "";
    protected string Re_Password = "";
    private bool isRegister = false;

	// Use this for initialization
	void Start () {

        if (UserInput != null)
        {
            UserInput.characterLimit = MaxNameLenght;
        }
	}
	
	/// <summary>
	/// 
	/// </summary>
    void Update()
    {
        if (UserInput != null)
        {
            UserInput.text = Regex.Replace(UserInput.text, @"[^a-zA-Z0-9 ]", "");//not permit simbols
            UserName = UserInput.text;
        }
        if (PasswordInput != null)
        {
            Password = PasswordInput.text;
        }
        if (Re_PasswordInput != null)
        {
            Re_Password = Re_PasswordInput.text;
        }
    }

    /// <summary>
    /// Register function to check.
    /// </summary>
    public void Register()
    {
        if (isRegister)//if alredy connect
            return;

        if (UserName != string.Empty && Re_Password != string.Empty && Password != string.Empty)
        {
            if (Password == Re_Password)
            {
                StartCoroutine(RegisterProcess());
                bl_LoginManager.UpdateDescription("");
            }
            else
            {
                bl_LoginManager.UpdateDescription("Password does not match");
                Debug.LogWarning("Password does not match");
            }
        }
        else
        {
            if (UserName == string.Empty)
            {
                Debug.LogWarning("User Name is Emty");
            }
            if (Password == string.Empty)
            {
                Debug.LogWarning("Password is Emty");
            }
            if (Re_Password == string.Empty)
            {
                Debug.LogWarning("Re-Password is Emty");
            }
            bl_LoginManager.UpdateDescription("Complete all fields");
        }
    }

    /// <summary>
    /// Connect with database
    /// </summary>
    /// <returns></returns>
    IEnumerator RegisterProcess()
    {
        if (isRegister)
            yield return null;

        isRegister = true;
        bl_LoginManager.UpdateDescription("Registering...");
        bl_LoginManager.LoadingCache.ChangeText("Request Register...", true);
        //Used for security check for authorization to modify database
        string hash = Md5Sum(UserName + Password + SecretKey).ToLower();

        //Assigns the data we want to save
        //Where -> Form.AddField("name" = matching name of value in SQL database
        WWWForm mForm = new WWWForm();
        mForm.AddField("name", UserName); // adds the player name to the form
        mForm.AddField("password", Password); // adds the player password to the form
        mForm.AddField("kills", 0); // adds the kill total to the form
        mForm.AddField("deaths", 0); // adds the death Total to the form
        mForm.AddField("score", 0); // adds the score Total to the form
        mForm.AddField("uIP", bl_LoginManager.m_IP);
        mForm.AddField("hash", hash); // adds the security hash for Authorization

        //Creates instance of WWW to runs the PHP script to save data to mySQL database
        WWW www = new WWW(RegisterPHP_Url, mForm);
        bl_LoginManager.UpdateDescription("Processing...");
        Debug.Log("Processing...");
        yield return www;
        bl_LoginManager.LoadingCache.ChangeText("Get response...", true,2f);
        Debug.Log("" + www.text);
        if (www.text == "Done")
        {
            Debug.Log("Registered Successfully.");
            this.GetComponent<bl_LoginManager>().ShowLogin();
            bl_LoginManager.UpdateDescription("Registered Successfully, Login Now");

        }
        else
        {
            Debug.Log(www.text);
            bl_LoginManager.UpdateDescription(www.text);

        }
        isRegister = false;
    }
    
    /// <summary>
    /// Md5s Security Features
    /// </summary>
    public string Md5Sum(string input)
    {
        System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hash = md5.ComputeHash(inputBytes);

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++) { sb.Append(hash[i].ToString("X2")); }
        return sb.ToString();
    }
}
