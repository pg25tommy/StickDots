using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectLocalPlayerInfo : MonoBehaviour
{
    public int currentPlayerInfoIndex = 0;

    public TMP_InputField playerNameInputField;
    public GameObject colorPickerPanel;
    public TMP_Text promptText;

    private Color selectedColor = Color.white;

    public FlexibleColorPicker colorPicker;

    public Player[] players = GamePlayManager.Instance.players;
    public PlayerColor[] playerColor = GamePlayManager.Instance.playerColor;
    public int currentPlayerIndex = GamePlayManager.Instance.currentPlayerIndex;
    public int playerCount = GamePlayManager.Instance.PlayersCount;

    void ChangePlayerInfo()
    {
        if (currentPlayerInfoIndex < playerCount)
        {
            promptText.text = $"Player {currentPlayerInfoIndex + 1}: Type your player name";

            // Clear the name input field
            playerNameInputField.text = "";
            playerNameInputField.gameObject.SetActive(true);
            colorPickerPanel.SetActive(false);
            currentPlayerInfoIndex++;
        }
        else
        {
            // Start turn when all info has been entered

            // TODO: Change access modifier in GameplayeManager to gain access
            //GamePlayManager.Instance.StartTurn();
            //GamePlayManager.Instance._board = new Board(
            //    GamePlayManager.Instance.H, 
            //    GamePlayManager.Instance.W);
            GridGenerator.Instance.CreateBoard();
            LineController.Instance.CreateLineDrawing();
        }
    }

    public void OnNameEntered()
    {
        string playerName = playerNameInputField.text;
        playerNameInputField.gameObject.SetActive(false);
        promptText.text = "Choose your player color";
        colorPickerPanel.SetActive(true);
    }

    public void OnColorSelected()
    {
        selectedColor = colorPicker.newColor;
        selectedColor = colorPicker.color;

        players[currentPlayerInfoIndex - 1].playerName = playerNameInputField.text;
        players[currentPlayerInfoIndex - 1].GetComponentInChildren<TextMeshProUGUI>().text = playerNameInputField.text;

        players[currentPlayerInfoIndex - 1].myColor = selectedColor;
        players[currentPlayerInfoIndex - 1].GetComponentInChildren<Image>().color = selectedColor;
        playerColor[currentPlayerIndex].myColor = selectedColor;

        colorPickerPanel.SetActive(false);

        // Continue to colect the other player's info
        ChangePlayerInfo();
    }
}
