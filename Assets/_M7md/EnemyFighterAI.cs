using UnityEngine;

[RequireComponent(typeof(FighterController))]
public class EnemyFighterAI : MonoBehaviour
{
    /* ───── Target & Combat ───── */
    [Header("Target & Ranges")]
    public Transform player;
    public float   attackRange    = 1.5f;

    [Header("Attack")]
    public float   attackCooldown = 1.2f;

    /* ───── Locomotion ───── */
    [Header("Movement")]
    public float   chaseSpeed     = 5f;      // controller supplies actual speed

    /* ───── Behaviour Switching ───── */
    public enum Behaviour { Idle, Advance, Retreat }

    [Header("Behaviour Probabilities (Advance = 1 – idle – retreat)")]
    [Range(0, 1)] public float idleProbability    = 0.30f;
    [Range(0, 1)] public float retreatProbability = 0.20f;

    [Header("Behaviour Durations (seconds)")]
    public Vector2 idleDurationRange    = new Vector2(0.4f, 1.5f);
    public Vector2 advanceDurationRange = new Vector2(0.6f, 2.0f);
    public Vector2 retreatDurationRange = new Vector2(0.4f, 1.2f);

    /* ───── Random Jump / Crouch ───── */
    [Header("Random Jump / Crouch")]
    [Range(0, 1)] public float jumpChancePerSec   = 0.10f;
    [Range(0, 1)] public float crouchChancePerSec = 0.05f;
    public float   crouchDuration = 0.5f;

    /* ───── Internals ───── */
    FighterController ctrl;
    Behaviour currentBehaviour;
    float behaviourEndTime;
    float lastAttackTime;
    float crouchReleaseTime;

    /* ───── Unity Hooks ───── */
    void Start()
    {
        ctrl       = GetComponent<FighterController>();
        ctrl.isBot = true;                         // ensure controller ignores player input
        PickNextBehaviour();                      // initialise first behaviour
    }

    void Update()
    {
        if (!player) return;

        /* Face player at all times */
        ctrl.FaceTowards(player.position);

        /* Switch behaviour when the timer expires */
        if (Time.time >= behaviourEndTime)
            PickNextBehaviour();

        /* Execute current behaviour */
        ExecuteBehaviour();

        /* Handle attacking if in range */
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            ctrl.TryAttack();
            lastAttackTime = Time.time;
        }

        /* Opportunistic jump (only if not crouching) */
        if (!ctrl.IsCrouching &&
            Random.value < jumpChancePerSec * Time.deltaTime)
        {
            ctrl.TryJump();
        }

        /* Temporary random crouch */
        if (Time.time >= crouchReleaseTime)
        {
            ctrl.TryCrouch(false);         // stand up if crouching

            if (Random.value < crouchChancePerSec * Time.deltaTime)
            {
                ctrl.TryCrouch(true);
                crouchReleaseTime = Time.time + crouchDuration;
            }
        }
    }

    /* ───── Behaviour Helpers ───── */
    void ExecuteBehaviour()
    {
        switch (currentBehaviour)
        {
            case Behaviour.Idle:
                ctrl.SetMoveInput(0f);
                break;

            case Behaviour.Advance:
                MoveTowardsPlayer(+1f);
                break;

            case Behaviour.Retreat:
                MoveTowardsPlayer(-1f);
                break;
        }
    }

    void MoveTowardsPlayer(float directionSign)
    {
        float dir = Mathf.Sign(player.position.x - transform.position.x) * directionSign;
        ctrl.SetMoveInput(dir);            // –1 left, +1 right; controller clamps value
    }

    void PickNextBehaviour()
    {
        /* Choose behaviour based on probabilities */
        float r = Random.value;
        if (r < idleProbability)
            currentBehaviour = Behaviour.Idle;
        else if (r < idleProbability + retreatProbability)
            currentBehaviour = Behaviour.Retreat;
        else
            currentBehaviour = Behaviour.Advance;

        /* Choose duration based on the behaviour */
        Vector2 range = advanceDurationRange;
        switch (currentBehaviour)
        {
            case Behaviour.Idle:    range = idleDurationRange;    break;
            case Behaviour.Retreat: range = retreatDurationRange; break;
        }
        behaviourEndTime = Time.time + Random.Range(range.x, range.y);
    }

    /* ───── Visual Debug ───── */
    void OnDrawGizmosSelected()
    {
        if (player)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
