using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameUIPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("Buttons")]
    public Button startButton;
    public Button restartButton;
    public Button quitButton;

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

        ShowMainMenu();
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

        // Activate target
        targetPanel?.SetActive(true);
    }
}