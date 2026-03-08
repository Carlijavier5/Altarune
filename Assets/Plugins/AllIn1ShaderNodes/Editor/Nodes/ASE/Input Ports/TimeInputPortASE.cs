#if AMPLIFY_SHADER_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class TimeInputPortASE : FloatInputPortASE
	{
		private const string NOT_CONNECTED_CODE = "_Time.y";

		public TimeInputPortASE(InputPort inputPort, float defaultValue) : base(inputPort, defaultValue)
		{
		
		}

		public override string GenerateNotConnectedCode(ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			return NOT_CONNECTED_CODE;
		}
	}
}
#endif