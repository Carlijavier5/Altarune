using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Sprite Outline", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class SpriteOutlineASE : AbstractColorRGBAEffectNode
	{
		private const string FUNC_NAME = @"SpriteOutline({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddSampler2DCustomPort("Tex", string.Empty);
			AddCustomTexelSizePort();
			AddColorCustomPort("_OutlineColor", Color.white);
			AddFloatCustomPort("_OutlineWidth", 0.004f);
			AddFloatCustomPort("_OutlineAlpha", 1f);
			AddFloatCustomPort("_OutlineGlow", 1.5f);
			AddCustomUVPort();
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif