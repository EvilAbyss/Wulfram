//////////////////////////////////////////////////////////////////////////////
// bl_TopUserInfo.cs
//
//
//                       Lovatto Studio 2015
//////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class bl_TopUserInfo : MonoBehaviour
{
    public string rank = "";
    public string mname = "";
    public int kills = 0;
    public int deaths = 0;
    public int score = 0;
    public int status = 0;
    [Space(5)]
    public Text m_number = null;
    public Text m_name = null;
    public Text m_kills = null;
    public Text m_deaths = null;
    public Text m_score = null;
    [Space(5)]
    public Color AdminColor = new Color(0.9f, 0f, 0.1f, 0.9f);
    public Color ModeratorColor = new Color(0f, 0.1f, 0.9f, 0.9f);
   
    /// <summary>
    /// get info and update Text
    /// </summary>
    public void CreateUser()
    {
        m_number.text = rank.ToString(); 
        m_name.text = mname;
        m_kills.text = kills.ToString();
        m_deaths.text = deaths.ToString();
        m_score.text = score.ToString(); 
    }
    /// <summary>
    /// when is admin 
    /// </summary>
    public void isAdmin()
    {
        mname += " <color=red>[Admin]</color>";
        m_number.color = AdminColor;
        m_name.color = AdminColor;
        m_kills.color = AdminColor;
        m_deaths.color = AdminColor;
        m_score.color = AdminColor;
    }
    /// <summary>
    /// 
    /// </summary>
    public void isBanned()
    {
        mname += " <color=red>[Banned]</color>";
        Debug.Log("Detect a Ban");
    }

    public void isModerator()
    {
        mname += " <color=#4E8DE6>[Moderator]</color>";
        m_number.color = ModeratorColor;
        m_name.color = ModeratorColor;
        m_kills.color = ModeratorColor;
        m_deaths.color = ModeratorColor;
        m_score.color = ModeratorColor;
    }

}