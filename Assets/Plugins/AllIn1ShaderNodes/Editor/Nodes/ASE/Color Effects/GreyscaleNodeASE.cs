using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Greyscale", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class GreyscaleNodeASE : AbstractColorRGBEffectNode
	{
		private const string FUNC_NAME = @"Greyscale({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("_GreyscaleLuminosity", 0f);
			AddColorCustomPort("_GreyscaleTintColor", Color.white);
			AddFloatCustomPort("_GreyscaleBlend", 1f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif