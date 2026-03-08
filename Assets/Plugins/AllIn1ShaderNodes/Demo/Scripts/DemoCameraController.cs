using UnityEngine;

namespace AllIn1ShaderNodes
{
	[ExecuteInEditMode]
	public class DemoCameraController : MonoBehaviour
	{
		public Camera cam;

		private void Awake()
		{
#if ALLIN13DSHADER_BIRP
			cam.depthTextureMode = DepthTextureMode.Depth;
#endif
		}
	}
}