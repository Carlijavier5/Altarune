#if AMPLIFY_SHADER_EDITOR
using AllIn1ShaderNodes;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class Vector4InputPortASE : AbstractInputPortASE
	{
		private Vector4 defaultValue;

		public Vector4InputPortASE(InputPort inputPort) : base(inputPort)
		{

		}

		public Vector4InputPortASE(InputPort inputPort, Vector4 defaultValue) : base(inputPort)
		{
			this.defaultValue = defaultValue;
		}

		public override void ApplyDefaultValue()
		{
			inputPort.Vector4InternalData = defaultValue;
		}
	}
}

#endif