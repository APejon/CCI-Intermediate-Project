using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{

    public static UiManager Instance { get; private set; }

   // [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameUIPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject gameCredits;

    //  [Header("Buttons")]
    public Button startButton;
    public Button restartButton;
    public Button quitButton;
    public Button openWebButton;
    public Button muteButton;

    private bool isMuted = false;

    public string url = "https://itch.io/profile/jalboot"; // Replace with your desired URL

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Button listeners
        if (startButton != null) startButton.onClick.AddListener(() => SceneLoader.Instance.LoadGameScene());
        if (restartButton != null) restartButton.onClick.AddListener(() => SceneLoader.Instance.RestartScene());
        if (quitButton != null) quitButton.onClick.AddListener(() => Application.Quit());
        if (openWebButton != null) openWebButton.onClick.AddListener(OpenWeb); // Hook URL button
        if (muteButton != null) muteButton.onClick.AddListener(ToggleMute);
        ShowMainMenu();
    }
    public void OpenWeb()
    {
        Application.OpenURL(url);
    }
    // === UI Control Methods ===
    public void ShowMainMenu()
    {
        SetActivePanel(mainMenuPanel);
    }

    public void ShowGameUI()
    {
        SetActivePanel(gameUIPanel);
    }

    public void ShowPauseMenu()
    {
        SetActivePanel(pausePanel);
        muteButton?.gameObject.SetActive(true);
    }
    public void ShowGameCredits()
    {
        SetActivePanel(gameCredits);
    }
    public void ShowGameOver()
    {
        SetActivePanel(gameOverPanel);
    }

    private void SetActivePanel(GameObject targetPanel)
    {
        // Deactivate all
        mainMenuPanel?.SetActive(false);
        gameUIPanel?.SetActive(false);
        pausePanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        gameCredits?.SetActive(false);
        muteButton?.gameObject.SetActive(false);
        // Activate target
        targetPanel?.SetActive(true);
    }
}