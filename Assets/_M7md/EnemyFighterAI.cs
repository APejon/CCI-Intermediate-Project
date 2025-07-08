using UnityEngine;

[RequireComponent(typeof(FighterController))]
public class EnemyFighterAI : MonoBehaviour
{
    /* ── Configurable in Inspector ────────────────────────────── */
    [Header("Target & Ranges")]
    public Transform player;
    public float attackRange = 1.5f, attackCooldown = 1.2f;

    [Header("Behaviour Probabilities")]
    [Range(0, 1)] public float idleProb = 0.30f;
    [Range(0, 1)] public float retreatProb = 0.20f;

    [Header("Behaviour Durations")]
    public Vector2 idleDur = new(0.4f, 1.5f);
    public Vector2 advDur  = new(0.6f, 2.0f);
    public Vector2 retDur  = new(0.4f, 1.2f);

    [Header("Random Jump / Crouch")]
    [Range(0, 1)] public float jumpChance = 0.10f;
    [Range(0, 1)] public float crouchChance = 0.05f;
    public float crouchDuration = 0.5f;

    /* ── Internals ───────────────────────────────────────────── */
    enum Behaviour { Idle, Advance, Retreat }

    FighterController ctrl;
    Behaviour mode;
    float modeEnd, lastAttack, crouchRelease;

    /* ── Unity hooks ──────────────────────────────────────────── */
    void Start()
    {
        ctrl = GetComponent<FighterController>();
        ctrl.isBot = true;
        StartAI();
    }

    public void StartAI() => PickNextMode();
    public void ResetAI()
    {
        lastAttack = 0f;
        crouchRelease = 0f;
    }
    

    void Update()
    {
        Debug.Log("AI Update Running");
        if (!player || !enabled || GameManager.Instance.roundLocked) {
            Debug.Log("AI Disabled: Round Paused");
            return;     // disabled during round pause
        }

        if (Time.time >= modeEnd) PickNextMode();
        ExecuteMode();

        // What direction facing
        // Vector2 vDir = (player.position - transform.position);
        // vDir.Normalize();
        
        float d = player.position.x - transform.position.x;
        d = Mathf.Abs(d);
        
        //float d = Vector2.Distance(transform.position, player.position);
        if (d <= attackRange && Time.time >= lastAttack + attackCooldown)
        {
            ctrl.TryAttack();
            lastAttack = Time.time;
            Debug.Log("AI ATTACKS!");
        }

        if (!ctrl.IsCrouching && Random.value < jumpChance * Time.deltaTime)
        {
            ctrl.TryJump();
            Debug.Log("AI JUMP!");
        }

        if (Time.time >= crouchRelease)
        {
            ctrl.TryCrouch(false);
            Debug.Log("AI CROUCH!");
            if (Random.value < crouchChance * Time.deltaTime)
            {
                ctrl.TryCrouch(true);
                crouchRelease = Time.time + crouchDuration;
            }
        }
    }

    /* ── Modes -------------------------------------------------- */
    void ExecuteMode()
    {
        if (ctrl.IsAttacking || ctrl.IsCrouching || GameManager.Instance.roundLocked) {
            ctrl.SetMoveInput(0f);
            Debug.Log("ExecuteMode Aborted");// stop AI movement while locked
            Debug.Log("CROUCHING: " + ctrl.IsCrouching);
            Debug.Log("ATTACKING: " + ctrl.IsAttacking);
            return;
        }

        switch (mode)
        {
            case Behaviour.Idle:    ctrl.SetMoveInput(0f);        break;
            case Behaviour.Advance: Move( +1f);                break;
            case Behaviour.Retreat: Move( -1f);                break;
        }
    }

    void Move(float sign)
    {
        float dir = Mathf.Sign(player.position.x - transform.position.x) * sign;

        // stop pushing when at wall
        float left  = GameManager.Instance ? GameManager.Instance.leftBoundary  : -999f;
        float right = GameManager.Instance ? GameManager.Instance.rightBoundary :  999f;
        bool atLeft  = transform.position.x <= left  + 0.05f;
        bool atRight = transform.position.x >= right - 0.05f;

        if ((dir < 0 && atLeft) || (dir > 0 && atRight)) dir = 0;
        ctrl.SetMoveInput(dir);
    }

    void PickNextMode()
    {   Debug.Log("PickNextMode");
        float r = Random.value;
        mode = r < idleProb ? Behaviour.Idle : r < idleProb + retreatProb ? Behaviour.Retreat : Behaviour.Advance;

        Vector2 range = mode switch
        {
            Behaviour.Idle    => idleDur, Behaviour.Retreat => retDur, _ => advDur
        };
        modeEnd = Time.time + Random.Range(range.x, range.y);
    }

    /* ── Visual aid -------------------------------------------- */
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
