using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Luminosity", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class LuminosityNodeASE : AbstractColorRGBAEffectNode
	{
		private const string FUNC_NAME = @"Luminosity({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif