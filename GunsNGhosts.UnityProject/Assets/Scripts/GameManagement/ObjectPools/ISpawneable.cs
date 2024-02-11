using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunsNGhosts
{
	/// <summary> Class for components in GameObjects that can be spawn by the Game manager. </summary>
	public interface ISpawneable
	{
		public void Respawn();
	}
}
