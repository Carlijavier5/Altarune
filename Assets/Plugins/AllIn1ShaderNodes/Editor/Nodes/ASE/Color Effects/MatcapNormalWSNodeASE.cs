using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Matcap - Normal WS", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class MatcapNormalWSNodeASE : AbstractColorRGBEffectNode
	{
		private const string FUNC_NAME = @"Matcap_NormalWS({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddNormalWSCustomPort();
			AddVertexPositionWSCustomPort();
			
			AddFloatCustomPort("_MatcapIntensity", 1f);
			AddFloatCustomPort("_MatcapBlend", 1f);
			AddSampler2DCustomPort("_MatcapTex", string.Empty);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif