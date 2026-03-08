using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Hologram", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class HologramNodeASE : AbstractColorRGBAEffectNode
	{
		private const string FUNC_NAME = @"Hologram({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddVertexPositionWSCustomPort();
			AddCustomShaderTimePort();

			AddFloat4CustomPort("_HologramLineDirection", new Vector4(0f, 1f, 0f, 0f));
			AddFloatCustomPort("_HologramFrequency", 20f);
			AddFloatCustomPort("_HologramScrollSpeed", 2f);
			AddFloatCustomPort("_HologramLineCenter", 0.5f);
			AddFloatCustomPort("_HologramLineSpacing", 2f);
			AddFloatCustomPort("_HologramLineSmoothness", 2f);
			AddFloatCustomPort("_HologramBaseAlpha", 0.1f);
			AddFloatCustomPort("_HologramAccentFrequency", 2f);
			AddFloatCustomPort("_HologramAccentSpeed", 1f);
			AddFloatCustomPort("_HologramAlpha", 1f);
			AddFloatCustomPort("_HologramAccentAlpha", 0.5f);
			AddColorCustomPort("_HologramColor", new Color(1.25f, 2.8f, 6.8f, 1f));
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif