using UnityEngine;
using LucasSerrano.Input;

namespace GunsNGhosts
{
	/// <summary> Hub with public references to some key components of the game. </summary>
	public class Game : MonoBehaviour
	{
		static Game instance = null;        // Singleton.

		[Space] [SerializeField] InputManagerComponent inputManager = null;
		[Space] [SerializeField] Player player = null;
		[SerializeField] CameraShaker cameraShaker = null;
		[SerializeField] 



		// ----------------------------------------------------------------------

		public void Awake()
		{
			if (instance == null) instance = this;
		}


		// ----------------------------------------------------------------------
		#region Properties

		/// <summary> Instance of the Game in the scene. </summary>
		public static Game Instance => instance;

		public static InputManagerComponent InputManager
		{
			get
			{
				if (instance == null) return null;
				return instance.inputManager;
			}
		}
		public static Player Player
		{
			get
			{
				if (instance == null) return null;
				return instance.player;
			}
		}
		public static CameraShaker CameraShaker
		{
			get
			{
				if (instance == null) return null;
				return instance.cameraShaker;
			}

			set
			{
				if (instance == null) return;
				instance.cameraShaker = value;
			}
		}

		#endregion
	}
}
