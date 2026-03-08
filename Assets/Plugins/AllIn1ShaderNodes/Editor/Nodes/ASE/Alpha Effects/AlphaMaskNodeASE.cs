using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Alpha Mask", EFFECT_CATEGORY_ALPHA_EFFECTS, "")]
	public class AlphaMaskNodeASE : AbstractAlphaEffectNode
	{
		private const string FUNC_NAME = @"AlphaMask({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddSampler2DCustomPort("_MaskTex", string.Empty);
			AddCustomUVPort();
			AddFloat2CustomPort("tiling", new Vector2(1f, 1f));
			AddFloatCustomPort("_MaskPower", 1f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif
