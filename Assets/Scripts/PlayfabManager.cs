using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class PlayfabManager : MonoBehaviour
{
    public bool isLoggedIn = false;
    public bool hasDisplayName = false;
    public bool sentScore = false;
    public bool displayedScore = false;

    private Text[] names;
    private Text[] scores;
    
    public void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
    }

    void OnSuccess(LoginResult result)
    {
        Debug.Log("Successful login/account creation");
        isLoggedIn = true;
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Error");
        Debug.Log(error.GenerateErrorReport());
    }

    public void SendLeaderboard(int score, int level)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Level " + level.ToString() + " Score",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }

    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successfully sent to leaderboard");
        sentScore = true;
    }

    public void GetLeaderboard(int level, Text[] names, Text[] scores)
    {
        this.names = names;
        this.scores = scores;
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Level " + level + " Score",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        Debug.Log("Successfully got leaderboard");
        for (int i = 0; i < result.Leaderboard.Count; ++i)
        {
            if (result.Leaderboard[i].StatValue > 0)
            {
                names[i].text = result.Leaderboard[i].DisplayName;
                scores[i].text = result.Leaderboard[i].StatValue.ToString();
            }
            else
            {
                names[i].text = "";
                scores[i].text = "";
            }
        }
        for (int i = result.Leaderboard.Count; i < 10; ++i)
        {
            names[i].text = "";
            scores[i].text = "";
        }
        displayedScore = true;
    }

    public void UpdateDisplayName(string displayName)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdateDisplayName, OnError);
    }

    void OnUpdateDisplayName(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Successfully updated display name");
        hasDisplayName = true;
    }
}
