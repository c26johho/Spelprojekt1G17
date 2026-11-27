using System;
using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(RailMovement),typeof(AILerp), typeof(AIDestinationSetter))]
public class EnemyMovementHandler : MonoBehaviour
{
    private RailMovement railM;
    private AILerp aiLerp;
    private AIDestinationSetter aiDest;
    
    private GameObject returnPoint;
    private bool isReturning;

    public Pathfinding pathfinding;
    public Rail railMovement;
    private void Awake()
    {
        railM = GetComponent<RailMovement>();
        aiLerp = GetComponent<AILerp>();
        aiDest = GetComponent<AIDestinationSetter>();
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

    /*public void SetTarget(Transform newTarget)
    {
        aiLerp.enabled = true;
        CreateReturnPoint();
        aiDest.target = newTarget;
    }
    private void CreateReturnPoint()
    {
        if (returnPoint != null) return;
        returnPoint = new GameObject("ReturnPoint_" + this.name);
        returnPoint.transform.SetPositionAndRotation(
            new Vector3(
                Mathf.Floor(transform.position.x) + 0.5f,
                Mathf.Floor(transform.position.y) + 0.5f), 
            transform.rotation);
    }
    public void ReturnToPath()
    {
        isReturning = true;
        aiDest.target = returnPoint.transform;
    }
    public void Returned()
    {
        isReturning = false;
        aiLerp.enabled = false;
        Destroy(returnPoint);
    }*/

    public void ChangeMovementStyle()
    {
        if (aiLerp.enabled)
        {
            aiLerp.enabled = false;
            railM.active = true;
        }
        else
        {
            aiLerp.enabled = true;
            railM.active = false;
        }
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
    }
    
}

