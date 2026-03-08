using UnityEngine;

namespace AllIn1ShaderNodes
{
	public class DemoExpositorMovement
	{
		private Transform transform;
		private bool isMoving;
		private bool disableOnFinish;

		private Vector3 srcPosition;
		private Vector3 dstPosition;
		private float lerpSpeed;

		public DemoExpositorMovement(Transform transform)
		{
			this.transform = transform;
		}

		public void StartMoving(Vector3 srcPosition, Vector3 dstPosition, float lerpSpeed, bool disableOnFinish)
		{
			this.srcPosition = srcPosition;
			this.dstPosition = dstPosition;
			this.lerpSpeed = lerpSpeed;

			this.isMoving = true;
			this.disableOnFinish = disableOnFinish;
		}

		public void Update(float deltaTime)
		{
			if (isMoving)
			{
				Move(deltaTime);
			}
		}

		private void Move(float deltaTime)
		{
			Vector3 newPos = Vector3.Lerp(transform.position, dstPosition, deltaTime * lerpSpeed);
			transform.position = newPos;

			if (disableOnFinish)
			{
				float dst = (transform.position - dstPosition).sqrMagnitude;
				if (dst <= 0.025f)
				{
					transform.gameObject.SetActive(false);
				}
			}
		}
	}
}