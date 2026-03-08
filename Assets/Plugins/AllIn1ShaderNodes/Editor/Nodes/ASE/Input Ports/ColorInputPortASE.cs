#if AMPLIFY_SHADER_EDITOR

using AmplifyShaderEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllIn1ShaderNodes
{
	public class ColorInputPortASE : AbstractInputPortASE
	{
		private Color defaultValue;

		public ColorInputPortASE(Color defaultValue) : base(null)
		{
			this.defaultValue = defaultValue;
		}

		public ColorInputPortASE(InputPort inputPort, Color defaultValue) : base(inputPort)
		{
			this.defaultValue = defaultValue;
		}

		public override void ApplyDefaultValue()
		{
			inputPort.ColorInternalData = defaultValue;
		}
	}
}

#endif