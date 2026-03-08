using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Shine", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class ShineASE : AbstractColorRGBAEffectNode
	{
		private const string FUNC_NAME = @"Shine({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddCustomUVPort();
			AddColorCustomPort("_ShineColor", Color.white);
			AddFloatCustomPort("_ShineLocation", 0.5f);
			AddFloatCustomPort("_ShineRotate", 0f);
			AddFloatCustomPort("_ShineWidth", 0.1f);
			AddFloatCustomPort("_ShineGlow", 1f);
			AddSampler2DCustomPort("_ShineMask", string.Empty);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif