using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Circle Gradient", EFFECT_CATEGORY_PROCEDURAL_SHAPES, "")]

	public class CircleGradientASE : AbstractProceduralShapeNodeASE
	{
		private const string FUNC_NAME = @"CircleGradient({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("size", 1f);
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