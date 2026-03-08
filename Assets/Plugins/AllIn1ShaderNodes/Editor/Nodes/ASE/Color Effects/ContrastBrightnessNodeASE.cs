using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Contrast Brightness", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class ContrastBrightnessNodeASE : AbstractColorRGBEffectNode
	{
		private const string FUNC_NAME = @"ContrastBrightness({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("contrast", 2f);
			AddFloatCustomPort("brightness", 0f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif