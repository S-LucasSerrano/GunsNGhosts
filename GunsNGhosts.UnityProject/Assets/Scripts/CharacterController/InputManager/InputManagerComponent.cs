using UnityEngine;

namespace LucasSerrano.Input
{
	/// <summary> Base class for InputManagers. </summary>
	public abstract class InputManagerComponent : MonoBehaviour
	{
		/// <summary> Returns the value of the input axis with the given name. </summary>
		public abstract float GetAxis(string name);

		/// <summary> Returns TRUE while the button with the given name is pressed. </summary>
		public abstract bool GetButton(string name);

		/// <summary> Returns TRUE the frame the button with the given name is pressed. </summary>
		public abstract bool GetButtonDown(string name);

		/// <summary> Returns TRUE the frame the button with the given name is released. </summary>
		public abstract bool GetButtonUp(string name);
	}
}
