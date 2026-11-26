using System;
using Pathfinding;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class LightCone : MonoBehaviour
{
    [Tooltip("The distance the light travels")]
    [SerializeField] private float rayDistance;
    
    [Tooltip("The field of view angle")] 
    [SerializeField, Range(2f, 360f)] private int angle;
    
    [Tooltip("More rays = more performance but more accurate light")] 
    [SerializeField, Range(2,250)] private int numberOfRays;
    
    [Tooltip("Insert an object with a EnemyMovement component, used to move an object along a set route")]
    [SerializeField] private EnemyMovement enemyMovement;
    
    [Tooltip("Insert an object with a AIDestinationSetter component, used for chasing other objects")]
    [SerializeField] private AIDestinationSetter destinationSetter;
    
    [Tooltip("Specify which layers should be able to be seen by the light cone")]
    [SerializeField] private LayerMask objMask;

    [Tooltip("Insert Spot Light 2D from child object")]
    [SerializeField] private Light2D lightSource;

    private float facingDirection;
    private bool hasLineOfSight = false;
    public Vector3[] hits { get; private set; }
    private void Awake()
    {
        lightSource.pointLightOuterAngle = angle;
        lightSource.pointLightInnerAngle = angle / 3f;
    }

    void Update()
    {
        UpdateFacingDir();
        
    }
    
    void FixedUpdate()
    {
        hits = new Vector3[numberOfRays + 1];
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

        if (!Mathf.Approximately(lightSource.transform.rotation.z, facingDirection - 90))
        {
            lightSource.transform.rotation = Quaternion.Euler(0,0,facingDirection - 90);
        }
    }
}
