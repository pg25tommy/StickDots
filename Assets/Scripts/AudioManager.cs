using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioClip backgroundMusicClip;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;

        if (backgroundMusicClip != null)
        {
            PlayBackgroundMusic();
        }
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusicClip != null && !audioSource.isPlaying)
        {
            audioSource.clip = backgroundMusicClip;
            audioSource.Play();
        }
    }
}