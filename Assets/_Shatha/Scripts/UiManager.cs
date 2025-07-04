using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UiManager : MonoBehaviour
{

    public static UiManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gamePlayUI;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject gameCredits;
    public GameObject scrIntroOne;  // First intro screen shown after pressing start, usually controls screen
    public GameObject fadePanel;

    [Header("Buttons")]
    public Button startButton;
    public Button restartButton;
    public Button quitButton;
    public Button openWebButton;
    
    [Header("Scripts")]
    public GameManager gameManager;

    public string url = "https://itch.io/profile/jalboot"; // Replace with your desired URL

    public FighterController P2Controller;  
    public EnemyFighterAI P2Ai;

    private bool _bMultiplayer = false; // If false = 1 player vs computer, true = two player controllers
    public bool bMultiplayer
    {
        get
        {
            return _bMultiplayer;
        }
        set {
            _bMultiplayer = value;
            if (value )  // Two player mode
            {
                P2Controller.enabled = true;
                P2Ai.enabled = false;
            } else  // Single player Arcade mode
            {
                P2Ai.enabled = true;
            }
        }
    }
    

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

    void Update()
    {
        if (scrIntroOne.activeInHierarchy == false) return;


        bool controllerPressed = false;

        // Check the first few joystick buttons (common ones: 0�15)
        for (int i = 0; i < 16; i++)
        {
            if (Input.GetKeyDown("joystick button " + i))
            {
                controllerPressed = true;
                break;
            }
        }

        if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || controllerPressed)
        {
            Debug.Log("Pressed something");

            ShowGameUI();
        }
    }

    public void OpenWeb()
    {
        Application.OpenURL(url);
    }
    // === UI Control Methods ===
    public void ShowMainMenu()
    {
        
        FadeInFadeOut.Instance.FadeAndDo(() => SetActivePanel(mainMenuPanel));
    }

    public void ShowGameUI()
    {

        FadeInFadeOut.Instance.FadeAndDo(() => SetActivePanel(gamePlayUI));
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

            gameManager.enabled = true;
        });
    }

    public void OnArcade()
    {
        bMultiplayer = false;
        scrIntroOne.gameObject.SetActive(true);
    }

    public void OnTwoPlayer()
    {
        bMultiplayer = true;
        scrIntroOne.gameObject.SetActive(true);
    }

    public void IntroScreenOne()
    {
        gamePlayUI.gameObject.SetActive(true);
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
        pausePanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        gameCredits?.SetActive(false);
        pausePanel.SetActive(false);
        scrIntroOne.SetActive(false);
        // Activate target
        targetPanel?.SetActive(true);

    }
}