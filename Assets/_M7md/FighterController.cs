using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class FighterController : MonoBehaviour
{
    /* ── Input (only for human player) ─────────────────────────── */
    [Header("Input Keys")]
    public string leftKey = "a";
    public string rightKey = "d";
    public string jumpKey = "w";
    public string crouchKey = "s"; 
    public string attackKey = "space";

    /* ── Movement / Physics ────────────────────────────────────── */
    [Header("Movement")]
    public float moveSpeed = 5f, jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float     groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    /* ── Hurt- / Hit-boxes ─────────────────────────────────────── */
    [Header("Hurt-/Hit-Boxes")]
    public GameObject hurtIdle;
    public GameObject hurtCrouch;
    public GameObject hurtJump; 
    public GameObject hitAttack;
    public Vector2 knockbackForce;

    /* ── Facing & target ───────────────────────────────────────── */
    public bool facingRight = true;
    public Transform opponent;               // optional, for auto-facing

    /* ── Public state flags (read-only) ────────────────────────── */
    public bool IsCrouching => isCrouching;
    public bool IsAttacking => isAttacking;
    [HideInInspector] public bool isBot = false;

    /* ── Private ------------------------------------------------- */
    Rigidbody2D rb;
    Animator    anim;
    Vector3     startScale;
    
    
    bool isGrounded, isAttacking, isCrouching, isKnockback = false;
    float moveInput;

    void Awake()
    {
        rb         = GetComponent<Rigidbody2D>();
        anim       = GetComponent<Animator>();
        startScale = transform.localScale;
        ActivateHurtBox(hurtIdle);
    }

    /* ── Main loops ────────────────────────────────────────────── */
    void Update()
    {
        CheckGrounded();
        
        if (isGrounded && opponent) FaceTowards(opponent.position);
        if (!isBot && Input.GetKeyUp(crouchKey))
            TryCrouch(false);
        if (isAttacking)            return;          // lock input

        if (!isBot)
        {
            HandleMoveKeys();
            HandleJumpKey();
            HandleCrouchKeys();
            HandleAttackKey();
        }

        anim.SetBool("isGrounded",  isGrounded);
        anim.SetBool("isCrouching", isCrouching);
        anim.SetBool("isAttacking", isAttacking);
        
        /* ---- NEW: walk toggle ---- */
        bool walking = !isAttacking && Mathf.Abs(moveInput) > 0.01f;
        anim.SetBool("isWalking", walking); 
    }

    void FixedUpdate()
    {
        
        if (isKnockback || GameManager.Instance.roundLocked) return;

        float horiz = isAttacking ? 0f : moveInput;
        rb.linearVelocity = new Vector2(horiz * moveSpeed, rb.linearVelocity.y);

        float left  = GameManager.Instance ? GameManager.Instance.leftBoundary  : -999f;
        float right = GameManager.Instance ? GameManager.Instance.rightBoundary :  999f;

        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, left, right);
        transform.position = p;
    }

    /* ── Player-side input helpers ─────────────────────────────── */
    void HandleMoveKeys()
    {
        if (GameManager.Instance.roundLocked) return;
        moveInput = Input.GetKey(leftKey)  ? -1f :
                    Input.GetKey(rightKey) ?  1f : 0f;
    }

    void HandleJumpKey()
    {
        if (GameManager.Instance.roundLocked) return;
        if (Input.GetKeyDown(jumpKey) && isGrounded && !isCrouching)
            TryJump();
    }

    void HandleCrouchKeys()
    {
        if (GameManager.Instance.roundLocked) return;
        if (Input.GetKeyDown(crouchKey) && isGrounded)      TryCrouch(true);
        if (Input.GetKeyUp(crouchKey))                      TryCrouch(false);
    }

    void HandleAttackKey()
    {
        if (Input.GetKeyDown(attackKey) && !GameManager.Instance.roundLocked) TryAttack();
    }

    /* ── Public actions for AI / GM ────────────────────────────── */
    public void SetMoveInput(float v) => moveInput = Mathf.Clamp(v, -1f, 1f);

    public void TryJump()
    {
        if (!isGrounded || isCrouching || isAttacking) return;
        ActivateHurtBox(hurtJump);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded  = false;
    }

    public void TryCrouch(bool on)
    {
        if (!isGrounded || isAttacking) return;
        isCrouching = on;
        ActivateHurtBox(on ? hurtCrouch : hurtIdle);
    }

    public void TryAttack()
    {
        if (isAttacking) return;
        moveInput   = 0f;
        isAttacking = true;
        anim.SetBool("isAttacking", true);   // triggers Animator transition
    }

    /* ── Animation Events --------------------------------------- */
    public void FC_EnableHitBox()
    {
        var hb = hitAttack.GetComponent<HitBox>();
        if (hb) hb.owner = this;
        hitAttack.SetActive(true);
    }

    public void FC_DisableHitBox() => hitAttack.SetActive(false);

    public void FC_EndAttackAnimation()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", false);

        if (isCrouching && !Input.GetKey(crouchKey))
            TryCrouch(false);            // stand up if key no longer held
    }

    /* ── Helpers ------------------------------------------------ */
    void ActivateHurtBox(GameObject target)
    {
        hurtIdle .SetActive(false);
        hurtCrouch.SetActive(false);
        hurtJump .SetActive(false);
        target.SetActive(true);
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle
        (
            groundCheckPoint.position, groundCheckRadius, groundLayer
        );
        if (isGrounded && !isCrouching && !hurtIdle.activeSelf)
            ActivateHurtBox(hurtIdle);
    }

    public void FaceTowards(Vector3 pos)
    {
        bool right = pos.x > transform.position.x;
        if (right != facingRight)
        {
            facingRight = right;
            Vector3 s = startScale; s.x *= facingRight ? 1 : -1;
            transform.localScale = s;
        }
    }

    /* ── Round control (called by GameManager) ------------------ */
    public void PauseControl(bool on)
    {
        GameManager.Instance.roundLocked = on;
    }
    public void ResetMotion() => rb.linearVelocity = Vector2.zero;
    public void Knockback(Vector2 impulse)
    {
        Debug.Log("Knockback Force: " + impulse);
        rb.linearVelocity = Vector2.zero;

        float sign = opponent.position.x > transform.position.x ? -1f : 1f;
        Vector2 knockDir = new Vector2(sign * Mathf.Abs(impulse.x), impulse.y);
        Debug.Log("Knock Direction: " + knockDir);

        rb.AddForce(knockDir, ForceMode2D.Impulse);
        isKnockback = true;

        Invoke(nameof(EndKnockback), 0.2f); // adjust duration as needed
    }

    void EndKnockback()
    {
        isKnockback = false;
    }


}
