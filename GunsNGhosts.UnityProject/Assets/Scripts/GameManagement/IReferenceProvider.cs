using System.Collections.Generic;
using UnityEngine;

namespace GunsNGhosts
{
	/// <summary> Interface for classes that will act as hub of references to key components of the game. </summary>
	public interface IReferenceProvider
	{
		/// <summary> Instance of the reference provider. </summary>
		public static IReferenceProvider Instance { get; protected set; }


		// ----------------------------------------------------------------------

		/// <summary> Returns the first reference in this provider to a component of the given type. </summary>
		public T GetReference<T>() where T : Component;

		/// <summary> Returns a list with all the references in this provider to components of the given type. </summary>
		public List<T> GetReferences<T>() where T : Component;
	}


	// =========================================================================

	/// <summary> Static class that allows to get references from the instance of the <see cref="IReferenceProvider"/>. </summary>
	public static class ReferenceProvider
	{
		/// <summary> Returns the first reference in the provider to a component of the given type. </summary>
		public static T GetReference<T>() where T : Component
		{
			if (IReferenceProvider.Instance == null)
				return null;
			return IReferenceProvider.Instance.GetReference<T>();
		}

		/// <summary> Returns a list with all the references in the provider to components of the given type. </summary>
		public static List<T> GetReferences<T>() where T : Component
		{
			if (IReferenceProvider.Instance == null)
				return new List<T>();
			return IReferenceProvider.Instance.GetReferences<T>();
		}
	}
}
