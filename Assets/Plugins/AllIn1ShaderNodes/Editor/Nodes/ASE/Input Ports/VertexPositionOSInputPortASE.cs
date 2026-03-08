#if AMPLIFY_SHADER_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class VertexPositionOSInputPortASE : Vector3InputPortASE
	{
		private const string NOT_CONNECTED_CODE = "vertexOS";

		public VertexPositionOSInputPortASE(InputPort inputPort, Vector3 defaultValue) : base(inputPort, defaultValue)
		{
		}

		public override string GenerateNotConnectedCode(ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			return NOT_CONNECTED_CODE;
		}
	}
}

#endif