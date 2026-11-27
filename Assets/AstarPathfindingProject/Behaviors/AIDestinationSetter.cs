using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VectorGraphics;
using UnityEngine.Events;

namespace Pathfinding {
	/// <summary>
	/// Sets the destination of an AI to the position of a specified object.
	/// This component should be attached to a GameObject together with a movement script such as AIPath, RichAI or AILerp.
	/// This component will then make the AI move towards the <see cref="target"/> set on this component.
	///
	/// See: <see cref="Pathfinding.IAstarAI.destination"/>
	///
	/// [Open online documentation to see images]
	/// </summary>
	[UniqueComponent(tag = "ai.destination")]
	[HelpURL("https://arongranberg.com/astar/docs/class_pathfinding_1_1_a_i_destination_setter.php")]
	
	public class AIDestinationSetter : VersionedMonoBehaviour
	{
		/// <summary>The object that the AI should move to</summary>
		public Transform target;
		IAstarAI ai;

		//JohanEdit
		[Tooltip("Events that occur when object returns to original position after chase.")]
		public UnityEvent onReturn;
		private GameObject returnPoint;
		private AILerp aiLerp;
		private bool isReturning;
		public float dirToDeg { get; private set; }


		void OnEnable () {
			ai = GetComponent<IAstarAI>();
			aiLerp = GetComponent<AILerp>();
			// Update the destination right before searching for a path as well.
			// This is enough in theory, but this script will also update the destination every
			// frame as the destination is used for debugging and may be used for other things by other
			// scripts as well. So it makes sense that it is up to date every frame.
			if (ai != null) ai.onSearchPath += Update;
		}

		void OnDisable () {
			if (ai != null) ai.onSearchPath -= Update;
		}

		/// <summary>Updates the AI's destination every frame</summary>
		void Update () {
			if (target != null && ai != null) ai.destination = target.position;
			//JohanEdit
			if (isReturning)
			{
				if ((transform.position - returnPoint.transform.position).magnitude < 0.01f)
				{
					OnDisable();
					onReturn.Invoke();
				}
			}
			GetFacingDirection();
		}
		
		//JohanEdit everything below this point
		public void SetTarget(Transform newTarget)
		{
			aiLerp.enabled = true;
			CreateReturnPoint();
			target = newTarget;
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
			target = returnPoint.transform;
		}
		public void Returned()
		{
			isReturning = false;
			aiLerp.enabled = false;
			Destroy(returnPoint);
		}
		private void GetFacingDirection()
		{
			Vector2 dir = ai.desiredVelocity / ai.maxSpeed;
			Vector2 roundedDir = new Vector2(
				Mathf.Round(dir.x),
				Mathf.Round(dir.y));
			if (dir.magnitude > 1) return;
			if (roundedDir.x != 0)
			{
				switch (roundedDir.x)
				{
					case 1 :
						dirToDeg = -90;
						break;
					case -1 :
						dirToDeg = 90;
						break;
				}
			}
			else
			{
				switch (roundedDir.y)
				{
					case 1 :
						dirToDeg = 0;
						break;
					case -1 :
						dirToDeg = 180;
						break;
				}
			}
		}
	}
}
