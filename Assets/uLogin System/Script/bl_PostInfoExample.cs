using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class bl_PostInfoExample : MonoBehaviour {

    public bl_SaveInfo SaveInfo = null;
    public Text PlayerInfo = null;
    public InputField KillsInput = null;
    public InputField DeathsInput = null;
    public InputField ScoreInput = null;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        if (GameObject.Find("PlayerInfo") != null)
        {
            SaveInfo = GameObject.Find("PlayerInfo").GetComponent<bl_SaveInfo>();
            PlayerInfo.text = "User: "+SaveInfo.m_UserName+ "\n Kills: "+SaveInfo.m_Kills+"\n Deaths: "+SaveInfo.m_Deaths+"\n Score: "+SaveInfo.Score;
        }
        else
        {
            Debug.LogWarning("Please Login Before open this scene");
            PlayerInfo.text = "Please Login";
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public void PostInfo()
    {
        if (SaveInfo != null)
        {
            //send info for save in data base
            SaveInfo.SaveInfo(KillsInput.text, DeathsInput.text, ScoreInput.text);
            StartCoroutine(RefreshWait());
            KillsInput.text = "";
            DeathsInput.text = "";
            ScoreInput.text = "";
        }
        else
        {
            Debug.LogWarning("Please Login Before open this scene");
            PlayerInfo.text = "Please Login";
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void Refresh()
    {
        PlayerInfo.text = "User: " + SaveInfo.m_UserName + "\n Kills: " + SaveInfo.m_Kills + "\n Deaths: " + SaveInfo.m_Deaths + "\n Score: " + SaveInfo.Score;
    }

    IEnumerator RefreshWait()
    {
        yield return new WaitForSeconds(5);
        Refresh();
    }

}
