using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotTurn : MonoBehaviour
{
    private void Start()
    {
        
    }
    private void OnMouseDown()
    {
        int playerIndex = GamePlayManager.Instance.currentPlayerIndex;
        Color spriteColor = gameObject.GetComponent<SpriteRenderer>().color;

        spriteColor = GamePlayManager.Instance.players[playerIndex].myColor;
        Color color = spriteColor;
        color.a = 1f;
        gameObject.GetComponent<SpriteRenderer>().color = color;
        //gameObject.GetComponent<Renderer>().material.color = GamePlayManager.Instance.players[playerIndex].myColor;

        GamePlayManager.Instance.EndTurn();
    }
}
