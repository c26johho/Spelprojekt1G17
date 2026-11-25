using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class LightCone : MonoBehaviour
{
    [SerializeField] private float rayDistance;
    private GameObject player;
    private EnemyMovement movement;
    private float facingDirection;
    private bool hasLineOfSight = false;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
    }

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
        for (int i = -15; i <= 15; i++)
        {
            float degree = (i + facingDirection) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(degree), Mathf.Sin(degree)) * rayDistance;
            RaycastHit2D ray = Physics2D.Raycast(transform.position, direction);
            if (ray.collider != null)
            {
                hasLineOfSight = ray.collider.CompareTag("Player");
                
                if (hasLineOfSight && (player.transform.position - transform.position).magnitude < rayDistance)
                {
                    Debug.DrawRay(transform.position, direction, Color.green);
                }
                else
                {
                    Debug.DrawRay(transform.position, direction, Color.red);
                }
            }
        }
    }

    private void UpdateFacingDir()
    {
        switch (movement.faceDir)
        {
            case EnemyMovement.FacingDirection.Right :
                facingDirection = 0;
                break;
            case EnemyMovement.FacingDirection.Left :
                facingDirection = 180;
                break;
            case EnemyMovement.FacingDirection.Up :
                facingDirection = 90;
                break;
            case EnemyMovement.FacingDirection.Down :
                facingDirection = 270;
                break;
        }
    }
}
