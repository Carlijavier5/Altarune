using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Subsurface Scattering", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class SubsurfaceScatteringNodeASE : AbstractColorRGBEffectNode
	{
		private const string FUNC_NAME = @"SubsurfaceScattering({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddCustomUVPort();
			AddCustomViewDirWSPort();
			AddNormalWSCustomPort();

			AddFloatCustomPort("_NormalInfluence", 0.5f);
			AddFloatCustomPort("_SSSPower", 1f);
			AddFloatCustomPort("_SSSFrontPower", 3f);
			AddFloatCustomPort("_SSSFrontAtten", 0.3f);
			AddFloatCustomPort("_SSSAtten", 1f);
			AddColorCustomPort("_SSSColor", Color.white);
			AddSampler2DCustomPort("_SSSMap", string.Empty);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif