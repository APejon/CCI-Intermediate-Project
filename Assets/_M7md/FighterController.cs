using System.Collections;
using UnityEngine;

public class FighterController : MonoBehaviour
{
    /* ---------- Inspector Fields ---------- */
    [Header("Input Keys")]
    public string leftKey = "a";
    public string rightKey = "d";
    public string jumpKey = "w";
    public string crouchKey = "s";
    public string attackKey = "space";

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    [Header("Hurtboxes / Hitboxes")]
    public GameObject hurtBoxIdle;
    public GameObject hurtBoxCrouch;
    public GameObject hurtBoxJump;
    public GameObject hitBoxAttack;

    [Header("Attack Settings")]
    public float attackDuration = 0.75f;

    [Header("Sprite Facing")]
    public bool facingRight = true;      // start facing right

    [Header("Enemy Reference")]
    public Transform enemy;
    public bool isBot = false;

    /* ---------- Private State ---------- */
    private Rigidbody2D rb2D;
    private Animator   anim;
    private Vector3    _initialScale;

    private bool   isGrounded;
    private bool   isAttacking;
    private bool   isCrouching;
    [HideInInspector] public float moveInput;   // AI can write

    /* ---------- Public Read-only Helpers ---------- */
    public bool IsCrouching => isCrouching;
    public bool IsAttacking => isAttacking;

    /* ---------- Unity Hooks ---------- */
    void Start()
    {
        rb2D        = GetComponent<Rigidbody2D>();
        anim        = GetComponent<Animator>();
        _initialScale = transform.localScale;

        ActivateHurtBox(hurtBoxIdle);
    }

    void Update()
    {
        CheckGrounded();

        // Auto-face target when grounded
        if (isGrounded && enemy) FaceTowards(enemy.position);

        if (isAttacking) return;   // movement already frozen in FixedUpdate

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
        // Freeze horizontal motion while attacking
        float horizontal = isAttacking ? 0f : moveInput;
        rb2D.linearVelocity = new Vector2(horizontal * moveSpeed, rb2D.linearVelocity.y);
    }

    /* ---------- Facing ---------- */
    public void FaceTowards(Vector3 targetPosition)
    {
        bool shouldFaceRight = targetPosition.x > transform.position.x;
        if (shouldFaceRight != facingRight)
        {
            facingRight = shouldFaceRight;
            Vector3 s   = _initialScale;
            s.x        *= facingRight ? 1 : -1;
            transform.localScale = s;
        }
    }

    /* ---------- Input-handling (player side) ---------- */
    private void HandleMovementInput()
    {
        moveInput = 0f;
        if (Input.GetKey(leftKey))  moveInput = -1f;
        if (Input.GetKey(rightKey)) moveInput =  1f;
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(jumpKey) && isGrounded && !isCrouching)
            TryJump();
    }

    private void HandleCrouchInput()
    {
        if (Input.GetKeyDown(crouchKey) && isGrounded)
            TryCrouch(true);
        else if (Input.GetKeyUp(crouchKey))
            TryCrouch(false);
    }

    private void HandleAttackInput()
    {
        if (Input.GetKeyDown(attackKey))
            TryAttack();
    }

    /* ---------- Public wrappers for AI ---------- */
    public void SetMoveInput(float input)
    {
        moveInput = Mathf.Clamp(input, -1f, 1f);
    }

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
        if (!isAttacking && isGrounded)
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

    /* ---------- Internals ---------- */
    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        hitBoxAttack.SetActive(true);
        anim?.SetBool("isAttacking", true);

        yield return new WaitForSeconds(attackDuration);

        hitBoxAttack.SetActive(false);
        anim?.SetBool("isAttacking", false);
        isAttacking = false;
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheckPoint.position, groundCheckRadius, groundLayer);

        if (isGrounded && !isCrouching && !hurtBoxIdle.activeSelf)
            ActivateHurtBox(hurtBoxIdle);
    }

    private void ActivateHurtBox(GameObject target)
    {
        hurtBoxIdle .SetActive(false);
        hurtBoxCrouch.SetActive(false);
        hurtBoxJump .SetActive(false);
        target.SetActive(true);
    }

    private void UpdateAnimatorParameters()
    {
        if (!anim) return;
        anim.SetBool ("isGrounded", isGrounded);
        anim.SetBool ("isCrouching", isCrouching);
        anim.SetFloat("Speed",      Mathf.Abs(moveInput));
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheckPoint)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}
