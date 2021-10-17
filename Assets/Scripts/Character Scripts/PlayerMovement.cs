using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private CharacterStats cStats;
    private Rigidbody2D rb;
    private Animator animator;

    public bool isPlayer;
    private float moveSpeed;

    Vector2 movement;

    public bool ReadyToExecute = false;
    public bool wasToRight = false;
    public bool done = false;


    void Start()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        this.cStats = GetComponent<CharacterStats>();
        isPlayer = cStats.getAiStatus();
        moveSpeed = cStats.getSpeed();
    }

    void Update() // Updates 1x per frame, use this to register input
    {
        moveSpeed = cStats.getSpeed();

        // Get user input
        movement.x = Input.GetAxisRaw("Horizontal"); // left/right input
        movement.y = Input.GetAxisRaw("Vertical"); // up/down input

        // update animator
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
        
    }


    void FixedUpdate() // Updates ~50x a second
    {

        // Disable diagonal movement & prioritize direction of last key pressed
        bool isMovingUp = movement.x < -0.5f;
        bool isMovingDown = movement.x > 0.5f;
        bool isMovingLeft = movement.y < -0.5f;
        bool isMovingRight = movement.y > 0.5f;

        if ((isMovingUp || isMovingDown) && (isMovingLeft || isMovingRight))
        {
            if (isMovingUp || isMovingDown) { movement.y = 0; }
            else { movement.x = 0; }
        }
        else if (isMovingLeft)
            movement.x = 0;
        else if (isMovingRight)
            movement.x = 0;
        else if (isMovingUp)
            movement.y = 0;
        else if (isMovingDown)
            movement.y = 0;
        else
        {
            movement.x = 0;
            movement.y = 0;
        }

        // move rigidbody
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime); // deltatime = amount of time since function last called
    }
}
