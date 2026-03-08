using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Fade", EFFECT_CATEGORY_ALPHA_EFFECTS, "")]
	public class FadeNodeASE : AbstractAlphaEffectNode
	{
		private const string FUNC_NAME = @"Fade({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("_FadePower", 1f);
			AddFloatCustomPort("_FadeAmount", 0.15f);
			AddFloatCustomPort("_FadeTransition", 0.2f);
			AddSampler2DCustomPort("_FadeTex", string.Empty);
			AddCustomUVPort();
			AddFloat2CustomPort("tiling", new Vector2(1f, 1f));
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif