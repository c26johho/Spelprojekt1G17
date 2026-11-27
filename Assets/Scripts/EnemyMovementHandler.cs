using System;
using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(RailMovement),typeof(AILerp), typeof(AIDestinationSetter))]
public class EnemyMovementHandler : MonoBehaviour
{
    public RailMovement railM { get; private set; }
    public AILerp aiLerp { get; private set; }
    public  AIDestinationSetter aiDest { get; private set; }
    

    public Pathfinding pathfinding;
    public Rail railMovement;
    private void Awake()
    {
        railM = GetComponent<RailMovement>();
        aiLerp = GetComponent<AILerp>();
        aiDest = GetComponent<AIDestinationSetter>();
        aiLerp.enabled = false;
        railM.path = railMovement.path;
    }

    private void Update()
    {
        if (railM.active)
        {
            railM.speed = railMovement.speed;
        }
        if (aiDest.isReturning)
        {
            aiLerp.speed = pathfinding.returnSpeed;
        }
        else aiLerp.speed = pathfinding.chaseSpeed;
    }

    public void SetTarget(Transform newTarget)
    {
        ActivatePathfinding();
        CreateReturnPoint();
        aiDest.target = newTarget;
    }
    private void CreateReturnPoint()
    {
        if (aiDest.returnPoint != null) return;
        aiDest.returnPoint = new GameObject("ReturnPoint_" + this.gameObject.name);
        aiDest.returnPoint.transform.SetPositionAndRotation(
            new Vector3(
                Mathf.Floor(transform.position.x) + 0.5f,
                Mathf.Floor(transform.position.y) + 0.5f), 
            transform.rotation);
    }
    public void ReturnToPath()
    {
        aiDest.isReturning = true;
        SetTarget(aiDest.returnPoint.transform);
    }
    public void Returned()
    {
        aiDest.isReturning = false;
        ActivateRailMovement();
        Destroy(aiDest.returnPoint);
    }

    private void ActivatePathfinding()
    {
        aiLerp.enabled = true;
        railM.active = false;
        railM.hasReturned = false;
    }
    private void ActivateRailMovement()
    {
        aiLerp.enabled = false;
        railM.active = true;
        railM.hasReturned = true;
    }

    
    [Serializable]
    public class Pathfinding
    {
        [SerializeField, Min(0)] public float chaseSpeed;
        [SerializeField, Min(0)] public float returnSpeed;
    }

    [Serializable]
    public class Rail
    {
        [Tooltip("The speed which the object moves along the path. Speed recommendations 0-15")]
        [SerializeField, Min(0)] public float speed;
        [Tooltip("The path that the object moves along. Has to be an object with the LineRenderer component")]
        [SerializeField] public LineRenderer path;
    }
    
}

