#if AMPLIFY_SHADER_EDITOR
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class NormalOSPortASE : Vector3InputPortASE
	{
		private const string NOT_CONNECTED_CODE = "normalOS";

		public NormalOSPortASE(InputPort inputPort, Vector3 defaultValue) : base(inputPort, defaultValue)
		{
		}

		public override string GenerateNotConnectedCode(ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			return NOT_CONNECTED_CODE;
		}
	}
}
#endif