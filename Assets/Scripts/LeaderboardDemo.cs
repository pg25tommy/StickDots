using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;


public class LeaderboardDemo : MonoBehaviour {
    public string leaderboardId = "stickdots-leaderboard";
    public TextMeshProUGUI messageText;
    public LeaderboardScoreView scoreViewPrefab;
    public TMP_InputField scoreInputField;
    public Button submitScoreButton;
    public Button loadScoresButton;
    public Transform scoresContainer;

    private async void Awake() {
        await UnityServices.InitializeAsync();
    }

    private async void Start() {
        AuthenticationService.Instance.SignedIn += OnSignedIn;
        AuthenticationService.Instance.SignInFailed += OnSignInFailed;

        messageText.text = "Signing in...";
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        submitScoreButton.onClick.AddListener(SubmitScoreAsync);
        loadScoresButton.onClick.AddListener(LoadScoresAsync);
    }

    private void OnSignedIn() {
        messageText.text = $"Signed in as: {AuthenticationService.Instance.PlayerId}";
    }

    private void OnSignInFailed(RequestFailedException exception) {
        messageText.text = $"Sign in failed with exception: {exception}";
    }

    private async void SubmitScoreAsync() {
        if (string.IsNullOrEmpty(scoreInputField.text)) {
            return;
        }

        var score = Convert.ToDouble(scoreInputField.text);
        scoreInputField.text = string.Empty;
        try {
            var scoreeee = await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);
            messageText.text = "Score submitted!";
        }
        catch (Exception e) {
            messageText.text = $"Failed to submit score: {e}";
            throw;
        }
    }

    private async void LoadScoresAsync() {
        try {
            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId);
            var childCount = scoresContainer.childCount;
            for (int i = 0; i < childCount; i++) {
                Destroy(scoresContainer.GetChild(i).gameObject);
            }

            foreach (var leaderboardEntry in scoresResponse.Results) {
                var scoreView = Instantiate(scoreViewPrefab, scoresContainer);
                scoreView.Initialize(leaderboardEntry.Rank.ToString(), leaderboardEntry.PlayerName,
                    leaderboardEntry.Score.ToString());
            }

            messageText.text = "Scores fetched!";
        }
        catch (Exception e) {
            messageText.text = $"Failed to fetch scores: {e}";
            throw;
        }
    }

    private void OnDestroy() {
        AuthenticationService.Instance.SignedIn -= OnSignedIn;
        AuthenticationService.Instance.SignInFailed -= OnSignInFailed;
    }
}