using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class CharMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float MovementSpeed = 5f;
    public float jumpForce = 5f;
    private float dirX;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private string WalkAnim = "IsWalking";
    private string AttackAnim = "IsAttacking";
    private bool isDead = false;   // to track if the player is dead
                                   //  public AttackHitBox attackHitBox;
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
        AnimatePlayer();
        HandleAttack();
    }
    private void FixedUpdate()
    {
        Playerjump();
        HandleAttack();
    }

    void PlayerMoveKeyboard()
    {
        dirX = Input.GetAxis("Horizontal_P2");
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
    void Playerjump()
    {
        if (Input.GetButtonDown("Jump_P2") && isGrounded)
        {
            //isGrounded = false;
            anim.SetBool("IsJumping", true);
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            isGrounded = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Ground_tag))
       {
           isGrounded = true;
           anim.SetBool("IsJumping", false);
       }
    }
      void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals("Ground"))
           {
                isGrounded = false;
            }
        }
        void HandleAttack()
        {
            // If attack button is pressed, trigger the attack animation
            if (Input.GetButtonDown("Fire1") && !isDead)
            {
                anim.SetTrigger(AttackAnim);
            }

        }
       // void Die()
       // {
        //    isDead = true; // set player to dead
        //    anim.SetTrigger("Die"); // trigger death animation
                                    //SceneManager.LoadScene(2);
          //  Debug.Log("playerDie1");// Destroy(gameObject);
      //  }
    }