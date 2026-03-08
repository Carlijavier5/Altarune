using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Wave UV", EFFECT_CATEGORY_UV, "")]
	public class WaveUVNodeASE : AbstractUVNodeASE
	{
		private const string FUNC_NAME = @"WaveUV({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddCustomShaderTimePort();
			AddFloatCustomPort("_WaveX", 0f);
			AddCustomTilingAndOffsetPort();
			AddFloatCustomPort("_WaveY", 0.5f);
			AddFloatCustomPort("_WaveAmount", 7f);
			AddFloatCustomPort("_WaveSpeed", 10f);
			AddFloatCustomPort("_WaveStrength", 7.5f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif