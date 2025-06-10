using UnityEngine;
using UnityEngine.SceneManagement;
public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioClip titleTheme;
    public AudioClip gameTheme;

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
}