using System.Collections.Generic;

namespace LucasSerrano.Pooling
{
	/// <summary> Basic pool of objects. <para></para>
	/// Creates new objets when there are no objects to return. </summary>
	public class Pool<T> : IPool<T>
	{
		/// <summary> Delegate defining a function that creates a new element of the pool. </summary>
		public delegate T CreateElement();
		/// <summary> Actual function that will be called when creating new elements for the pool. </summary>
		CreateElement createElementAction = null;

		/// <summary> List of elements from the pool that are avialable to use. </summary>
		Queue<T> freeElements = new Queue<T>();
		/// <summary> List of elements from the pool that currelty in use, and unavialable to be returned. </summary>
		List<T> usedElements = new List<T>();


		// ----------------------------------------------------------------------
		#region Constructor

		/// <param name="createElementAction"> Function called when creating a new element for the pool. </param>
		/// <param name="defaultLength"> Number of elements that will be created in the pool by default. </param>
		public Pool(CreateElement createElementAction = null, int defaultLength = 0)
		{
			if (createElementAction == null)
				return;
			this.createElementAction = createElementAction;

			if (defaultLength <= 0)
				return;
			for (int i = 0; i < defaultLength; i++)
			{
				freeElements.Enqueue( createElementAction() );
			}

		}

		#endregion


		// ----------------------------------------------------------------------
		#region Pooling

		/// <summary> Get the next element of the pool. <para></para>
		/// Elements need to be returned to the pool using the Release() function before they can be used again. </summary>
		public T Get()
		{
			T element;

			// If there are free elements in the pool, return the first one.
			// If not, create a new one.
			if ( freeElements.Count > 0 )
				element = freeElements.Dequeue();
			else
				element = createElementAction();
				
			// Add the element to the used list and return it.
			usedElements.Add(element);
			return element;
		}

		/// <summary> Return an element to the pool so it can be used again. </summary>
		public void Release(T element)
		{
			if (!usedElements.Contains(element))
				return;

			freeElements.Enqueue(element);
			usedElements.Remove(element);
		}

		#endregion


		// ----------------------------------------------------------------------
		#region Properties

		/// <summary> Function called when creating a new element for the pool. </summary>
		public CreateElement CreateElementAction
		{
			set => createElementAction = value;
		}

		#endregion
	}
}
