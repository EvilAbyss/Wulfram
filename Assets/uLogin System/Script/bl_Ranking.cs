//////////////////////////////////////////////////////////////////////////////
// bl_Ranking.cs
//
//
//                       Lovatto Studio 2015
//////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class bl_Ranking : MonoBehaviour {

    public string GetTopPHP_Url = ""; //be sure to add a ? to your url
    public string Admin = ""; //be sure to add a ? to your url
    [Space(5)]
    public GameObject UserTopPrefab;
    public Transform TableRanking;
    public List<bl_TopUserInfo> Top = new List<bl_TopUserInfo>();
    public InputField SearchField;
    public Text InfoText = null;
    [Space(5)]
    public Color colorA;
    public Color colorD;

    private bool isget = false;
    private int LastFount = 0;
    /// <summary>
    /// get top
    /// </summary>
    void Start()
    {
        StartCoroutine(GetTop());
    }

    // Get the scores from the MySQL DB.
    // remember to use StartCoroutine when calling this function!
    IEnumerator GetTop()
    {
        if (isget)//if alredy load info
            yield return null;

        isget = true;//notify is load now
        StopCoroutine("CleanText");
        if (InfoText.text == string.Empty)
        {
            InfoText.text = "Get List From Data Base...";
        }
        bl_LoadingEffect.Loading = true;

        WWW www = new WWW(GetTopPHP_Url);
        yield return www;//wait for response

        bl_LoadingEffect.Loading = false;
        if (www.error != null)
        {
            print("There was an error getting the high score: " + www.error);
            InfoText.text = www.error;
        }
        else
        {
            Top.Clear();//Clear list cache

            if (TableRanking.childCount > 0)//if exist children
            {
                foreach (Transform t in TableRanking.GetComponent<Transform>())//destroy
                {
                    Destroy(t.gameObject);
                }
            }
            //get info from data base
            string result = www.text;
            //get and separate users
            string[] splituser = result.Split("\n"[0]);

            int numbers = 0;
            bool mtype = false;

            foreach (string u in splituser)
            {
                numbers++;
                mtype = !mtype;
                //get separate info user
                string[] splitinfo = u.Split("|"[0]);

                if (u != string.Empty && u != null)
                {
                    //instantiate UI of User
                    GameObject go = Instantiate(UserTopPrefab) as GameObject;
                    //list effect
                    if (mtype)
                    {
                        go.GetComponent<Image>().color = colorA;
                    }
                    else
                    {
                        go.GetComponent<Image>().color = colorD;
                    }

                    bl_TopUserInfo user = go.GetComponent<bl_TopUserInfo>();
                    go.name = splitinfo[0];

                    user.rank = numbers + "";
                    user.mname = splitinfo[0];
                    user.kills = int.Parse(splitinfo[1]);
                    user.deaths = int.Parse(splitinfo[2]);
                    user.score = int.Parse(splitinfo[3]);
                    int st = int.Parse(splitinfo[4]);
                    user.status = st;
                    //detect if name equal to admin in inspector
                    if (splitinfo[0] == Admin || st == 2)
                    {
                        user.isAdmin();
                    }
                    if (st == 1)
                    {
                        user.isBanned();
                    }
                    else if (st == 3)
                    {
                        user.isModerator();
                    }
                    
                    //send info
                    user.CreateUser();
                    //put in table
                    go.transform.SetParent(TableRanking,false);
                    //store in cache
                    Top.Add(user);//cache top's
                }
            }

            InfoText.text = "Loaded Successfully";
            StartCoroutine(CleanText());
        }
        
        isget = false;
    }

    /// <summary>
    /// get agai top list
    /// </summary>
    public void Refresh()
    {
        if (isget)
            return;
        //get list again
        StartCoroutine(GetTop());
    }

    /// <summary>
    /// search a player in ranking
    /// </summary>
    /// <param name="p"></param>
    public void Search()
    {
        string t = SearchField.text;//get text from inputfield
        bool isFount = false;

        //search in list
        for (int i = 0; i < Top.Count; i++)
        {
            //if name no equal who we seek
            if (Top[i].mname != t && Top[i] != null)
            {
                Destroy(Top[i].gameObject);
            }
            else if (Top[i].mname == t)
            {
                isFount = true;
                LastFount = i;
            }
            else
            {
                StopCoroutine("CleanText");
                InfoText.text = "There is no user with this name <color=orange>" + t + "</color>,Refresh List for found again";
                StartCoroutine(CleanText());
            }
        }
        if (isFount)//if exist player
        {
            StopCoroutine("CleanText");
            InfoText.text = "User <color=orange>" + t + "</color> is <color=green>" + Top[LastFount].rank +"</color> in Ranking";
            StartCoroutine(CleanText());
        }
        else
        {
            StopCoroutine("CleanText");
            InfoText.text = "User <color=orange>" + t + "</color>does not exist in the ranking list";
            StartCoroutine(CleanText());
            Refresh();//if not found, then refresh list
            
        }
        SearchField.text = string.Empty;
        
    }

    IEnumerator CleanText()
    {

        yield return new WaitForSeconds(7);
        InfoText.text = string.Empty;
    }
}