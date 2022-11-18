using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Animator anim;
    float speed;
    public float sprintSpeed;
    public float runSpeed  = 12f;
    public float crouchSpeed;
    public float gravity = -9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    Vector3 velocity;
    Vector3 forwardDirection;
    Vector3 move;
    Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    Vector3 standingCenter = new Vector3(0, 0, 0);
    bool isGrounded;
    bool isCrouching;
    bool isSprinting;
    public float jumpHeight = 3f;
    public float totalExtraJumpCharges;
    float extraJumpCharges;
    float crouchHeight = 0.5f;
    float startHeight;
    private void Start()
    {
        startHeight = transform.localScale.y;
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // Set Grounded State
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        // Reset Velocity
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        HandleInput();
        Jump();
    }
    private void HandleInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Crouch
        if (Input.GetKeyDown(KeyCode.C))
        {
            Crouch();
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            ExitCrouch();
        }
        // Sprint
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && !isCrouching)
        {
            isSprinting = true;
            Sprint();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
            Idle();
        }
        SpeedHandler();
    }


    void Jump()
    {
        if (Input.GetButtonDown("Jump") && extraJumpCharges > 0)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            JumpAnim();
            extraJumpCharges--;
        }
        else if (Input.GetButtonDown("Jump") && extraJumpCharges == 0 && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            JumpAnim();
        }
        // reset extra jumps to starting value
        if (isGrounded)
        {
            extraJumpCharges = totalExtraJumpCharges;
        }
    }

    void Crouch()
    {
        controller.height = crouchHeight;
        controller.center = crouchingCenter;
        transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
        
        isCrouching = true;
    }
    void ExitCrouch()
    {
        controller.height = (startHeight * 2);
        controller.center = standingCenter;
        transform.localScale = new Vector3(transform.localScale.x, startHeight, transform.localScale.z);
        isCrouching = false;
    }

    void SpeedHandler()
    {
        speed = isSprinting ? sprintSpeed : isCrouching ? crouchSpeed : runSpeed;
        // Set Animation States
        if (move != Vector3.zero && !isSprinting) 
        {
            Walk();
        }
        if (move == Vector3.zero)
        {
            Idle();
        }
    }

    // Animation Settings
    private void Idle()
    {
        anim.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
    }
    private void Walk()
    {
        anim.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
    }
    private void Sprint()
    {
        anim.SetFloat("Speed", 1f, 0.1f, Time.deltaTime);
    }

    private void JumpAnim()
    {
        anim.SetTrigger("Jump");
    }
}
