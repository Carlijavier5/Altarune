using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Vertex Inflate", EFFECT_CATEGORY_VERTEX, "")]
	public class VertexInflateNodeASE : AbstractVertexEffectNodeASE
	{
		private const string FUNC_NAME = @"VertexInflate({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddNormalOSCustomPort();

			AddFloatCustomPort("_MinInflate", 0f);
			
			AddFloatCustomPort("_MaxInflate", 0.2f);

			AddFloatCustomPort("_InflateBlend", 1f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif