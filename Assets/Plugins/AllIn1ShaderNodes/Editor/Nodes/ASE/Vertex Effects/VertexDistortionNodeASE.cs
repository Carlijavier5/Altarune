using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Vertex Distortion", EFFECT_CATEGORY_VERTEX, "")]
	public class VertexDistortionNodeASE : AbstractVertexEffectNodeASE
	{
		private const string FUNC_NAME = @"VertexDistortion({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddNormalOSCustomPort();
			
			AddFloat3CustomPort("_VertexDistortionNoiseSpeed", new Vector3(1f, 1f, 0f));

			AddFloatCustomPort("_VertexDistortionAmount", 0.5f);

			AddSampler2DCustomPort("_VertexDistortionNoiseTex", string.Empty);

			AddCustomTilingAndOffsetPort();
			AddCustomShaderTimePort();
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif