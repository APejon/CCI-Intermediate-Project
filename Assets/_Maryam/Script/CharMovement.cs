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
    //private string Ground_tag = "Ground";
    // private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PlayerMoveKeyboard();
        AnimatePlayer();

        // Attack input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetBool(AttackAnim, true);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            anim.SetBool(AttackAnim, false);
        }

    }
    void PlayerMoveKeyboard()
    {
        dirX = Input.GetAxisRaw("Horizontal");
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
}