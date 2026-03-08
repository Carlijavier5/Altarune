#if AMPLIFY_SHADER_EDITOR
using AmplifyShaderEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class CameraPositionWSInputPortASE : Vector3InputPortASE
	{
		private const string NOT_CONNECTED_CODE = "_WorldSpaceCameraPos";

		public CameraPositionWSInputPortASE(InputPort inputPort, Vector3 defaultValue) : base(inputPort, defaultValue)
		{

		}

		public override string GenerateNotConnectedCode(ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			return NOT_CONNECTED_CODE;
		}
	}
}
#endif