using UnityEngine;

public class FighterController : MonoBehaviour
{
    public string leftKey = "a";
    public string rightKey = "d";
    public string jumpKey = "w";
    public string crouchKey = "s";
    public string attackKey = "space";

    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float crouchScale = 2.5f;

    private Rigidbody2D rb2D;
    private bool isGrounded = true;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontal = 0f;

        if (Input.GetKey(leftKey)) horizontal = -1f;
        else if (Input.GetKey(rightKey)) horizontal = 1f;

        rb2D.linearVelocity = new Vector2(horizontal * moveSpeed, rb2D.linearVelocity.y);

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        if (Input.GetKeyDown(crouchKey) && isGrounded)
        {
            //CROUCH
            Vector2 newScale = transform.localScale;
            newScale.y *= crouchScale; // Scale only the X-axis
            transform.localScale = newScale; // Apply the new scale
        } else if (Input.GetKeyDown(crouchKey) && isGrounded)
        {
            //A
        }

        if (Input.GetKeyDown(attackKey))
        {
            Debug.Log("Attack!");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}

