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
    public GameObject hitStand;
    public GameObject hitCrouch; 
    public GameObject hitJump;
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


    private bool isGrounded, isAttacking, isCrouching, isKnocked = false;
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
        //anim.SetBool("isAttacking", isAttacking);
        
        /* ---- NEW: walk toggle ---- */
        bool walking = !isAttacking && Mathf.Abs(moveInput) > 0.01f;
        anim.SetBool("isWalking", walking); 
    }

    void FixedUpdate()
    {
        
        if (isCrouching || !isGrounded || isKnocked || GameManager.Instance.roundLocked) return;

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
        // moveInput = Input.GetKey(leftKey) ? -1f :
        //             Input.GetKey(rightKey) ? 1f : 0f;
        if (gameObject.tag == "P1")
        {
            moveInput = Input.GetAxis("Horizontal_KB_P1");
            if (moveInput == 0)
                moveInput = Input.GetAxis("Horizontal_C_P1");
            if (moveInput == 0)
                moveInput = Input.GetAxis("Horizontal_CDPAD_P1");
        }
        else if (gameObject.tag == "P2")
        {
            moveInput = Input.GetAxis("Horizontal_KB_P2");
            if (moveInput == 0)
                moveInput = Input.GetAxis("Horizontal_C_P2");
            if (moveInput == 0)
                moveInput = Input.GetAxis("Horizontal_CDPAD_P2");
        }

    }

    void HandleJumpKey()
    {
        if (GameManager.Instance.roundLocked || isKnocked) return;
        if (/*Input.GetKeyDown(jumpKey)*/ (Input.GetButtonDown("Jump_KB_P1")
        || Input.GetButtonDown("Jump_C_P1")
        || Input.GetAxis("Jump_C_P1") < 0
        || Input.GetAxis("Jump_CDPAD_P1") > 0)  && gameObject.tag == "P1" && isGrounded && !isCrouching)
            TryJump();
        else if (/*Input.GetKeyDown(jumpKey)*/ (Input.GetButtonDown("Jump_KB_P2")
        || Input.GetButtonDown("Jump_C_P2")
        || Input.GetAxis("Jump_C_P2") < 0
        || Input.GetAxis("Jump_CDPAD_P2") > 0) && gameObject.tag == "P2" && isGrounded && !isCrouching)
            TryJump();
    }

    void HandleCrouchKeys()
    {
        if (GameManager.Instance.roundLocked) return;
        if (/*Input.GetKeyDown(crouchKey)*/ (Input.GetButton("Crouch_KB_P1")
        || Input.GetButton("Crouch_C_P1")
        || Input.GetAxis("Crouch_C_P1") > 0
        || Input.GetAxis("Crouch_CDPAD_P1") < 0) && gameObject.tag == "P1" && isGrounded)
            TryCrouch(true);
        else if (/*Input.GetKeyUp(crouchKey)*/ (Input.GetButtonUp("Crouch_KB_P1")
        || Input.GetButtonUp("Crouch_C_P1")
        || Input.GetAxis("Crouch_C_P1") == 0
        || Input.GetAxis("Crouch_CDPAD_P1") == 0) && gameObject.tag == "P1")
            TryCrouch(false);
        if (/*Input.GetKeyDown(crouchKey)*/ (Input.GetButton("Crouch_KB_P2")
        || Input.GetButton("Crouch_C_P2")
        || Input.GetAxis("Crouch_C_P2") > 0
        || Input.GetAxis("Crouch_CDPAD_P2") < 0) && gameObject.tag == "P2" && isGrounded)
            TryCrouch(true);
        else if (/*Input.GetKeyUp(crouchKey)*/ (Input.GetButtonUp("Crouch_KB_P2")
        || Input.GetButtonUp("Crouch_C_P2")
        || Input.GetAxis("Crouch_C_P2") == 0
        || Input.GetAxis("Crouch_CDPAD_P2") == 0) && gameObject.tag == "P2")
            TryCrouch(false);
    }

    void HandleAttackKey()
    {
        if (/*Input.GetKeyDown(attackKey)*/ (Input.GetButtonDown("Fire_KB_P1") || Input.GetButtonDown("Fire_C_P1")) && gameObject.tag == "P1" && !GameManager.Instance.roundLocked) TryAttack();
        if (/*Input.GetKeyDown(attackKey)*/ (Input.GetButtonDown("Fire_KB_P2") || Input.GetButtonDown("Fire_C_P2")) && gameObject.tag == "P2" && !GameManager.Instance.roundLocked) TryAttack();
    }

    /* ── Public actions for AI / GM ────────────────────────────── */
    public void SetMoveInput(float v) => moveInput = Mathf.Clamp(v, -1f, 1f);

    public void TryJump()
    {
        if (!isGrounded || isCrouching || isAttacking) return;
        AudioManager.Instance.Play(gameObject.tag + "_jump", Random.Range(0.8f,1.2f), Random.Range(0.8f,1.2f));
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
        AudioManager.Instance.Play(gameObject.tag + "_whip", Random.Range(0.8f,1.2f), Random.Range(0.8f,1.2f));
        moveInput   = 0f;
        isAttacking = true;
        anim.SetBool("isAttacking", true);   // triggers Animator transition
    }

    /* ── Animation Events --------------------------------------- */
    public void FC_EnableHitBox() // Stand Hitbox
    {
        var hb = hitStand.GetComponent<HitBox>();
        if (hb) hb.owner = this;
        hitStand.SetActive(true);
    }
    public void FC_DisableHitBox() => hitStand.SetActive(false);
    
    public void FC_EnableJumpHitbox() // Jump Hitbox
    {
        var hb = hitJump.GetComponent<HitBox>();
        if (hb) hb.owner = this;
        hitJump.SetActive(true);
    }

    public void FC_DisableJumpHitbox() => hitJump.SetActive(false);

    public void FC_EnableCrouchHitbox() // Crouch Hitbox
    {
        var hb = hitJump.GetComponent<HitBox>();
        if (hb) hb.owner = this;
        hitCrouch.SetActive(true);
    }
    
    public void FC_DisableCrouchHitbox() => hitCrouch.SetActive(false);

    public void FC_EndAttackAnimation()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);

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
        if (skipGroundCheck)
        {
            isGrounded = false;
            return;
        }
        isGrounded = Physics2D.OverlapCircle
        (
            groundCheckPoint.position, groundCheckRadius, groundLayer
        );
        if (isGrounded && !isCrouching && !hurtIdle.activeSelf)
            ActivateHurtBox(hurtIdle);
    }

    public float flipThreshold = 0.1f; // <-- Add this to your header or as a field

    public void FaceTowards(Vector3 targetPos)
    {
        if (!groundCheckPoint) return;

        float dx = targetPos.x - transform.position.x;

        // Only flip if the opponent is clearly to the other side
        if (Mathf.Abs(dx) < flipThreshold) return;

        bool shouldFaceRight = dx > 0;
        if (shouldFaceRight != facingRight)
        {
            Vector3 pivotWorldPos = groundCheckPoint.position;

            facingRight = shouldFaceRight;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (facingRight ? 1 : -1);
            transform.localScale = scale;

            Vector3 newPivotWorldPos = groundCheckPoint.position;
            Vector3 pivotOffset = pivotWorldPos - newPivotWorldPos;
            transform.position += pivotOffset;
        }
    }


    /* ── Round control (called by GameManager) ------------------ */
    // public void PauseControl(bool on)
    // {
    //     GameManager.Instance.roundLocked = on;
    // }
    public void ResetMotion() => rb.linearVelocity = Vector2.zero;
    public void Knockback(Vector2 impulse)
    {
        Debug.Log("Knockback Force: " + impulse);
        rb.linearVelocity = Vector2.zero;

        float sign = opponent.position.x > transform.position.x ? -1f : 1f;
        Vector2 knockDir = new Vector2(sign * Mathf.Abs(impulse.x), impulse.y);
        Debug.Log("Knock Direction: " + knockDir);

        rb.AddForce(knockDir, ForceMode2D.Impulse);
        GroundCheckDisable();
        
        anim.SetTrigger("Knocked");
        isKnocked = true;
        
        //Invoke(nameof(EndKnockback), 0.2f); // adjust duration as needed
    }
    
    bool skipGroundCheck;
    
    void GroundCheckDisable()
    {
        skipGroundCheck = true;
        Debug.Log("Ground Check Disabled");
        Invoke(nameof(EnableGroundCheck), 0.2f); // or use animation event
    }

    void EnableGroundCheck()
    {
        Debug.Log("Ground Check Enabled");
        skipGroundCheck = false;
    }

    public void ResetKnock()
    {
        isKnocked = false;
        anim.SetTrigger("Reset");
    }
    

}
