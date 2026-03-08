using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("SDF Square", EFFECT_CATEGORY_PROCEDURAL_SHAPES, "")]

	public class SDFSquareASE : AbstractProceduralShapeNodeASE
	{
		private const string FUNC_NAME = @"SDFSquare({0})";

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
			AddFloatCustomPort("cutOff", 0.2f);
			AddFloat2CustomPort("center", Vector2.zero);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif