using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Gradient", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class GradientNodeASE : AbstractColorRGBAEffectNode
	{
		private const string FUNC_NAME = @"GradientColor({0})";

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
			AddColorCustomPort("_GradTopRightCol", new Color(1f, 1f, 0f, 1f));
			AddColorCustomPort("_GradBotLeftCol", new Color(0f, 0f, 1f, 1f));
			AddColorCustomPort("_GradBotRightCol", new Color(0f, 1f, 0f, 1f));
			AddFloatCustomPort("_GradBoostX", 1.2f);
			AddFloatCustomPort("_GradBoostY", 1.2f);
			AddFloatCustomPort("_GradBlend", 0.75f);
			AddCustomTilingAndOffsetPort();

		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif