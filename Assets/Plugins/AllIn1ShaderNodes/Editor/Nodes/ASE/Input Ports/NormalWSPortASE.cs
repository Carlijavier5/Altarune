#if AMPLIFY_SHADER_EDITOR
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class NormalWSPortASE : Vector3InputPortASE
	{
		private const string NOT_CONNECTED_CODE = "normalWS";

		public NormalWSPortASE(InputPort inputPort, Vector3 defaultValue) : base(inputPort, defaultValue)
		{
		}



		public override string GenerateNotConnectedCode(ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			return NOT_CONNECTED_CODE;
		}
	}
}
#endif