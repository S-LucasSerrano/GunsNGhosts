using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LucasSerrano.Pooling;

namespace GunsNGhosts
{
	/// <summary> Hub with public references to some key components of the game. </summary>
	public class Game : MonoBehaviour, IReferenceProvider
	{
		static Game instance = null;        // Singleton.

		[Space] [SerializeField] List<IdComponent> references = new List<IdComponent>();
		[Space] [SerializeField] List<IdPool> pools = new();


		// ----------------------------------------------------------------------

		public void Awake()
		{
			if (instance == null) instance = this;
			if (IReferenceProvider.Instance == null) IReferenceProvider.Instance = this;

			// Initialize the game's object pools.
			foreach (IdPool pool in pools)
				pool.Initialize();
		}


		// ----------------------------------------------------------------------
		#region Properties

		/// <summary> Instance of the Game in the scene. </summary>
		public static Game Instance => instance;

		#endregion


		// ----------------------------------------------------------------------
		#region Get References

		/// <summary> Returns the first reference in this manager to a component of the given type. </summary>
		public T GetReference<T>() where T : Component
		{
			if (instance == null)
				return null;

			foreach(IdComponent reference in instance.references)
			{
				if (typeof(T).IsAssignableFrom( reference.component.GetType() ))
					return reference.component as T;
			}
			return null;
		}

		/// <summary> Returns the first reference in this manager to a component of the given type and id. </summary>
		public T GetReference<T>(string id) where T : Component
		{
			if (instance == null)
				return null;

			foreach (IdComponent reference in instance.references)
			{
				if (reference.id == id && reference.component.GetType() == typeof(T))
					return reference.component as T;
			}
			return null;
		}

		/// <summary> Returns a list with all the references in this manager to the components of the given type. </summary>
		public List<T> GetReferences<T>() where T : Component
		{
			List<T> list = new List<T>();

			if (instance == null)
				return list;

			foreach (IdComponent reference in instance.references)
			{
				if (reference.component.GetType() == typeof(T))
					list.Add(reference.component as T);
			}
			return list;
		}

		#endregion

		#region IdComponent Definition

		/// <summary> A class that links an id whith a reference to a Unity Component. </summary>
		[System.Serializable]
		public class IdComponent
		{
			public string id = "";
			public Component component = null;
		}

		#endregion


		// ----------------------------------------------------------------------
		#region Get Pools

		/// <summary> Returns the next element of to the pool with the given id. </summary>
		public static GameObject GetElementFromPool(string poolId)
		{
			if (instance == null)
				return null;

			foreach (IdPool pool in instance.pools)
			{
				if (pool.id == poolId)
				{
					GameObject element = pool.pool.Get();

					// If the element is spawneable, call its respawn function.
					ISpawneable[] components = element.GetComponents<ISpawneable>();
					foreach (ISpawneable component in components)
						component.Respawn();

					return element;
				}
			}

			return null;
		}

		#endregion

		#region IdPool Definition

		/// <summary> Pool assigned to an id. </summary>
		[System.Serializable]
		public class IdPool
		{
			/// <summary> Id of the pool. </summary>
			public string id = "";
			/// <summary> Each element of the pool is a copy of this GameObject. </summary>
			public GameObject prefab = null;
			/// <summary> Actual pool of objects. </summary>
			public IPool<GameObject> pool = null;
			/// <summary> Default size of the pool. </summary>
			public int size = 10;

			/// <summary> Transform that will be the parent of all the pool GameObjects. </summary>
			Transform parent = null;


			/// <summary> Initialize the pool. </summary>
			public void Initialize()
			{
				// Create the parent for the elements of the pool.
				parent = new GameObject(id).transform;
				parent.parent = (instance == null) ? null : instance.transform;

				// If the prefab has a pooleable component, we create a dynamic pool.
				// If not, a fixed pool.
				IPooleable pooleableComponent = null;
				if (prefab.TryGetComponent<IPooleable>(out pooleableComponent))
					pool = new DynamicPool<GameObject>(CreateElement, size);
				else
					pool = new FixedPool<GameObject>(CreateElement, size);

				pool.Initialize();
			}

			/// <summary> Instantiate a new GameObject for the pool. </summary>
			GameObject CreateElement()
			{
				GameObject element = Instantiate(prefab, parent);
				element.SetActive(false);

				// In the case we created a dynamic pool, we give a reference to the pool to each component in the newly created element.
				if (pool.GetType() == typeof(DynamicPool<GameObject>))
				{
					IPooleable[] poolElementComponents = element.GetComponents<IPooleable>();
					foreach (IPooleable poolElementComponent in poolElementComponents)
						poolElementComponent.Pool = pool as DynamicPool<GameObject>;
				}

				return element;
			}

			IEnumerator TurnOffPoolElementRoutine(GameObject element)
			{
				yield return null;
				element.SetActive(false);
			}
		}

		#endregion
	}
}
