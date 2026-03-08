using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Hue Shift", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class HueShiftNodeASE : AbstractColorRGBEffectNode
	{
		private const string FUNC_NAME = @"HueShift({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("hueShift", 100f);
			AddFloatCustomPort("hueSaturation", 2f);
			AddFloatCustomPort("hueBrightness", 1f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif