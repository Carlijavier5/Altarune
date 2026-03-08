using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Radial Pattern - Lines", EFFECT_CATEGORY_PROCEDURAL_SHAPES, "")]

	public class RadialPatternLinesASE : AbstractProceduralShapeNodeASE
	{
		private const string FUNC_NAME = @"RadialPatternLines({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("segments", 20f);
			AddFloatCustomPort("rotationSlider", 0.2f);
			AddFloatCustomPort("spacing", 0.8f);
			AddFloatCustomPort("blur", 0.1f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif