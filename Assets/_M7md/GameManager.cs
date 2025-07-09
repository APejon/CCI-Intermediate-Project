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

    public enum CurrentWinnerType
    {
        Unknown,
        P1,
        P2,
        Tie
    };

    public CurrentWinnerType CurrentWinner = CurrentWinnerType.Unknown;

    private void OnEnable()
    {
        CamScript.enabled = true;
        roundLocked = true;
    }
    
    public void ResetMatch()
    {
        player1.EndReset();
        player2.EndReset();
        
        Debug.Log("Reset Match");
        RefreshScoreUI();
        currentTimer = matchTime;
        p1Score = 0;
        p2Score = 0;
        
        player1.isWon = false;
        player1.isLost = false;
        
        player2.isWon = false;
        player2.isLost = false;

        centerMessageText.text = "";
        ResetPositions();
        StartCoroutine(StartCountdownThenFight());
    }

    // This starts the 3,2,1 counter
    public void StartMatch()
    {
        Debug.Log("Start Match");
        currentTimer = matchTime;
        timerText.text = matchTime.ToString();
        StartCoroutine(StartCountdownThenFight());
    }

    void Update()
    {
        if (true)
        {
           // if (Input.GetKeyDown(KeyCode.LeftControl)) {
                if (Input.GetKey(KeyCode.Alpha1))
                {
                    StartCoroutine(EndGameRoutine(player1));
                    player1.isWon = true;
                    player2.isLost = true;
                    player2.Knockback(knockForce);
                } else
                if (Input.GetKey(KeyCode.Alpha2))
                {
                    StartCoroutine(EndGameRoutine(player2));
                    player2.isWon = true;
                    player1.isLost = true;
                    player1.Knockback(knockForce);
                }
            //}
        }
    }

    public void RegisterPoint(FighterController attacker, FighterController defender, Vector2 hitPoint)
{
    if (roundLocked) return;
    roundLocked = true;

    if (timerCoroutine != null)
    {
        StopCoroutine(timerCoroutine);
        timerCoroutine = null;
    }

    if (attacker == player1) ++p1Score;
    else ++p2Score;

    if (p1Score >= maxScore)
    {
        player1.isWon = true;
        player2.isLost = true;
    }
    else if (p2Score >= maxScore)
    {
        player2.isWon = true;
        player1.isLost = true;
    }
    
    defender.Knockback(knockForce);

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
            StartCoroutine(EndGameRoutine(null));
        }
        else if (p1Score > p2Score)
        {
            centerMessageText.text = "Time’s up!\nPlayer 1 wins!";
            StartCoroutine(EndGameRoutine(player1));
        }
        else if (p2Score > p1Score)
        {
            centerMessageText.text = "Time’s up!\nPlayer 2 wins!";
            StartCoroutine(EndGameRoutine(player2));
        }
        else
        {
            centerMessageText.text = "Time’s up!\nIt’s a tie!";
            StartCoroutine(EndGameRoutine(null));
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

    /// <summary>
    /// Go to the Win Screen
    /// </summary>
    /// <param name="winner"></param>
    /// <returns></returns>
    IEnumerator EndGameRoutine(FighterController winner)
    {
        if (winner == player1)
        {
            CurrentWinner = CurrentWinnerType.P1;
        } else if (winner == player2) {
            CurrentWinner = CurrentWinnerType.P2;
        } else
        {
            CurrentWinner = CurrentWinnerType.Tie;
        }
            roundLocked = true;
        centerMessageText.text = (winner == player1 ? "Player 1" : "Player 2") + " wins!";
        winner.triggerWinPose();
        
        yield return new WaitForSecondsRealtime(pauseAfterPoint);

        UiManager.Instance.ShowGameOver();
        //this.enabled = false;
    }

    IEnumerator StartCountdownThenFight()
    {
        roundLocked = true;

        string[] steps = { "3", "2", "1", "Fight!" };
        foreach (string step in steps)
        {
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

        player1.ResetAnimator();
        player2.ResetAnimator();
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
