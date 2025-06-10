using UnityEngine;

[RequireComponent(typeof(FighterController))]
public class EnemyFighterAI : MonoBehaviour
{
    [Header("Target & Ranges")]
    public Transform player;
    public float attackRange    = 1.5f;

    [Header("Speeds")]
    public float chaseSpeed     = 5f;

    [Header("Attack Settings")]
    public float attackCooldown = 1.2f;
    
    [Header("Random Jump/Crouch (per second)")]
    [Range(0,1)] public float jumpChancePerSec   = 0.1f;
    [Range(0,1)] public float crouchChancePerSec = 0.05f;
    public float crouchDuration   = 0.5f;    // how long a random crouch lasts

    private FighterController ctrl;
    private float  lastAttackTime;
    private float  crouchReleaseTime;

    void Start()
    {
        ctrl = GetComponent<FighterController>();
    }

    void Update()
    {
        // 1) Always face the player
        ctrl.FaceTowards(player.position);

        float dist = Vector2.Distance(transform.position, player.position);

        // 2) Continuous movement toward/away
        if (dist > attackRange)
        {
            // chase
            float dir = Mathf.Sign(player.position.x - transform.position.x);
            ctrl.SetMoveInput(dir * (chaseSpeed / ctrl.moveSpeed));
        }
        else
        {
            // in attack range → stop moving and maybe attack
            ctrl.SetMoveInput(0f);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                ctrl.TryAttack();
                lastAttackTime = Time.time;
            }
        }

        // 3) Random jump
        if (Random.value < jumpChancePerSec * Time.deltaTime)
            ctrl.TryJump();

        // 4) Random crouch with automatic release
        if (Time.time >= crouchReleaseTime)
        {
            // if currently crouching, stand back up
            ctrl.TryCrouch(false);

            // maybe re-crouch now
            if (Random.value < crouchChancePerSec * Time.deltaTime)
            {
                ctrl.TryCrouch(true);
                crouchReleaseTime = Time.time + crouchDuration;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
