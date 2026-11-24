using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class FourDirTopDown : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("The players movement speed")]
    [SerializeField] private float speed;

    public bool controlEnabled { get; set; } = true;

    private Vector2 moveInput;
    private Vector2 inputDirection;
    private string animationDirection;
    
    private Rigidbody2D rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    // Update handles the animation logic.
    void Update()
    {
        
    }

    // Fixed update handles movement of the Rigidbody2D
    private void FixedUpdate()
    {
        if (controlEnabled)
        {
            rb.linearVelocity = inputDirection * speed;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().normalized;
        inputDirection = GetDirection(moveInput);
    }

    private Vector2 GetDirection(Vector2 input)
    {
        Vector2 moveDirection = Vector2.zero;
        //Up
        if (moveInput.y > 0)
        {
            animationDirection = "Up";
            moveDirection = new Vector2(0, 1);
        }
        //Down
        else if (moveInput.y < 0)
        {
            animationDirection = "Down";
            moveDirection = new Vector2(0, -1);
        }
        //Right
        else if (moveInput.x > 0)
        {
            animationDirection = "Right";
            moveDirection = new Vector2(1, 0);
        }
        //Left
        else if (moveInput.x < 0)
        {
            animationDirection = "Left";
            moveDirection = new Vector2(-1, 0);
        }

        return moveDirection;
    }
}
