using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Tooltip("The path that the object moves along. Has to be an object with the LineRenderer component")]
    [SerializeField] private LineRenderer path;
    [Tooltip("The speed which the object moves along the path. Speed recommendations 0-15")]
    [SerializeField] private float speed = 1;
    
    private int index;
    private Transform tf;
    private float timer;
    private bool waiting, moving, active, movingForwards;
    private bool right, left, up, down;
    private Animator animator;
    public FacingDirection faceDir;
    
    private void Awake()
    {
        tf = GetComponent<Transform>();
        index = 0;
        tf.position = GetVertex(index);
        movingForwards = true;
        moving = true;
        active = true;
    }
    
    void Update()
    {
        if (active)
        {
            if (moving)
            {
                Vector3 velocity = GetDir() * speed;
                GetFacingDirection(velocity);
                transform.position += velocity * Time.deltaTime;
                if (movingForwards)
                {
                    float distanceRemaining = (GetVertex(index + 1) - transform.position).magnitude;
                    UpdateForwardsPatrol(distanceRemaining);
                }
                else
                {
                    float distanceRemaining = (GetVertex(index - 1) - transform.position).magnitude;
                    UpdateBackwardsPatrol(distanceRemaining);
                }
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

    private void InitiateWait(int waitIndex)
    {
        float waitTime = GetVertex(waitIndex).z;
        if (waitTime > 0)
        {
            timer = waitTime;
            moving = false;
        }
    }
    private Vector3 GetVertex(int point)
    {
        return path.GetPosition(point);
    }
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

    public void PauseMovement()
    {
        active = false;
    }

    public void ResumeMovement()
    {
        active = true;
    }

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