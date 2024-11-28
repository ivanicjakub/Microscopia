using UnityEngine;
namespace Secrets.Gameplay
{
	public class Billboard : MonoBehaviour
	{
		[SerializeField] private bool onlyYaw;
		[SerializeField] private bool directLookAt;
		[SerializeField] private Vector3 eulerOffset;
		[SerializeField] private float zOffset = 0f;
		Vector3 origLocalPosition;
		private Camera _camera;

		private void Awake()
		{
			_camera = Camera.main;
			origLocalPosition = transform.localPosition;
		}

		void LateUpdate()
		{
			if (_camera == null)
				_camera = Camera.main;

			if (_camera == null)
				return;

			if (onlyYaw)
			{
				Vector3 euler = Quaternion.LookRotation((_camera.transform.position - transform.position).normalized, Vector3.up).eulerAngles;
				euler.Scale(Vector3.up);
				euler += eulerOffset;
				transform.eulerAngles = euler;
			}
			else if (directLookAt)
			{
				transform.LookAt(_camera.transform.position);
			}
			else
			{
				transform.LookAt(transform.position + _camera.transform.forward);
			}

			if (zOffset != 0)
			{
				Vector3 position = transform.parent.TransformPoint(origLocalPosition);
				position += zOffset * (_camera.transform.position - position).normalized;
				transform.position = position;
			}
		}
	}
}