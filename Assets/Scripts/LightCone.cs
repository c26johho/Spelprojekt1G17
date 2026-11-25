using System;
using System.Collections.Generic;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class LightCone : MonoBehaviour
{
    [Tooltip("The distance the light travels")]
    [SerializeField] private float rayDistance;

    [Tooltip("The field of view angle")] 
    [SerializeField, Range(2f, 180f)] private int angle;
    [Tooltip("More rays = more performance but more accurate light")] 
    [SerializeField, Range(2, 200)] private int numberOfRays;
    [Tooltip("Insert an object with a EnemyMovement component, used to move an object along a set route")]
    [SerializeField] private EnemyMovement enemyMovement;
    [Tooltip("Insert an object with a AIDestinationSetter component, used for chasing other objects")]
    [SerializeField] private AIDestinationSetter destinationSetter;
    private GameObject player;
    private float facingDirection;
    private bool hasLineOfSight = false;
    private int layerId;
    [SerializeField] private LayerMask objMask;
    

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        UpdateFacingDir();
    }
    
    void FixedUpdate()
    {
        
        for (int i = -numberOfRays / 2; i <= numberOfRays / 2; i++)
        {
            float degree = (facingDirection + i) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(degree), Mathf.Sin(degree));
            RaycastHit2D ray = Physics2D.Raycast(
                transform.position,
                direction,
                rayDistance,
                objMask);
            if (ray.collider != null)
            {
                hasLineOfSight = ray.collider.CompareTag("Player");
                
                if (hasLineOfSight)
                {
                    Debug.DrawRay(transform.position, ray.point - (Vector2)transform.position, Color.green);
                }
                else
                {
                    Debug.DrawRay(transform.position, ray.point - (Vector2)transform.position, Color.red);
                }
            }
        }
    }

    private void UpdateFacingDir()
    {
        switch (enemyMovement.faceDir)
        {
            case FacingDirection.Right :
                facingDirection = 0;
                break;
            case FacingDirection.Left :
                facingDirection = 180;
                break;
            case FacingDirection.Up :
                facingDirection = 90;
                break;
            case FacingDirection.Down :
                facingDirection = 270;
                break;
        }
    }
}
