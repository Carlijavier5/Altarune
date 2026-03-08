#if AMPLIFY_SHADER_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class ViewDirWSInputPortASE : Vector3InputPortASE
	{
		private const string NOT_CONNECTED_CODE = "normalize(_WorldSpaceCameraPos.xyz - WorldPosition)";

		public ViewDirWSInputPortASE(InputPort inputPort, Vector3 defaultValue) : base(inputPort, defaultValue)
		{
		}

		public override string GenerateNotConnectedCode(ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			return NOT_CONNECTED_CODE;
		}
	}
}

#endif