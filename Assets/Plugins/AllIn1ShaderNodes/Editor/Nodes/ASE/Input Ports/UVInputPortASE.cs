#if AMPLIFY_SHADER_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class UVInputPortASE : Vector2InputPortASE
	{
		private const string NOT_CONNECTED_CODE = "mainUV";

		public UVInputPortASE(InputPort inputPort, Vector2 defaultValue) : base(inputPort, defaultValue)
		{
		}



		public override string GenerateNotConnectedCode(ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			return NOT_CONNECTED_CODE;
		}
	}
}

#endif