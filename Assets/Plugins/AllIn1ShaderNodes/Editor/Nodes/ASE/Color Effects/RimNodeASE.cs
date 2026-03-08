using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Rim", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class RimNodeASE : AbstractColorRGBEffectNode
	{
		private const string FUNC_NAME = @"Rim({0})";

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

			AddFloat4CustomPort("_RimOffset", Vector4.zero);
			AddFloatCustomPort("_MinRim", 0f);
			AddFloatCustomPort("_MaxRim", 1f);
			AddFloatCustomPort("_RimAttenuation", 1.0f);
			AddColorCustomPort("_RimColor", Color.white);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif