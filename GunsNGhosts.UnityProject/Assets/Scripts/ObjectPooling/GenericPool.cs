using System.Collections.Generic;

namespace LucasSerrano.Pooling
{
	/// <summary> Base class for pools that use to a delegate to create its elements. </summary>
	public abstract class GenericPool<T> : IPool<T>
	{
		/// <summary> Delegate defining a function to create new elements for the pool. </summary>
		public delegate T CreateElementDelegate();
		/// <summary> Actual function that will be called when creating new elements for the pool. </summary>
		protected CreateElementDelegate createElementAction = null;

		/// <summary> List of elements that will be returned one at a time. </summary>
		protected Queue<T> elements = new Queue<T>();

		/// <summary> Number of elements that the pool will have. </summary>
		protected int size = 0;


		// -----------------------------------------------------------------
		#region Pooling

		public void Initialize()
		{
			if (createElementAction == null || size == 0)
				return;

			// Create the elements of this pool.
			for (int i = 0; i < size; i++)
			{
				elements.Enqueue(createElementAction());
			}
		}

		public abstract T Get();

		#endregion


		// -----------------------------------------------------------------
		#region Properties

		/// <summary> Actual function that will be called when creating new elements for the pool. </summary>
		public CreateElementDelegate CreateElementAction
		{
			protected get => createElementAction;
			set => createElementAction = value;
		}

		#endregion
	}
}
