using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiController : MonoBehaviour
{
    [SerializeField] private GameObject emojiPrefab;
    [SerializeField] private string emojiAnimation;
    [SerializeField] private Transform spawnTransform;

    private void Awake()
    {
        spawnTransform = GameObject.FindGameObjectWithTag("EmojiSpawn").transform;
    }

    public void SpawnEmoji()
    {
        // Instatiates an emoji at the player's emoji transform
        GameObject emojiInstance = Instantiate(emojiPrefab, spawnTransform);
        emojiInstance.GetComponentInChildren<Animator>().Play(emojiAnimation);
        Destroy(emojiInstance, 1.2f);
    }
}
