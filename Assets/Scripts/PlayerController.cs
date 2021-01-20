using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField]
    private GameObject escPanel;
    public int combo = 0;
    [SerializeField]
    public bool usesCombo2;
    [SerializeField]
    private int maxCombo = 3;
    [SerializeField]
    private float movementSpeed, groundCheckRadius, jumpForce;
    [SerializeField]
    private Transform groundCheck, HUD;
    [SerializeField]
    private LayerMask whatIsGround;
    private float xInput;
    private int facingDirection = 1;
    private bool isGrounded, isJumping, canJump, otherPlayerTackled;
    private Vector2 newVelocity, newForce;
    private Rigidbody2D rb;
    private Animator animator;
    private float lastTime, comboCooldown = 0.5f;
    private PhotonView photon;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        photon = GetComponent<PhotonView>();

        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        CheckInput();
    }

    private void FixedUpdate()
    {
        CheckGround();
        ApplyMovement();
    }

    private void MeleeAttack()
    {
        if (lastTime < Time.time && combo < maxCombo)
        {
            combo++;
            lastTime = Time.time + comboCooldown;
        }


        if ((Time.time - lastTime) > comboCooldown)
        {

            combo = 0;
            animator.SetBool("attack1", false);
            animator.SetBool("attack2", false);
            animator.SetBool("attack3", false);

        }

        if (combo == 1)
        {
            if (photon.IsMine)
            {
                animator.SetBool("attack1", true);
                animator.SetBool("attack2", false);
                animator.SetBool("attack3", false);

            }

        }

        if (combo == 2)
        {
            if (photon.IsMine)
            {
                animator.SetBool("attack1", true);
                animator.SetBool("attack2", false);
                animator.SetBool("attack3", false);

            }
        }

        if (combo == 3)
        {
            if (photon.IsMine)
            {
                animator.SetBool("attack1", false);
                animator.SetBool("attack2", true);
                animator.SetBool("attack3", false);

            }
        }
        if (combo == 4 && usesCombo2)
        {
            if (photon.IsMine)
            {
                animator.SetBool("attack1", false);
                animator.SetBool("attack2", false);
                animator.SetBool("attack3", true);

            }
        }
    }

    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (xInput == 1 && facingDirection == -1 && photon.IsMine)
        {
            Flip();
        }
        else if (xInput == -1 && facingDirection == 1 && photon.IsMine)
        {
            Flip();
        }

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && photon.IsMine)
        {
            escPanel.GetComponentInChildren<Text>().text = "Wanna Quit?";
            escPanel.SetActive(true);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            MeleeAttack();
        }

        if (Input.GetButtonUp("Fire1"))
        {
            animator.SetBool("attack1", false);
            animator.SetBool("attack2", false);
            animator.SetBool("attack3", false);
        }

    }
    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        if (rb.velocity.y <= 0.0f)
        {
            isJumping = false;
        }

        if (isGrounded && !isJumping)
        {
            canJump = true;
        }

    }

    

    private void Jump()
    {
        if (canJump)
        {
            if (photon.IsMine)
            {

                canJump = false;
                isJumping = true;
                newVelocity.Set(0.0f, 0.0f);
                animator.SetBool("isJumping", true);
                rb.velocity = newVelocity;
                newForce.Set(0.0f, jumpForce);
                rb.AddForce(newForce, ForceMode2D.Impulse);
            }
        }
    }



    private void ApplyMovement()
    {
        if (isGrounded && !isJumping && photon.IsMine && !otherPlayerTackled) 
        {
            newVelocity.Set(movementSpeed * xInput, 0.0f);
            rb.velocity = newVelocity;
        }
        else if (!isGrounded && photon.IsMine) //If in air
        {
            newVelocity.Set(movementSpeed * xInput, rb.velocity.y);
            rb.velocity = newVelocity;
        }


        if (photon.IsMine)
        {
            if (isGrounded && rb.velocity.magnitude > 0 && !otherPlayerTackled)
            {
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }

            if (rb.velocity.y < -0.1 && !isGrounded) animator.SetBool("isFalling", true);
            else animator.SetBool("isFalling", false);
        }

    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
        HUD.rotation = Quaternion.Euler(0f, transform.rotation.y * facingDirection, 0f);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

}


