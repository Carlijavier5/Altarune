using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Height Gradient", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class HeightGradientNodeASE : AbstractColorRGBAEffectNode
	{
		private const string FUNC_NAME = @"HeightGradient({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddCustomVertexPositionOSPort();
			AddFloatCustomPort("_MinGradientHeight", 0f);
			AddFloatCustomPort("_MaxGradientHeight", 0.1f);
			AddColorCustomPort("_GradientHeightColor01", new Color(0.1f, 0.1f, 0.1f, 1f));
			AddColorCustomPort("_GradientHeightColor02", Color.white);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif