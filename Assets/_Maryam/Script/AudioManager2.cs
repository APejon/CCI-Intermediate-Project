using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static AudioManager2 Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;           // Added for sound effects like button clicks
    public AudioClip titleTheme;
    public AudioClip gameTheme;
    public AudioClip buttonClickSound;      // Added for button sound

    [Header("UI")]
    public Toggle muteToggle;

    void Awake()
    {
        // Singleton pattern to keep the audio manager alive between scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        PlayTitleTheme();
    }

    public void PlayTitleTheme()
    {
        musicSource.clip = titleTheme;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayGameTheme()
    {
        musicSource.clip = gameTheme;
        musicSource.loop = true;
        musicSource.Play();
    }

    // New method to play button click sound
    public void PlayButtonClick()
    {
        if (buttonClickSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(buttonClickSound);
        }
    }

    // Call this from your button instead of StartGame()
    public void StartGameWithSound()
    {
        PlayButtonClick();
        StartCoroutine(DelayedStartGame());
    }

    private System.Collections.IEnumerator DelayedStartGame()
    {
        yield return new WaitForSeconds(0.2f); // wait to let the click sound play
       SceneManager.LoadScene(0);
        PlayGameTheme();
    }
    public void ToggleMuteFromUI(Toggle toggle)
    {
        bool isMuted = toggle.isOn;           // "Is checkbox checked?"
        musicSource.mute = isMuted;           // Mute if checked
        PlayerPrefs.SetInt("MusicMuted", isMuted ? 1 : 0);
    }
}