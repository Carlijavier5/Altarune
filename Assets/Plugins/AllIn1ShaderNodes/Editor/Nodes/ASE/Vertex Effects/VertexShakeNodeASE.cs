using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Vertex Shake", EFFECT_CATEGORY_VERTEX, "")]
	public class VertexShakeNodeASE : AbstractVertexEffectNodeASE
	{
		private const string FUNC_NAME = @"VertexShake({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("_ShakeSpeedMult", 1f);
			AddFloat4CustomPort("_ShakeSpeed", new Vector4(41f, 49f, 45f, 0f));
			AddFloat4CustomPort("_ShakeMaxDisplacement", new Vector4(0.1f, 0.1f, 0.1f, 0f));
			AddFloatCustomPort("_ShakeBlend", 1f);

			AddCustomShaderTimePort();
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif