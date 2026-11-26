using System;
using System.Collections.Generic;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
public class LightCone : MonoBehaviour
{
    [Tooltip("The distance the light travels")]
    [SerializeField] private float rayDistance;
    
    [Tooltip("The field of view angle")] 
    [SerializeField, Range(2f, 360f)] private int angle;
    
    [Tooltip("More rays = more performance but more accurate light")] 
    [SerializeField, Range(2, 200)] private int numberOfRays;
    
    [Tooltip("Insert an object with a EnemyMovement component, used to move an object along a set route")]
    [SerializeField] private EnemyMovement enemyMovement;
    
    [Tooltip("Insert an object with a AIDestinationSetter component, used for chasing other objects")]
    [SerializeField] private AIDestinationSetter destinationSetter;
    
    [Tooltip("Specify which layers should be able to be seen by the light cone")]
    [SerializeField] private LayerMask objMask;
    
    private float facingDirection;
    private bool hasLineOfSight = false;
    private Light2D lightCone;

    private void Awake()
    {
        lightCone = GetComponent<Light2D>();
    }

    void Update()
    {
        UpdateFacingDir();
    }
    
    void FixedUpdate()
    {
        Vector3[] hits = new Vector3[numberOfRays + 1];
        Vector3 origin = Vector3.zero;
        hits[0] = origin;
        for (int i = 0; i < numberOfRays; i++)
        {
            float rad = (facingDirection - angle / 2f + angle / (numberOfRays - 1f) * i) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
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
                    Debug.DrawRay(transform.position, ray.point - (Vector2)transform.position, Color.darkOrange);
                }

                hits[i + 1] = ray.point - (Vector2)transform.position;
            }
            else
            {
                Debug.DrawRay(transform.position, direction * rayDistance, Color.red);
                hits[i + 1] = direction * rayDistance;
            }
        }

        lightCone.SetShapePath(hits);
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
