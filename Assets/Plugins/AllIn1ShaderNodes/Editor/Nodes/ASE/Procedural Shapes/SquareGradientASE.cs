using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Square Gradient", EFFECT_CATEGORY_PROCEDURAL_SHAPES, "")]

	public class SquareGradientASE : AbstractProceduralShapeNodeASE
	{
		private const string FUNC_NAME = @"SquareGradient({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("sizeX", 0.5f);
			AddFloatCustomPort("sizeY", 0.5f);
			AddFloatCustomPort("edgeMin", 0.16f);
			AddFloatCustomPort("edgeMax", 0.3f);
			AddFloat2CustomPort("center", Vector2.zero);
			AddFloatCustomPort("smooth", 0.2f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif