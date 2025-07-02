using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UiManager : MonoBehaviour
{

    public static UiManager Instance { get; private set; }

    // [Header("Panels")
    public GameObject mainMenuPanel;
    public GameObject gamePlayUI;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject gameCredits;
    public GameObject fadePanel;

    //  [Header("Buttons")]
    public Button startButton;
    public Button restartButton;
    public Button quitButton;
    public Button openWebButton;

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
        if (startButton != null) startButton.onClick.AddListener(OnStartClicked);
        if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);
        if (quitButton != null) quitButton.onClick.AddListener(() => Application.Quit());
        if (openWebButton != null) openWebButton.onClick.AddListener(OpenWeb); // Hook URL button
        fadePanel.SetActive(true);
        SetActivePanel(mainMenuPanel);
    }
    public void OpenWeb()
    {
        Application.OpenURL(url);
    }
    // === UI Control Methods ===
    public void ShowMainMenu()
    {
        
        //ScreenFader.Instance.FadeOut(mainMenuPanel, 1f);
        FadeInFadeOut.Instance.FadeAndDo(() => SetActivePanel(mainMenuPanel));
    }

    public void ShowGameUI()
    {
        //mainMenuPanel?.SetActive(false);
        //  FadeInFadeOut.Instance.FadeAndDo(() => SetActivePanel(gamePlayUI));
        gamePlayUI.gameObject.SetActive(true);
    }

    public void ShowPauseMenu()
    {
        FadeInFadeOut.Instance.FadeAndDo(() => SetActivePanel(pausePanel));
    }
    public void ShowGameCredits()
    {
        FadeInFadeOut.Instance.FadeAndDo(() => SetActivePanel(gameCredits));
    }
    public void ShowGameOver()
    {
        FadeInFadeOut.Instance.FadeAndDo(() => SetActivePanel(gameOverPanel));
    }

    private void OnStartClicked()
    {
        FadeInFadeOut.Instance.FadeAndDo(() =>
        {
            // Instead of loading a new scene, just hide menu and show gameplay panels
            SetActivePanel(null); // if you're showing gameplay UI, enable it here
            // Or load scene: SceneManager.LoadScene("GameScene"); if you're using scene system
        });
    }

    private void OnRestartClicked()
    {
        FadeInFadeOut.Instance.FadeAndDo(() =>
        {
            // Example: Reset panels instead of loading scene
            ShowGameUI(); // or ShowMainMenu(); or custom reset logic
        });
    }

    private void SetActivePanel(GameObject targetPanel)
    {
        // Deactivate all
        mainMenuPanel?.SetActive(false);
        gamePlayUI?.SetActive(false);
        pausePanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        gameCredits?.SetActive(false);
        pausePanel.SetActive(false);
        // Activate target
        targetPanel?.SetActive(true);

        // Activate the chosen panel

    }
}