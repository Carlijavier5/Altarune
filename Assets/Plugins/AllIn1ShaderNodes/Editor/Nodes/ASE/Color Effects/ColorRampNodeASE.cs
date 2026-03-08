using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Color Ramp", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class ColorRampNodeASE : AbstractColorRGBAEffectNode
	{
		private const string FUNC_NAME = @"ColorRamp({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddCustomShaderTimePort();
			
			AddFloatCustomPort("_ColorRampLuminosity", 0f);
			AddFloatCustomPort("_ColorRampTiling", 1f);
			AddFloatCustomPort("_ColorRampScrollSpeed", 0f);
			AddFloatCustomPort("_ColorRampBlend", 1f);

			AddSampler2DCustomPort("_ColorRampTex", string.Empty);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif