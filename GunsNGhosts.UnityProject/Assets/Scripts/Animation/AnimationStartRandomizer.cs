using UnityEngine;

/// <summary> Component playes an animation starting at a random time. </summary>
public class AnimationStartRandomizer : MonoBehaviour
{
	/// <summary> Animator where the randomized animation is going to be played. </summary>
	[SerializeField] Animator animator = null;
	/// <summary> Name of the animation graph this is going to play in the Animator. </summary>
	[SerializeField] string stateName = "Idle";
	/// <summary> Layer of the animator where the animation is played. </summary>
	[SerializeField] int layer = 0;


	private void Reset()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		float randomTime = Random.Range(0f, 1f);
		animator.Play(stateName, layer, randomTime);
	}
}
