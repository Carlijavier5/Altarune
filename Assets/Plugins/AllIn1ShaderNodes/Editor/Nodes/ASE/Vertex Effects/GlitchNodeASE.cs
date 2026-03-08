using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Glitch", EFFECT_CATEGORY_VERTEX, "")]
	public class GlitchNodeASE : AbstractVertexEffectNodeASE
	{
		private const string FUNC_NAME = @"Glitch({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloat4CustomPort("_GlitchOffset", new Vector4(-0.5f, 0f, 0f, 0f));
			AddFloatCustomPort("_GlitchWorldSpace", 1f);
			AddFloatCustomPort("_GlitchSpeed", 2.5f);
			AddFloatCustomPort("_GlitchTiling", 5f);
			AddFloatCustomPort("_GlitchAmount", 0.5f);

			AddCustomShaderTimePort();
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif