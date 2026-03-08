using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Glow", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class GlowNodeASE : AbstractColorRGBAEffectNode
	{
		private const string FUNC_NAME = @"Glow({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("_Glow", 10f);
			AddColorCustomPort("_GlowColor", Color.white);
			AddFloatCustomPort("_GlowGlobal", 1f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif