using System.Collections;
using UnityEngine;

namespace GunsNGhosts.Guns
{
	public class BurstGun : SimpleGun
	{
		/// <summary> Number of bullets in the burst. </summary>
		[Space] [Header("   Burst")] [SerializeField] int burst = 3;
		/// <summary> Time between shots in the burst. </summary>
		[SerializeField] float timeBeteweenBullets = 0.1f;

		Coroutine burstRoutine = null;


		public override void Shoot()
		{
			// Start the shooting routine.
			if (burstRoutine != null) StopCoroutine(burstRoutine);
			burstRoutine = StartCoroutine(BurstRoutine());
		}

		IEnumerator BurstRoutine()
		{
			for (int i = 0; i < burst; i++)
			{
				base.Shoot();
				yield return new WaitForSeconds(timeBeteweenBullets);
			}
		}
	}
}
