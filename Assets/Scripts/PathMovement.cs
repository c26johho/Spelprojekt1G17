using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PathMovement : MonoBehaviour
{
    [Tooltip("The path that the object moves along. Has to be an object with the LineRenderer component")]
    [SerializeField] private LineRenderer path;
    [Tooltip("The speed which the object moves along the path. Speed recommendations 0-15")]
    [SerializeField] private float speed = 1;
    
    public FacingDirection faceDir { get; private set; }
    
    private int index;
    private Transform tf;
    private float timer;
    private bool waiting, moving, active, movingForwards;
    private bool right, left, up, down;
    private Animator animator;
    /// <summary>
    /// On awake, set the objects transform to the first vertex of the path. Setting movingForwards, moving and active
    /// to true means that the object starts to move on the first frame and continues until told otherwise.
    /// </summary>
    private void Awake()
    {
        tf = GetComponent<Transform>();
        index = 0;
        tf.position = GetVertex(index);
        movingForwards = true;
        moving = true;
        active = true;
    }
    /// <summary>
    /// Every frame, check if active = true. If so, check if moving = true. If both are true, move the object according
    /// to the function Move(). If moving = false, it means the object is currently still and should remain still until
    /// the timer "timer" reaches 0. When it does, set "moving" to true again and the object will continue along its
    /// path.
    /// </summary>
    void Update()
    {
        if (active)
        {
            if (moving)
            {
                Move();
            }
            else
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                    moving = true;
            }
        }
        /*animator.SetFloat("velocityX", velocity.x);
        animator.SetFloat("velocityY", velocity.y);*/
    }
    /// <summary>
    /// Uses the GetDir() method to get the direction the object is supposed to move along.
    /// The function constantly moves the object towards the next vertex and calculates the remaining distance.
    /// </summary>
    private void Move()
    {
        Vector3 velocity = GetDir() * speed;
        GetFacingDirection(velocity);
        float movementThisFrame = (velocity * Time.deltaTime).magnitude;
        if (movingForwards)
        {
            transform.position = Vector2.MoveTowards(transform.position, GetVertex(index + 1), movementThisFrame);
            float distanceRemaining = (GetVertex(index + 1) - transform.position).magnitude;
            UpdateForwardsPatrol(distanceRemaining);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, GetVertex(index - 1), movementThisFrame);
            float distanceRemaining = (GetVertex(index - 1) - transform.position).magnitude;
            UpdateBackwardsPatrol(distanceRemaining);
        }
    }
    /// <summary>
    /// This function gets called whenever the object is moving towards the end of the path.
    /// If the distance between the object and the target are less than 0.01, increase the index of the vertex so that
    /// the object starts targeting the next vertex along the path.
    /// Initiates the wait time that is set to the specific vertex.
    /// If the vertex is the end of the path, set movingForwards to false, meaning the object starts to reverse
    /// along the path. 
    /// </summary>
    /// <param name="distRem"></param>
    private void UpdateForwardsPatrol(float distRem)
    {
        if (distRem < 0.01f)
        {
            index++;
            InitiateWait(index);
            if (index + 1 >= path.positionCount)
            {
                index = path.positionCount - 1;
                movingForwards = false;
            }
        }
    }
    /// <summary>
    /// This function gets called whenever the object is moving towards the start of the path.
    /// If the distance between the object and the target are less than 0.01, decrease the index of the vertex so that
    /// the object starts targeting the next vertex along the path.
    /// Initiates the wait time that is set to the specific vertex.
    /// If the vertex is the start of the path, set movingForwards to true, meaning the object starts heading forwards
    /// along the path. 
    /// </summary>
    /// <param name="distRem"></param>
    private void UpdateBackwardsPatrol(float distRem)
    {
        if (distRem < 0.01f)
        {
            index--;
            InitiateWait(index);
            if (index <= 0)
            {
                index = 0;
                movingForwards = true;
            }

        }
    }
    
    /// <summary>
    /// Reads the z-value of the vertex Vector3 and sets the timer to that value.
    /// Sets moving = false to stop further movement.
    /// </summary>
    /// <param name="waitIndex"></param>
    private void InitiateWait(int waitIndex)
    {
        float waitTime = GetVertex(waitIndex).z;
        if (waitTime > 0)
        {
            timer = waitTime;
            moving = false;
        }
    }
    /// <summary>
    /// Returns the Vector3 of the vertex at a specific index.
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    private Vector3 GetVertex(int idx)
    {
        return path.GetPosition(idx);
    }
    /// <summary>
    /// Calculates the direction the object is heading by neutralizing the z-component of the Vector3 and normalizing
    /// the difference between the next vertex and the current/last visited. 
    /// </summary>
    /// <returns></returns>
    private Vector3 GetDir()
    {
        Vector3 nextVertex;
        Vector3 currentVertex;
        if (movingForwards)
        {
            nextVertex = GetVertex(index + 1);
            currentVertex = GetVertex(index);
            nextVertex.z = 0;
            currentVertex.z = 0;
            return (nextVertex - currentVertex).normalized;
        }

        nextVertex = GetVertex(index - 1);
        currentVertex = GetVertex(index);
        nextVertex.z = 0;
        currentVertex.z = 0;
        return (nextVertex - currentVertex).normalized;
    }
    /// <summary>
    /// Sets active = false in order to stop any further motion and save all information for later continuance.
    /// </summary>
    public void PauseMovement()
    {
        active = false;
    }

    /// <summary>
    /// Sets active = true to continue movement.
    /// </summary>
    public void ResumeMovement()
    {
        active = true;
    }
    
    /// <summary>
    /// Saves the direction the object is moving towards as a variable that can be used in other applications.
    /// </summary>
    /// <param name="dir"></param>
    private void GetFacingDirection(Vector3 dir)
    {
        if (dir.x > 0)
        {
            faceDir = FacingDirection.Right;
        }
        else if (dir.x < 0)
        {
            faceDir = FacingDirection.Left;
        }
        else if (dir.y > 0)
        {
            faceDir = FacingDirection.Up;
        }
        else
        {
            faceDir = FacingDirection.Down;
        }
    }
} 
public enum FacingDirection
{
    Up,
    Down,
    Left,
    Right
}