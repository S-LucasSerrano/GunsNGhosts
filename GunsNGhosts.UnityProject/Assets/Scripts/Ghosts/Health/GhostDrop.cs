using UnityEngine;
using GunsNGhosts.Guns;
using GunsNGhosts.CharacterController;
using GunsNGhosts.Damage;


namespace GunsNGhosts.Ghosts
{
	public class GhostDrop : MonoBehaviour, IRequire<Ghost>
	{
		[Space] [SerializeField] int points = 3;
		[SerializeField][Range(0f,360)] float angleRandomization = 15;
		[SerializeField] float positionRandomization = 0.1f;

		/// Chances of dropping an ammo package when the player has full ammo vs when they has none.
		[Space] [SerializeField] [Range(0,1)] float ammoPackChanceAtFull = 0;
		[SerializeField] [Range(0, 1)] float ammoPackChanceAtZero = 1;
		GunShooter shooter = null;
		/// Chances of dropping a health package when the player is at full health vs when it has none.
		[Space] [SerializeField] [Range(0, 1)] float hpPackChanceAtFull = 0;
		[SerializeField] [Range(0, 1)] float hpPackChanceAtZero = 1;
		PlayerHealth playerHealth = null;


		// ---------------------------------------------------------------

		public void SetRequirement(Ghost requirement)
		{
			requirement.Health.OnDeath.AddListener(OnGhostDeath);
		}

		private void Start()
		{
			shooter = ReferenceProvider.GetReference<Player>().Shooter;
			playerHealth = ReferenceProvider.GetReference<Player>().Health;
		}


		// ---------------------------------------------------------------

		Vector3 _sourceDir = new Vector3();

		void OnGhostDeath(Component source)
		{
			if (source != null)
				_sourceDir = transform.position - source.transform.position;
			else
				_sourceDir = Vector3.zero;

			// Spawn the score points.
			for (int i = 0; i < points; i++)
			{
				DropProjectile("scorePoint");
			}

			// Randomly spawn a Ammo package.
			float randomNum = Random.Range(0f, 1f);
			float chances = Utilities.Math.Remap( shooter.Ammo, 0, shooter.MaxAmmo, ammoPackChanceAtZero, ammoPackChanceAtFull );
			// Spawn a package for sure if the player has no ammo an there is no other packages in the scene.
			bool dropForSure = (FindObjectsOfType<AmmoPackage>(false).Length <= 0 )
				&& shooter.Ammo <= 5;
			if (randomNum < chances || dropForSure)
			{
				DropProjectile("package.ammo");
				return;
			}

			// Randomly spawn a Health package.
			chances = Utilities.Math.Remap(playerHealth.CurrentHealth, 0, playerHealth.MaxHealth, hpPackChanceAtZero, hpPackChanceAtFull);
			if (randomNum < chances)
			{
				DropProjectile("package.health");
			}
		}

		void DropProjectile(string poolId)
		{
			// Get the point from the pool.
			GameObject gameObj = Game.GetElementFromPool(poolId);
			if (gameObj == null)
				return;
			// Place the point in the Ghost postiion.
			gameObj.transform.position = transform.position;
			gameObj.transform.Translate(
				Random.Range(-positionRandomization, positionRandomization),
				Random.Range(-positionRandomization, positionRandomization),
				0);

			// Find the projectile component and shoot it a random angle.
			Projectile pointProjectile = gameObj.GetComponent<Projectile>();
			if (pointProjectile == null)
				return;

			pointProjectile.transform.rotation = Quaternion.identity;

			// Shoot the drop in a random angle within the range.
			Vector3 shootingDir = _sourceDir;
			float randomAngle = Random.Range(-angleRandomization/2, angleRandomization/2);
			shootingDir = Quaternion.AngleAxis( randomAngle, Vector3.forward ) * shootingDir;

			pointProjectile.Shoot(shootingDir);
		}
	}
}
