using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Albedo Vertex Color - Replace", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class AlbedoVertexColorReplaceNodeASE : AbstractColorRGBAEffectNode
	{
		private const string FUNC_NAME = @"AlbedoVertexColor_Replace({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddVertexColorCustomPort();
			AddFloatCustomPort("_VertexColorBlending", 0.75f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif