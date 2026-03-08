using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Chromatic Aberration", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class ChromaticAberrationASE : AbstractColorRGBAEffectNode
	{
		private const string FUNC_NAME = @"ChromaticAberration({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddCustomUVPort();
			AddSampler2DCustomPort("Tex", string.Empty);
			AddFloatCustomPort("_AberrationAmount", 1f);
			AddFloatCustomPort("_AberrationAlpha", 0.4f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif