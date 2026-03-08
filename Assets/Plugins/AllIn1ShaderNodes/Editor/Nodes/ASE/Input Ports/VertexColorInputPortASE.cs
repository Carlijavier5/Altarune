#if AMPLIFY_SHADER_EDITOR
using AllIn1ShaderNodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class VertexColorInputPortASE : ColorInputPortASE
	{
		private const string NOT_CONNECTED_CODE = "vertexColor";

		public VertexColorInputPortASE(InputPort inputPort, Color defaultValue) : base(inputPort, defaultValue)
		{
		}

		public override string GenerateNotConnectedCode(ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			return NOT_CONNECTED_CODE;
		}
	}
}

#endif