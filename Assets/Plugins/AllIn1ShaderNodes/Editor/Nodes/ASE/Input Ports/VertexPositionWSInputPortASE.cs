#if AMPLIFY_SHADER_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class VertexPositionWSInputPortASE : Vector3InputPortASE
	{
		private const string NOT_CONNECTED_CODE = "WorldPosition";

		public VertexPositionWSInputPortASE(InputPort inputPort, Vector3 defaultValue) : base(inputPort, defaultValue)
		{
		}

		public override string GenerateNotConnectedCode(ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			return NOT_CONNECTED_CODE;
		}
	}
}

#endif