using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FighterController))]
public class EnemyFighterAI : MonoBehaviour
{
    [Header("Attack Range & Speed")]
    public Transform player;                    // assign your Player’s transform
    public float attackRange     = 1.5f;
    public float chaseSpeed  = 5f;              // when player detected

    [Header("Attack Settings")]
    public float attackCooldown   = 1.2f;       // seconds between attacks
    public float decisionInterval = 0.2f;       // AI "think" every 0.2s

    [Header("Optional Jump/Crouch")]
    [Range(0,1)] public float jumpChance   = 0.1f;  // random chance to jump
    [Range(0,1)] public float crouchChance = 0.05f; // random chance to crouch

    private FighterController ctrl;
    private float lastAttackTime;

    void Start()
    {
        ctrl = GetComponent<FighterController>();
        StartCoroutine(AIBehaviorLoop());
    }

    private IEnumerator AIBehaviorLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(decisionInterval);

            // Distance to player
            float dist = Vector2.Distance(transform.position, player.position);

            // Reset inputs
            ctrl.SetMoveInput(0f);
            ctrl.TryCrouch(false);
            
                // --- Chase or Attack ---
                if (dist > attackRange)
                {
                    // 1. Move toward the player
                    float dir = Mathf.Sign(player.position.x - transform.position.x);
                    ctrl.SetMoveInput(dir * chaseSpeed / ctrl.moveSpeed);
                }
                else
                {
                    // 2. In attack range?
                    if (Time.time - lastAttackTime >= attackCooldown)
                    {
                        ctrl.TryAttack();
                        lastAttackTime = Time.time;
                    }
                }

                // --- Optional random jump/crouch for variety ---
                if (Random.value < jumpChance)   ctrl.TryJump();
                if (Random.value < crouchChance) ctrl.TryCrouch(true);

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
