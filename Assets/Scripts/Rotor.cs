namespace Secrets.Gameplay
{
	using UnityEngine;
	public class Rotor : MonoBehaviour
	{
		[SerializeField] private Vector3 speed;
		[SerializeField] private Space relativeTo = Space.Self;

		void Update()
		{
			transform.Rotate(speed * Time.deltaTime, relativeTo);
		}
	}
}
