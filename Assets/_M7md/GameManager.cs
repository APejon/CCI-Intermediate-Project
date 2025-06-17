using System.Collections;
using UnityEngine;
using TMPro;                       // ← TextMesh Pro namespace

public class GameManager : MonoBehaviour
{
    /* ── Singleton ─────────────────────────────────────────────── */
    public static GameManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else                  { Destroy(gameObject); }
    }

    /* ── Inspector Hooks ───────────────────────────────────────── */
    [Header("Fighters & Spawns")]
    public FighterController player1;
    public FighterController player2;
    public Transform         player1Spawn;
    public Transform         player2Spawn;
    
    [Header("UI (TMP)")]
    public TMP_Text player1ScoreText;   // left-side “★··” etc.
    public TMP_Text player2ScoreText;   // right-side “★··” etc.
    public TMP_Text centerMessageText;  // “Point!” / “Player 1 wins!” banner

    [Header("Round Flow")]
    public float pauseAfterPoint = 3f;           // real seconds
    public int   maxScore        = 3;            // first to 3 wins
    public Vector2 knockForce    = new(6f, 4f);  // x push, y lift

    /* ── Runtime State ─────────────────────────────────────────── */
    int  p1Score, p2Score;
    bool roundLocked;           // true while pause coroutine is running
    
    /* ── Stage limits (tweak in the Inspector) ──────────────────── */
    [Header("Stage Boundaries")]
    public float leftBoundary  = -8f;
    public float rightBoundary =  8f;
    public float gizmoHeight   = 5f; 

    /* ── Constants for score glyphs ────────────────────────────── */
    const char STAR = 'O';
    const char DOT  = '·';

    void Start() => RefreshScoreUI();  // initialise “···”

    /* ── Entry point called from HitBox.cs ─────────────────────── */
    public void RegisterPoint(FighterController attacker,
                              FighterController defender,
                              Vector2 hitPoint)
    {
        if (roundLocked) return;
        roundLocked = true;

        /* 1)  knock-back the loser */
        Vector2 dir = (defender.transform.position.x < attacker.transform.position.x)
            ? new Vector2(-knockForce.x, knockForce.y)
            : new Vector2( knockForce.x, knockForce.y);
        defender.Knockback(dir);

        /* 2) Update score */
        if (attacker == player1) ++p1Score; else ++p2Score;
        RefreshScoreUI();

        /* 3) Disable control for both */
        player1.EnableControl(false);
        player2.EnableControl(false);

        /* 4) Decide what happens next */
        if (p1Score >= maxScore || p2Score >= maxScore)
            StartCoroutine(EndGameRoutine(attacker));
        else
            StartCoroutine(PointPauseRoutine());
    }

    /* ── Coroutines ────────────────────────────────────────────── */
    IEnumerator PointPauseRoutine()
    {
        centerMessageText.text = "Point!";
        yield return new WaitForSecondsRealtime(pauseAfterPoint);

        ResetPositions();
        centerMessageText.text = "";
        player1.EnableControl(true);
        player2.EnableControl(true);
        roundLocked = false;
    }

    IEnumerator EndGameRoutine(FighterController winner)
    {
        centerMessageText.text = (winner == player1 ? "Player 1" : "Player 2") + " wins!";
        yield return new WaitForSecondsRealtime(pauseAfterPoint);

        // Game finished – fighters stay frozen. Replace with
        // SceneManager.LoadScene("MainMenu"); if you need a transition.
    }

    /* ── Helpers ───────────────────────────────────────────────── */
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
        float halfH  = gizmoHeight * 0.5f;
        Vector3 btmL = new(leftBoundary , transform.position.y - halfH, 0f);
        Vector3 topL = new(leftBoundary , transform.position.y + halfH, 0f);
        Vector3 btmR = new(rightBoundary, btmL.y,                         0f);
        Vector3 topR = new(rightBoundary, topL.y,                         0f);
        Gizmos.DrawLine(btmL, topL);
        Gizmos.DrawLine(btmR, topR);
    }
}
