using System.Collections;
using UnityEngine;

public class FighterController : MonoBehaviour
{
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

    private Rigidbody2D rb2D;
    private Animator anim;
    private bool isGrounded = true;
    private bool isAttacking = false;
    private bool isCrouching = false;
    
    [Header("Sprite Facing")]
    public bool facingRight = true;   // start facing right
    private Vector3 _initialScale;
    
    [Header("Enemy Reference")]
    public Transform enemy;
    
    // Make moveInput protected so AI can override it:
    [HideInInspector] public float moveInput = 0f;

    void Start()
    {
        _initialScale = transform.localScale;
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ActivateHurtBox(hurtBoxIdle);
    }

    void Update()
    {
        CheckGrounded();

        if (isGrounded)
        {
            if(enemy != null) FaceTowards(enemy.position);
        }

        if (isAttacking) return;

        HandleMovementInput();
        HandleJumpInput();
        HandleCrouchInput();
        HandleAttackInput();
        UpdateAnimatorParameters();
    }
    
    // Call this after you decide to move or attack
    public void FaceTowards(Vector3 targetPosition)
    {
        bool shouldFaceRight = targetPosition.x > transform.position.x;
        if (shouldFaceRight != facingRight)
            Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = _initialScale;
        s.x *= facingRight ? 1 : -1;
        transform.localScale = s;
    }

    void FixedUpdate()
    {
        rb2D.linearVelocity = new Vector2(moveInput * moveSpeed, rb2D.linearVelocity.y);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheckPoint.position,
            groundCheckRadius,
            groundLayer
        );

        if (isGrounded && !hurtBoxIdle.activeSelf && !isCrouching)
        {
            ActivateHurtBox(hurtBoxIdle);
        }
    }

    private void HandleMovementInput()
    {
        moveInput = 0f;
        if (Input.GetKey(leftKey)) moveInput = -1f;
        else if (Input.GetKey(rightKey)) moveInput = 1f;
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(jumpKey) && isGrounded && !isCrouching)
        {
            ActivateHurtBox(hurtBoxJump);
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
    }

    private void HandleCrouchInput()
    {
        if (Input.GetKeyDown(crouchKey) && isGrounded)
        {
            isCrouching = true;
            ActivateHurtBox(hurtBoxCrouch);
        }
        else if (Input.GetKeyUp(crouchKey))
        {
            isCrouching = false;
            ActivateHurtBox(hurtBoxIdle);
        }
    }

    private void HandleAttackInput()
    {
        if (Input.GetKeyDown(attackKey) && !isAttacking)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        hitBoxAttack.SetActive(true);
        if (anim != null) anim.SetBool("isAttacking", true);

        yield return new WaitForSeconds(attackDuration);

        hitBoxAttack.SetActive(false);
        if (anim != null) anim.SetBool("isAttacking", false);
        isAttacking = false;
    }

    private void ActivateHurtBox(GameObject target)
    {
        hurtBoxIdle.SetActive(false);
        hurtBoxCrouch.SetActive(false);
        hurtBoxJump.SetActive(false);
        target.SetActive(true);
    }

    private void UpdateAnimatorParameters()
    {
        if (anim == null) return;

        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isCrouching", isCrouching);
        anim.SetFloat("Speed", Mathf.Abs(moveInput));
    }

    // ~~~ These wrappers let the AI script trigger behaviors ~~~ //
    public void SetMoveInput(float input)
    {
        moveInput = Mathf.Clamp(input, -1f, 1f);
    }

    public void TryJump()
    {
        if (isGrounded && !isCrouching && !isAttacking)
        {
            // reuse your existing logic:
            ActivateHurtBox(hurtBoxJump);
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
            isGrounded = false;
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

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}
