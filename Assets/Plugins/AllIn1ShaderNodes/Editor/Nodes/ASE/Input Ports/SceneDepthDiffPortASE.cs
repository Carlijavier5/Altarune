#if AMPLIFY_SHADER_EDITOR
using AllIn1ShaderNodes;

namespace AmplifyShaderEditor
{
	public class SceneDepthDiffPortASE : AbstractInputPortASE
	{
		private const string NOT_CONNECTED_CODE = "sceneDepthDiff";

		private float defaultValue;

		public SceneDepthDiffPortASE(float defaultValue) : base(null)
		{
			this.defaultValue = defaultValue;
		}

		public SceneDepthDiffPortASE(InputPort inputPort, float defaultValue) : base(inputPort)
		{
			this.defaultValue = defaultValue;
		}

		public override void ApplyDefaultValue()
		{
			inputPort.FloatInternalData = defaultValue;
		}

		public override string GenerateNotConnectedCode(ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			return NOT_CONNECTED_CODE;
		}
	}
}
#endif