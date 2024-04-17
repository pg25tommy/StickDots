using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    // Create a leaderboard with this ID in the Unity Cloud Dashboard
    const string LeaderboardId = "stickdots-leaderboard";
    public static LeaderboardManager Instance { get; private set; }
    string VersionId { get; set; }
    int Offset { get; set; }
    int Limit { get; set; }
    int RangeLimit { get; set; }
    List<string> FriendIds { get; set; }

    public LeaderboardScoreView scoreViewPrefab;
    public Transform scoresContainer;
    public TMP_InputField scoreEntered;

    async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        await UnityServices.InitializeAsync();

        await SignInAnonymously();
    }

    async Task SignInAnonymously()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            // Take some action here...
            Debug.Log(s);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void AddScore()
    {
        //string score = scoreEntered.text;
        int scoreInt = 0;
        Int32.TryParse(scoreEntered.text, out scoreInt);
        var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(
            LeaderboardId, scoreInt);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
    }

    public async void AddScoreWithMetadata()
    {
        int scoreInt = 0;
        Int32.TryParse(scoreEntered.text, out scoreInt);
        var metadata = new Dictionary<string, string>() { { "team", "red" } };
        var playerEntry = await LeaderboardsService.Instance
            .AddPlayerScoreAsync(
                LeaderboardId,
                scoreInt,
                new AddPlayerScoreOptions { Metadata = metadata });
        Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }

    public async void GetScores()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }

    public async void LoadScoresAsync()
    {
        try
        {
            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId);
            var childCount = scoresContainer.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Destroy(scoresContainer.GetChild(i).gameObject);
            }

            foreach (var leaderboardEntry in scoresResponse.Results)
            {
                var scoreView = Instantiate(scoreViewPrefab, scoresContainer);
                scoreView.Initialize(leaderboardEntry.Rank.ToString(), leaderboardEntry.PlayerName,
                    leaderboardEntry.Score.ToString());
            }

            //messageText.text = "Scores fetched!";
        }
        catch (Exception e)
        {
            //messageText.text = $"Failed to fetch scores: {e}";
            throw;
        }
    }

    public async void GetPaginatedScores()
    {
        Offset = 10;
        Limit = 10;
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions { Offset = Offset, Limit = Limit });
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }

    public async void GetPlayerScore()
    {
        var scoreResponse =
            await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
    }

    public async void GetPlayerRange()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetPlayerRangeAsync(LeaderboardId, new GetPlayerRangeOptions { RangeLimit = RangeLimit });
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }

    public async void GetScoresByPlayerIds()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresByPlayerIdsAsync(LeaderboardId, FriendIds);
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }

    // If the Leaderboard has been reset and the existing scores were archived,
    // this call will return the list of archived versions available to read from,
    // in reverse chronological order (so e.g. the first entry is the archived version
    // containing the most recent scores)
    public async void GetVersions()
    {
        var versionResponse =
            await LeaderboardsService.Instance.GetVersionsAsync(LeaderboardId);

        // As an example, get the ID of the most recently archived Leaderboard version
        VersionId = versionResponse.Results[0].Id;
        Debug.Log(JsonConvert.SerializeObject(versionResponse));
    }

    public async void GetVersionScores()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetVersionScoresAsync(LeaderboardId, VersionId);
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }

    public async void GetPaginatedVersionScores()
    {
        Offset = 10;
        Limit = 10;
        var scoresResponse =
            await LeaderboardsService.Instance.GetVersionScoresAsync(LeaderboardId, VersionId, new GetVersionScoresOptions { Offset = Offset, Limit = Limit });
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }

    public async void GetPlayerVersionScore()
    {
        var scoreResponse =
            await LeaderboardsService.Instance.GetVersionPlayerScoreAsync(LeaderboardId, VersionId);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
    }
}