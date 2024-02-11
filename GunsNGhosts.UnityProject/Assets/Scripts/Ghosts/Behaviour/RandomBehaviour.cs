using UnityEngine;
using Pathfinding;

namespace GunsNGhosts.Ghosts
{
	public class RandomBehaviour : ChasingBehaviour
	{
		Transform playerTransform = null;
		Transform point;
		GridNode[] nodes = { };


		protected override void Start()
		{
			point = new GameObject(transform.name + ".Target").transform;
			point.parent = transform.parent;
			target = point;

			Player player = ReferenceProvider.GetReference<Player>();
			playerTransform = (player != null) ? player.Transform : transform;

			AstarPath astar = AstarPath.active;
			nodes = astar.data.gridGraph.nodes;
			FindNewTarget();
		}

		private void Update()
		{
			if (point == null)
				return;

			Vector3 dir = transform.position - point.position;
			if (dir.sqrMagnitude <= Mathf.Pow(1, 2))
			{
				FindNewTarget();
			}
		}

		/// <summary> Set a new random target for the Ghost. </summary>
		public void FindNewTarget()
		{
			if (nodes.Length > 0)
			{
				int i = Random.Range(0, nodes.Length);
				point.position = (Vector3)nodes[i].position;
			}
			else
			{
				point.position = playerTransform.position;
			}
		}
	}
}
