#if AMPLIFY_SHADER_EDITOR
using AllIn1ShaderNodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class Vector2InputPortASE : AbstractInputPortASE
	{
		private Vector2 defaultValue;

		public Vector2InputPortASE(Vector2 defaultValue) : base(null)
		{
			this.defaultValue = defaultValue;
		}

		public Vector2InputPortASE(InputPort inputPort, Vector2 defaultValue) : base(inputPort)
		{
			this.defaultValue = defaultValue;
		}

		public override void ApplyDefaultValue()
		{
			inputPort.Vector2InternalData = defaultValue;
		}
	}
}
#endif