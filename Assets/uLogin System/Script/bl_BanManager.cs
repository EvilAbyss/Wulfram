using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.Security;

public class bl_BanManager : MonoBehaviour
{

    public string BanURL = "";
    public string RequestURL = "";
    public string SecretKey = "123456";
    [Space(5)]
    [Header("User Info")]
    public string m_UserName = "";
    public string m_Ip = "";
    public string m_Reason = "";
    public string m_BanBy = "";
    [Space(5)]
    [Header("UI")]
    public Text UserInfoText = null;
    public GameObject BanButton = null;

    private bl_SaveInfo m_Info = null;
    private bool RequestBan = false;
    private bool isRequestUser = false;

    void Awake()
    {
        if (GameObject.Find(bl_SaveInfo.SaveInfoName) != null)
        {
            m_Info = GameObject.Find(bl_SaveInfo.SaveInfoName).GetComponent<bl_SaveInfo>();
        }
        BanButton.SetActive(false);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="i"></param>
    public void RequestUser(InputField i)
    {
        if (isRequestUser)
            return;

        string n = i.text;
        i.text = "";
        StartCoroutine(Request(n,0));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="i"></param>
    public void SearchUser(InputField i)
    {
        if (isRequestUser)
            return;

        string n = i.text;
        i.text = "";
        StartCoroutine(Request(n, 1));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    IEnumerator Request(string user,int type)
    {
        isRequestUser = true;
        WWWForm form = new WWWForm();
        form.AddField("name", user);
        form.AddField("type", type);

        WWW w = new WWW(RequestURL, form);

        yield return w;

        string t = w.text;
        string[] split = t.Split("|"[0]);
        if (type == 0)
        {
            if (split[0] == "Exist")
            {
                m_UserName = split[1];
                m_Reason = split[2];
                m_Ip = split[3];
                m_BanBy = split[4];

                UserInfoText.text = "<color=#FF9B30>User:</color> " + m_UserName + "\n <color=#FF9B30>Ban Reason:</color> " + m_Reason + "\n <color=#FF9B30>User IP:</color> " + m_Ip + "\n <color=#FF9B30>Banned By:</color> " + m_BanBy;
                BanButton.SetActive(false);
            }
            else
            {
                Debug.Log(w.text);
                UserInfoText.text = w.text;
                BanButton.SetActive(false);
            }
        }
        else if (type == 1)
        {
            if (split[0] == "Exist")
            {
                m_UserName = split[1];
                m_Ip = split[3];

                UserInfoText.text = "<color=#FF9B30>User:</color> " + m_UserName + "\n <color=#FF9B30>Status:</color> " + split[2] + "\n <color=#FF9B30>User IP:</color> " + split[3] + "\n <color=#FF9B30>Score:</color> " + split[4];
                if (split[3] != string.Empty && !string.IsNullOrEmpty(split[3]))
                {
                    BanButton.SetActive(true);
                }
                else
                {
                    UserInfoText.text += "\n You can not ban this user, since did not register any IP.";
                }
            }
            else
            {
                Debug.Log(w.text);
                UserInfoText.text = w.text;
                BanButton.SetActive(false);
            }
        }
        isRequestUser = false;
    }
    /// <summary>
    /// 
    /// </summary>
    public void BanPlayer(InputField i)
    {
        if (RequestBan)
            return;
        string t = i.text;
        if (string.IsNullOrEmpty(t))
        {
            UserInfoText.text += "\nReason cant be empty!";
            return;
        }
        
        StartCoroutine(Ban(m_UserName, t, m_Ip));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator Ban(string n, string reason,string ip)
    {
            RequestBan = true;
            if (m_Info == null)
            {
                Debug.Log("Need Login for can ban");
                yield return null;
            }

            string mName = m_Info.m_UserName;
            //Used for security check for authorization to modify database
            string hash = Md5Sum(mName + SecretKey).ToLower();

            WWWForm form = new WWWForm();
            form.AddField("name", mName);
            form.AddField("reason", reason);
            form.AddField("myIP", ip);
            form.AddField("mBy", mName);
            form.AddField("hash", hash);

            WWW www = new WWW(BanURL, form);

            yield return www;

            if (www.text == "Done")
            {
                UserInfoText.text = "Player: " + n + " is Banned";
                BanButton.SetActive(false);
                Debug.Log("Player Banned");
            }
            else
            {
                Debug.Log(www.text);
            }
            RequestBan = false;
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