#if AMPLIFY_SHADER_EDITOR
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class TangentWSPortASE : Vector3InputPortASE
	{
		private const string NOT_CONNECTED_CODE = "tangentWS";

		public TangentWSPortASE(InputPort inputPort, Vector3 defaultValue) : base(inputPort, defaultValue)
		{
		}



		public override string GenerateNotConnectedCode(ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			return NOT_CONNECTED_CODE;
		}
	}
}
#endif