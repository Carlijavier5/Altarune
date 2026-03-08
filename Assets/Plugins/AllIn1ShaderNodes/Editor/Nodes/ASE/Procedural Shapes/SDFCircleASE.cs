using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("SDF Circle", EFFECT_CATEGORY_PROCEDURAL_SHAPES, "")]

	public class SDFCircleASE : AbstractProceduralShapeNodeASE
	{
		private const string FUNC_NAME = @"SDFCircle({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("size", 1f);
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