using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Gradient Radial", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class GradientRadialNodeASE : AbstractColorRGBAEffectNode
	{
		private const string FUNC_NAME = @"GradientColorRadial({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddCustomUVPort();
			AddColorCustomPort("_GradTopLeftCol", new Color(1f, 0f, 0f, 1f));
			AddColorCustomPort("_GradBotLeftCol", new Color(0f, 0f, 1f, 1f));
			AddFloatCustomPort("_GradBoostX", 1.2f);
			AddFloatCustomPort("_GradBlend", 0.9f);
			AddCustomTilingAndOffsetPort();
			AddCustomTexelSizePort();
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif