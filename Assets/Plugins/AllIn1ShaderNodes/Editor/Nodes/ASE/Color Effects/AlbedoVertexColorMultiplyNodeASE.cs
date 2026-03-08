using System;
using UnityEditor;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Albedo Vertex Color - Multiply", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class AlbedoVertexColorMultiplyNodeASE : AbstractColorRGBAEffectNode
	{
		private const string FUNC_NAME = @"AlbedoVertexColor_Multiply({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddVertexColorCustomPort();
			AddFloatCustomPort("_VertexColorBlending", 1f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif