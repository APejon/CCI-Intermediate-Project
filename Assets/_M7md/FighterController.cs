using System;
using System.Collections;
using UnityEngine;

public class FighterController : MonoBehaviour
{
    /* ── Inspector Fields ───────────────────────────────────────── */
    [Header("Input Keys (player-controlled only)")]
    public string leftKey  = "a";
    public string rightKey = "d";
    public string jumpKey  = "w";
    public string crouchKey = "s";
    public string attackKey = "space";

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    [Header("Hurt- / Hit-Boxes")]
    public GameObject hurtBoxIdle;
    public GameObject hurtBoxCrouch;
    public GameObject hurtBoxJump;
    public GameObject hitBoxAttack;

    [Header("Attack")]
    public float attackDuration = 0.75f;

    [Header("Facing")]
    public bool  facingRight = true;          // start orientation
    public Transform enemy;                   // optional auto-face target

    /* ── Public Flags ───────────────────────────────────────────── */
    [HideInInspector] public bool isBot = false;

    /* ── Read-only accessors for AI / GM ────────────────────────── */
    public bool IsCrouching => isCrouching;
    public bool IsAttacking => isAttacking;

    /* ── Win / Lose pose hooks (optional) ───────────────────────── */
    public Action PlayWinPose    = null;
    public Action PlayDefeatPose = null;

    /* ── Private state ──────────────────────────────────────────── */
    Rigidbody2D rb2D;
    Animator    anim;
    Vector3     initialScale;

    bool  isGrounded;
    bool  isAttacking;
    bool  isCrouching;
    float moveInput;                    // –1 .. 1 ; AI writes directly

    private float leftBoundary;
    private float rightBoundary;

    /* ── Unity Hooks ───────────────────────────────────────────── */
    void Start()
    {
        rb2D        = GetComponent<Rigidbody2D>();
        anim        = GetComponent<Animator>();
        
        initialScale = transform.localScale;
        
        leftBoundary  = GameManager.Instance ? GameManager.Instance.leftBoundary  : -999f;
        rightBoundary = GameManager.Instance ? GameManager.Instance.rightBoundary :  999f;

        ActivateHurtBox(hurtBoxIdle);

        /* default pose triggers if user didn’t assign custom lambdas */
        PlayWinPose    ??= () => anim?.SetTrigger("Win");
        PlayDefeatPose ??= () => anim?.SetTrigger("Lose");
    }

    void Update()
    {
        CheckGrounded();

        if (isGrounded && enemy)
            FaceTowards(enemy.position);

        if (isAttacking) return;                    // locked-out during swing

        if (!isBot)
        {
            HandleMovementInput();
            HandleJumpInput();
            HandleCrouchInput();
            HandleAttackInput();
        }

        UpdateAnimatorParameters();
    }

    void FixedUpdate()
    {
        /* freeze horizontal speed while attacking */
        float horiz = isAttacking ? 0f : moveInput;
        rb2D.linearVelocity = new Vector2(horiz * moveSpeed, rb2D.linearVelocity.y);
        
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, leftBoundary, rightBoundary);
        transform.position = pos;
    }

    /* ── Facing ─────────────────────────────────────────────────── */
    public void FaceTowards(Vector3 pos)
    {
        bool shouldFaceRight = pos.x > transform.position.x;
        if (shouldFaceRight != facingRight)
        {
            facingRight = shouldFaceRight;
            Vector3 s   = initialScale;
            s.x        *= facingRight ? 1 : -1;
            transform.localScale = s;
        }
    }

    /* ── Player-side input helpers ──────────────────────────────── */
    void HandleMovementInput()
    {
        moveInput = 0f;
        if (Input.GetKey(leftKey))  moveInput = -1f;
        if (Input.GetKey(rightKey)) moveInput =  1f;
    }

    void HandleJumpInput()
    {
        if (Input.GetKeyDown(jumpKey) && isGrounded && !isCrouching)
            TryJump();
    }

    void HandleCrouchInput()
    {
        if (Input.GetKeyDown(crouchKey) && isGrounded)
            TryCrouch(true);
        else if (Input.GetKeyUp(crouchKey))
            TryCrouch(false);
    }

    void HandleAttackInput()
    {
        if (Input.GetKeyDown(attackKey))
            TryAttack();
    }

    /* ── Public wrappers for AI / GM ────────────────────────────── */
    public void SetMoveInput(float x) => moveInput = Mathf.Clamp(x, -1f, 1f);

    public void TryJump()
    {
        if (isGrounded && !isCrouching && !isAttacking)
        {
            ActivateHurtBox(hurtBoxJump);
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
            isGrounded    = false;
        }
    }

    public void TryCrouch(bool on)
    {
        if (isGrounded && !isAttacking)
        {
            isCrouching = on;
            ActivateHurtBox(on ? hurtBoxCrouch : hurtBoxIdle);
        }
    }

    public void TryAttack()
    {
        if (!isAttacking)
            StartCoroutine(AttackCoroutine());
    }

    /* ── Attack Logic ───────────────────────────────────────────── */
    IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        moveInput   = 0f;                           // stop walk instantly

        /* tag the HitBox with its owner each swing */
        var hb = hitBoxAttack.GetComponent<HitBox>();
        if (hb) hb.owner = this;

        hitBoxAttack.SetActive(true);
        anim?.SetBool("isAttacking", true);

        yield return new WaitForSeconds(attackDuration);

        hitBoxAttack.SetActive(false);
        anim?.SetBool("isAttacking", false);
        isAttacking = false;
    }

    public void CancelAttack()                     // called by GameManager
    {
        if (!isAttacking) return;

        StopAllCoroutines();
        hitBoxAttack.SetActive(false);
        anim?.SetBool("isAttacking", false);
        isAttacking = false;
    }

    /* ── Motion / control helpers for GameManager ──────────────── */
    public void EnableControl(bool on)
    {
        enabled        = on;                       // toggles Update/FixedUpdate
        if (!on) rb2D.linearVelocity = Vector2.zero;     // brake instantly when off
    }

    public void Knockback(Vector2 impulse)
    {
        rb2D.linearVelocity = Vector2.zero;
        rb2D.AddForce(impulse, ForceMode2D.Impulse);
    }

    public void ResetMotion()
    {
        rb2D.linearVelocity        = Vector2.zero;
        rb2D.angularVelocity = 0f;
    }

    /* ── Grounding & hurt-box state ─────────────────────────────── */
    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheckPoint.position, groundCheckRadius, groundLayer);

        if (isGrounded && !isCrouching && !hurtBoxIdle.activeSelf)
            ActivateHurtBox(hurtBoxIdle);
    }

    void ActivateHurtBox(GameObject target)
    {
        hurtBoxIdle .SetActive(false);
        hurtBoxCrouch.SetActive(false);
        hurtBoxJump .SetActive(false);
        target.SetActive(true);
    }

    /* ── Animator feed ──────────────────────────────────────────── */
    void UpdateAnimatorParameters()
    {
        if (!anim) return;
        anim.SetBool ("isGrounded", isGrounded);
        anim.SetBool ("isCrouching", isCrouching);
        anim.SetFloat("Speed",      Mathf.Abs(moveInput));
    }

    /* ── Debug visuals ──────────────────────────────────────────── */
    void OnDrawGizmosSelected()
    {
        if (groundCheckPoint)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}
