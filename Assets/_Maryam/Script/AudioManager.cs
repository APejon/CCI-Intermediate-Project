using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioClip titleTheme;
    public AudioClip gameTheme;
    public AudioClip buttonClickSound;
    public AudioSource sfxSource;

    public bool isMuted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        // Singleton pattern to keep the audio manager alive between scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayTitleTheme();
    }

    // Update is called once per frame

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
    // When entering gameplay scene
    void StartGame()
    {
        SceneManager.LoadScene(0);
        AudioManager.Instance.PlayGameTheme();
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

        public void MuteToggle(bool muted)
        {
            if (isMuted == false)
            {
                AudioListener.volume = 0;
                Debug.Log("mute");
                isMuted = true;
            }
            else if(isMuted == true)
            {
                AudioListener.volume = 1;
                Debug.Log("play");
                isMuted = false;
            }
        }
    
}
