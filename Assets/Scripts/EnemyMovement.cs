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
    private bool waiting;
    private float distanceTraveled;
    private bool movingForwards;
    private bool moving;
    private Vector3 velocity;
    private bool right, left, up, down;
    private Animator animator;
    private void Awake()
    {
        tf = GetComponent<Transform>();
        index = 0;
        tf.position = GetVertex(index);
        movingForwards = true;
        moving = true;
    }
    
    void Update()
    {
        if (moving)
        {
            Vector3 velocity = GetDir() * speed;
            transform.position += velocity * Time.deltaTime;
            distanceTraveled += (velocity * Time.deltaTime).magnitude;
            if (movingForwards)
            {
                UpdateForwardsPatrol();
            }
            else
            {
                UpdateBackwardsPatrol();
            }
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                moving = true;
        }
        /*animator.SetFloat("velocityX", velocity.x);
        animator.SetFloat("velocityY", velocity.y);*/
    }
    
    private void UpdateForwardsPatrol()
    {
        if (distanceTraveled > (GetVertex(index + 1) - GetVertex(index)).magnitude)
        {
            index++;
            InitiateWait(index);
            distanceTraveled = 0;
            if (index + 1 >= path.positionCount)
            {
                index = path.positionCount - 1;
                movingForwards = false;
            }

        }
    }
    private void UpdateBackwardsPatrol()
    {
        if (distanceTraveled > (GetVertex(index -1) - GetVertex(index)).magnitude)
        {
            index--;
            InitiateWait(index);
            distanceTraveled = 0;
            if (index <= 0)
            {
                index = 0;
                movingForwards = true;
            }

        }
    }

    private void InitiateWait(int index)
    {
        float waitTime = GetVertex(index).z;
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
        // TODO: Normalisera x och y istället för xyz
        if (movingForwards)
            return (GetVertex(index + 1) - GetVertex(index)).normalized;
        return (GetVertex(index - 1) - GetVertex(index)).normalized;
    }

    public void PauseMovement()
    {
        moving = false;
    }

    public void ResumeMovement()
    {
        moving = true;
    }
    
} 