using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // For potential scene controls

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
    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;
    public TMP_Text centerMessageText;
    public TMP_Text timerText;

    [Header("Round Flow")]
    public float pauseAfterPoint = 3f;
    public int maxScore = 3;
    public Vector2 knockForce = new(6f, 4f);
    public float matchTime = 30f;

    [Header("Stage Boundaries")]
    public float leftBoundary = -8f;
    public float rightBoundary = 8f;
    public float gizmoHeight = 5f;

    const char STAR = 'O';
    const char DOT = '·';

    int p1Score, p2Score;
    public bool roundLocked;
    float currentTimer;
    Coroutine timerCoroutine;
    public bool optionsOpen = false;

    void Start()
    {
        RefreshScoreUI();
        currentTimer = matchTime;
        timerCoroutine = StartCoroutine(MatchTimerRoutine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UiManager.Instance != null)
            {
                UiManager.Instance.ShowPauseMenu();
            }
        }
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
        //player1.PauseControl(true);
        //player2.PauseControl(true);

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
        // player1.PauseControl(true);
        // player2.PauseControl(true);

        if (p1Score == 0 && p2Score == 0)
        {
            centerMessageText.text = "Time’s up! No winner.";
        }
        else if (p1Score > p2Score)
        {
            centerMessageText.text = "Time’s up! Player 1 wins!";
        }
        else if (p2Score > p1Score)
        {
            centerMessageText.text = "Time’s up! Player 2 wins!";
        }
        else
        {
            centerMessageText.text = "Time’s up! It’s a tie!";
        }
    }

    IEnumerator PointPauseRoutine()
    {
        centerMessageText.text = "Point!";
        yield return new WaitForSecondsRealtime(pauseAfterPoint);

        ResetPositions();
        centerMessageText.text = "";
        // player1.PauseControl(false);
        // player2.PauseControl(false);
        roundLocked = false;
        timerCoroutine = StartCoroutine(MatchTimerRoutine());
    }

    IEnumerator EndGameRoutine(FighterController winner)
    {
        centerMessageText.text = (winner == player1 ? "Player 1" : "Player 2") + " wins!";
        yield return new WaitForSecondsRealtime(pauseAfterPoint);
    }

    void RefreshScoreUI()
    {
        player1ScoreText.text = BuildScoreString(p1Score);
        player2ScoreText.text = BuildScoreString(p2Score);
    }

    string BuildScoreString(int score)
    {
        return new string(STAR, score) + new string(DOT, maxScore - score);
    }

    void ResetPositions()
    {
        player1.transform.position = player1Spawn.position;
        player2.transform.position = player2Spawn.position;

        player1.ResetMotion();
        player2.ResetMotion();
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


