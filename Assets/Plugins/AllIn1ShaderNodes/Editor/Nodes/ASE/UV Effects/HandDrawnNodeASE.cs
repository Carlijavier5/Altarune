using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Hand Drawn", EFFECT_CATEGORY_UV, "")]
	public class HandDrawnNodeASE : AbstractUVNodeASE
	{
		private const string FUNC_NAME = @"HandDrawn({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddCustomShaderTimePort();

			AddFloatCustomPort("_HandDrawnSpeed", 0.25f);
			AddFloatCustomPort("_HandDrawnAmount", 17f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif