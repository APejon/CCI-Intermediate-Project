using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UiManager : MonoBehaviour
{

    public static UiManager Instance { get; private set; }

   // [Header("Panels")]
    public GameObject mainMenuPanel;
   // public GameObject gamePlayUI;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject gameCredits;

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
        if (startButton != null) startButton.onClick.AddListener(() => SceneLoader.Instance.LoadGameScene());
        if (restartButton != null) restartButton.onClick.AddListener(() => SceneLoader.Instance.RestartScene());
        if (quitButton != null) quitButton.onClick.AddListener(() => Application.Quit());
        if (openWebButton != null) openWebButton.onClick.AddListener(OpenWeb); // Hook URL button
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
        SceneManager.LoadScene(0);
    }

    public void ShowPauseMenu()
    {
        SetActivePanel(pausePanel);
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
       // gamePlayUI?.SetActive(false);
        pausePanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        gameCredits?.SetActive(false);
        pausePanel.SetActive(false);
        // Activate target
        targetPanel?.SetActive(true);

        // Activate the chosen panel

    }
}