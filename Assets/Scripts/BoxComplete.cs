using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

enum PlayerColour
{
    player_blue,
    player_red
}

public class BoxComplete : MonoBehaviour
{
    private List<GameObject> boxBackgrounds = new List<GameObject>();

    private Material activeColor;
    [SerializeField] private Shader completionShader;
    [SerializeField] private AudioClip audioClip;

    private int col;
    private int winPlayer;
    //[SerializeField] Player winPlayer;
    //[SerializeField] int col;
    //[SerializeField] int x, y;
    private bool blingMode;

    public List<GameObject> BoxBackgrounds
    {
        get { return boxBackgrounds; }
        set { boxBackgrounds = value; }
    }

    private void Start()
    {
        // Width in GameManager stores num of dots
        // For box need to -1
        col = GamePlayManager.Instance.W - 1;
        blingMode = false;
        ResetBling();
    }

    int getBoxHash(int x, int y)
    {
        // hash code for coord
        return x + (y * (col));
    }

    public void PlayCaptureBoxAnim(Vector3 boxCoordAndCapturedBy) 
    {
        int x = (int)boxCoordAndCapturedBy.x;
        int y = (int)boxCoordAndCapturedBy .y;
        winPlayer = (int)boxCoordAndCapturedBy.z;
        //get the specific gameobject render by hash code
        Renderer ren = boxBackgrounds[getBoxHash(x, y)].gameObject.GetComponent<Renderer>();

        ren.gameObject.SetActive(true);
        activeColor = new Material(completionShader);
        activeColor.SetColor("_baclgroundColor",
            GamePlayManager.Instance.players[winPlayer].myColor);
        ren.material = activeColor;
        blingMode = true;

        if (audioClip != null)
        {
            AudioSource.PlayClipAtPoint(audioClip, transform.position);
        }
    }

    private void ResetBling()
    {
        blingMode = false;
    }

    private void Update()
    {
        if (blingMode)
        {
            float offset = activeColor.GetFloat("_HighLightOffset");
            activeColor.SetFloat("_HighLightOffset", offset + Time.deltaTime/3.0f);
            if (offset >= 1.0f) ResetBling();
        }

        
    }
}