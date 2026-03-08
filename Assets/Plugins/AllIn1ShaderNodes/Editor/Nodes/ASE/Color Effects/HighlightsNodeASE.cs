using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Highlights", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class HighlightsNodeASE : AbstractColorRGBEffectNode
	{
		private const string FUNC_NAME = @"Highlights({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddCustomViewDirWSPort();
			AddNormalWSCustomPort();
			AddFloat4CustomPort("_HighlightOffset", Vector4.zero);
			AddFloatCustomPort("_HighlightCutoff", 0.5f);
			AddFloatCustomPort("_HighlightSmoothness", 0.5f);
			AddColorCustomPort("_HighlightsColor", new Color(2f, 2f, 2f, 1f));
			AddFloatCustomPort("_HighlightsStrength", 1f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif