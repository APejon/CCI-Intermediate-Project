using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    [Header("Fighters & Spawns")]
    public FighterController player1;
    public FighterController player2;
    public Transform player1Spawn;
    public Transform player2Spawn;

    [Header("UI (TMP)")]
    public TMP_Text centerMessageText;
    public TMP_Text timerText;
    public TMP_Text countdownText;

    [Header("Round Flow")]
    public float pauseAfterPoint = 3f;
    public int maxScore = 3;
    public Vector2 knockForce = new(6f, 4f);
    public float matchTime = 30f;

    [Header("Stage Boundaries")]
    public float leftBoundary = -8f;
    public float rightBoundary = 8f;
    public float gizmoHeight = 5f;

    [Header("Main Camera")] public CameraControl CamScript;

    [Header("Score Sprites")]
    public Sprite filledSprite;
    public Sprite emptySprite;

    [Header("Score UI Images")]
    public Image[] player1ScoreImages;
    public Image[] player2ScoreImages;
    
    const char STAR = 'O';
    const char DOT = '·';

    public int p1Score, p2Score;
    public bool roundLocked;
    float currentTimer;
    Coroutine timerCoroutine;
    public bool optionsOpen = false;
    private bool pausing;

    private void OnEnable() => 
        CamScript.enabled = true;

    void Start()
    {
        RefreshScoreUI();
        currentTimer = matchTime;
        StartCoroutine(StartCountdownThenFight());
        pausing = false;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        pausing = !pausing;
        Time.timeScale = pausing ? 0f : 1f;

        if (UiManager.Instance != null)
        {
            if (pausing)
                UiManager.Instance.ShowPauseMenu();
            else
                UiManager.Instance.ShowGameUI(); // Or hide pause panel however you prefer
        }

        Debug.Log(pausing ? "Game Paused" : "Game Resumed");
    }

    public void RegisterPoint(FighterController attacker, FighterController defender, Vector2 hitPoint)
    {
        if (roundLocked) return;
        roundLocked = true;

        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        defender.Knockback(knockForce);

        if (attacker == player1) ++p1Score;
        else ++p2Score;

        RefreshScoreUI();

        if (p1Score >= maxScore || p2Score >= maxScore)
            StartCoroutine(EndGameRoutine(attacker));
        else
            StartCoroutine(PointPauseRoutine());
    }

    IEnumerator MatchTimerRoutine()
    {
        while (currentTimer > 0f)
        {
            if (!optionsOpen && !roundLocked)
            {
                currentTimer -= Time.deltaTime;
                timerText.text = Mathf.CeilToInt(currentTimer).ToString();
            }
            yield return null;
        }

        roundLocked = true;

        if (p1Score == 0 && p2Score == 0)
        {
            centerMessageText.text = "Time’s up!\nNo winner.";
        }
        else if (p1Score > p2Score)
        {
            centerMessageText.text = "Time’s up!\nPlayer 1 wins!";
        }
        else if (p2Score > p1Score)
        {
            centerMessageText.text = "Time’s up!\nPlayer 2 wins!";
        }
        else
        {
            centerMessageText.text = "Time’s up!\nIt’s a tie!";
        }
    }

    IEnumerator PointPauseRoutine()
    {
        centerMessageText.text = "Point!";
        yield return new WaitForSecondsRealtime(pauseAfterPoint);

        ResetPositions();
        centerMessageText.text = "";
        StartCoroutine(StartCountdownThenFight());
    }

    IEnumerator EndGameRoutine(FighterController winner)
    {
        centerMessageText.text = (winner == player1 ? "Player 1" : "Player 2") + " wins!";
        yield return new WaitForSecondsRealtime(pauseAfterPoint);
    }

    IEnumerator StartCountdownThenFight()
    {
        roundLocked = true;

        string[] steps = { "3", "2", "1", "Fight!" };
        foreach (string step in steps)
        {
            while (pausing)
                yield return null;
            countdownText.text = step;
            yield return new WaitForSecondsRealtime(1f);
        }

        countdownText.text = "";
        roundLocked = false;

        if (timerCoroutine == null)
            timerCoroutine = StartCoroutine(MatchTimerRoutine());
    }

    void RefreshScoreUI()
    {
        UpdateScoreImages(player1ScoreImages, p1Score);
        UpdateScoreImages(player2ScoreImages, p2Score);
    }

    void UpdateScoreImages(Image[] scoreImages, int score)
    {
        for (int i = 0; i < scoreImages.Length; i++)
        {
            scoreImages[i].sprite = i < score ? filledSprite : emptySprite;
        }
    }

    void ResetPositions()
    {
        player1.transform.position = player1Spawn.position;
        player2.transform.position = player2Spawn.position;

        player1.ResetMotion();
        player2.ResetMotion();

        player1.ResetKnock();
        player2.ResetKnock();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        float halfH = gizmoHeight * 0.5f;
        Vector3 btmL = new(leftBoundary, transform.position.y - halfH, 0f);
        Vector3 topL = new(leftBoundary, transform.position.y + halfH, 0f);
        Vector3 btmR = new(rightBoundary, btmL.y, 0f);
        Vector3 topR = new(rightBoundary, topL.y, 0f);
        Gizmos.DrawLine(btmL, topL);
        Gizmos.DrawLine(btmR, topR);
    }
}
