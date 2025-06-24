using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class CharMovement : MonoBehaviour
{
    public float MovementSpeed = 5f;
    public float jumpForce = 5f;
    private float dirX;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private string WalkAnim = "isDucking";
    private string AttackAnim = "isBlocking";
    private bool isDead = false;
    private string Ground_tag = "Ground";
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        // attackHitBox = GetComponentInChildren<AttackHitBox>();
    }
    void Start()
    {

    }

    // Update is called once per frame

    void Update()
    {
        PlayerMoveKeyboard();
        HandleDuckInput();
        AnimatePlayer();
    }
    private void FixedUpdate()
    {
      //  Playerjump();
    }

    void PlayerMoveKeyboard()
    {
        dirX = Input.GetAxis("Horizontal");
        transform.position += new Vector3(dirX, 0f, 0f) * MovementSpeed * Time.deltaTime;
    }
    void AnimatePlayer()
    {
        //Moving to the right
        if (dirX > 0)
        {
            anim.SetBool(WalkAnim, true);
            sr.flipX = false;
        }
        //Moving to the Left
        else if (dirX < 0)
        {
            anim.SetBool(WalkAnim, true);
            sr.flipX = true;
        }
        else
        {
            anim.SetBool(WalkAnim, false);
        }


    }
    //  void Playerjump()
    //   {
    //  if (Input.GetButtonDown("Jump") && isGrounded)
    //   {
    //isGrounded = false;
    //  anim.SetBool("isJumping", true);
    //     rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    //     isGrounded = false;
    //   }
    //  }
    void HandleDuckInput()
    {
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            anim.SetBool(AttackAnim, true); // isDucking
        }
        else
        {
            anim.SetBool(AttackAnim, false); // Stop ducking when key is released
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Ground_tag))
        {
            isGrounded = true;
          //  anim.SetBool("isJumping", false);
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Ground_tag))
        {
            isGrounded = false;
        }
    }
}