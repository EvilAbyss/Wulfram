using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class bl_BanDetect : MonoBehaviour {

    [HideInInspector]public bool LoadWhenCheck = false;

	public string BanListURL = "";
    public string NextLevel = "";
    public List<BanUserInfo> BanList = new List<BanUserInfo>();
    [Space(5)]
    public List<GameObject> DesactiveObjects = new List<GameObject>();
    public Text BanInfoText = null;
    public GameObject BanUI;

    public static string myIP = "";
    public static bool isChecked = false;
    public static bool isBanned = false;

    private bool ReceiveBanList = false;
    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        StartCoroutine(GetBanList());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator GetBanList()
    {
        WWW www = new WWW(BanListURL);

        yield return www;

        if (www.error != null)
        {
            print("There was an error getting the ban list: " + www.error);
        }
        else
        {
            string text = www.text;
            if (text != "Emty")
            {
                string[] splitusers = text.Split("\n"[0]);

                foreach (string t in splitusers)
                {
                    if (t != string.Empty && t != null)
                    {
                        string[] splitinfo = t.Split("|"[0]);
                        BanUserInfo bui = new BanUserInfo();
                        bui.m_Name = splitinfo[0];
                        bui.m_reason = splitinfo[1];
                        bui.m_ip = splitinfo[2];
                        bui.m_by = splitinfo[3];
                        BanList.Add(bui);
                    }
                }
            }
            else
            {
                Debug.Log("Ban List is Emty");
            }
        }
        yield return new WaitForSeconds(0.2f);
        ReceiveBanList = true;
        StartCoroutine(CheckIsReady());
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckIsReady()
    {
        yield return new WaitForSeconds(0.2f);
        if (ReceiveBanList)
        {
            DetectIsBan();
        }
        else
        {
            StartCoroutine(CheckIsReady());
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void DetectIsBan()
    {
        for (int i = 0; i < BanList.Count; i++)
        {
            if (BanList[i].m_ip == bl_LoginManager.m_IP )
            {
                isBanned = true;
                isBan(BanList[i].m_Name, BanList[i].m_reason,0);
            }
        }
        isChecked = true;
        if (LoadWhenCheck && !isBanned) { Application.LoadLevel(this.GetComponent<bl_Login>().NextLevel); }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="n"></param>
    /// <param name="r"></param>
    void isBan(string n,string r,int _type)
    {
        foreach (GameObject g in DesactiveObjects)
        {
            g.SetActive(false);
        }
        if (_type == 0)
        {
            BanInfoText.text = "This IP has been banned due to <color=#F5A822>" + r + "</color> if you think this is a misunderstanding, contact the admin or some moderator of the game.";
        }
        else if(_type == 1)
        {
            BanInfoText.text = "Account " + n + " has been banned due to <color=#F5A822>" + r + "</color> if you think this is a misunderstanding, contact the admin or some moderator of the game.";
        }

        BanUI.SetActive(true);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="acc"></param>
    /// <returns></returns>
    public bool isBanAccount(string acc)
    {
        bool b = false;
        for (int i = 0; i < BanList.Count; i++)
        {
            if (BanList[i].m_Name == acc)
            {
                b = true;
                isBan(BanList[i].m_Name, BanList[i].m_reason,1);
            }
        }
        return b;
    }

    [System.Serializable]
    public class BanUserInfo
    {
        public string m_Name = "";
        public string m_reason = "";
        public string m_ip = "";
        public string m_by = "None";
    }
}