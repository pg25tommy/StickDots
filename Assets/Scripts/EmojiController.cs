using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiController : MonoBehaviour
{
    [SerializeField] private GameObject emojiPrefab;
    [SerializeField] private string emojiAnimation;

    public void SpawnEmoji()
    {
        GameObject emojiInstance = Instantiate(emojiPrefab, transform);
        emojiInstance.GetComponent<Animator>().Play(emojiAnimation);
        Destroy(emojiInstance, 1.2f);
    }
}
