using UnityEngine;
using System.Collections;
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

		[Tooltip("Events that occur when object returns to original position after chase.")]
		public UnityEvent OnReturn;


		private GameObject returnPoint;
		private bool isReturning;


		void OnEnable () {
			ai = GetComponent<IAstarAI>();
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
			if (isReturning)
			{
				if ((transform.position - returnPoint.transform.position).magnitude < 0.01f)
				{
					OnDisable();
					isReturning = false;
					Destroy(returnPoint);
					OnReturn.Invoke();
				}
			}
		}

		public void SetTarget(Transform newTarget)
		{
			CreateReturnPoint();
			target = newTarget;
		}

		public void ReturnToPath()
		{
			isReturning = true;
			target = returnPoint.transform;
		}

		private void CreateReturnPoint()
		{
			returnPoint = new GameObject("ReturnPoint");
			returnPoint.transform.SetPositionAndRotation(
				new Vector3(
					Mathf.Floor(transform.position.x) + 0.5f,
					Mathf.Floor(transform.position.y) + 0.5f), 
				transform.rotation);
		}
		
	}
}
