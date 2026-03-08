#if AMPLIFY_SHADER_EDITOR
using AllIn1ShaderNodes;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class Vector3InputPortASE : AbstractInputPortASE
	{
		private Vector3 defaultValue;


		public Vector3InputPortASE(Vector3 defaultValue) : base(null)
		{
			this.defaultValue = defaultValue;
		}

		public Vector3InputPortASE(InputPort inputPort, Vector3 defaultValue) : base(inputPort)
		{
			this.defaultValue = defaultValue;
		}

		public override void ApplyDefaultValue()
		{
			inputPort.Vector3InternalData = defaultValue;
		}
	}
}
#endif