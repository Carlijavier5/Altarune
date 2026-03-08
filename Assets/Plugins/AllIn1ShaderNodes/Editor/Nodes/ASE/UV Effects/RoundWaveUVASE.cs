using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Round Wave UV", EFFECT_CATEGORY_UV, "")]
	public class RoundWaveUVASE : AbstractUVNodeASE
	{
		private const string FUNC_NAME = @"RoundWaveUV({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("_RoundWaveStrength", 0.5f);

			AddFloatCustomPort("_RoundWaveSpeed", 0.5f);

			AddCustomTilingAndOffsetPort();
			AddCustomTexelSizePort();

			AddCustomShaderTimePort();
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif