using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Posterize", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class PosterizeNodeASE : AbstractColorRGBEffectNode
	{
		private const string FUNC_NAME = @"Posterize({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("_PosterizeGamma", 0.75f);
			AddFloatCustomPort("_PosterizeNumColors", 8f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif