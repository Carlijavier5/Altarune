#if AMPLIFY_SHADER_EDITOR
using AllIn1ShaderNodes;

namespace AmplifyShaderEditor
{
	public class FloatInputPortASE : AbstractInputPortASE
	{
		private float defaultValue;

		public FloatInputPortASE(float defaultValue) : base(null)
		{
			this.defaultValue = defaultValue;
		}

		public FloatInputPortASE(InputPort inputPort, float defaultValue) : base(inputPort)
		{
			this.defaultValue = defaultValue;
		}

		public override void ApplyDefaultValue()
		{
			inputPort.FloatInternalData = defaultValue;
		}
	}
}
#endif