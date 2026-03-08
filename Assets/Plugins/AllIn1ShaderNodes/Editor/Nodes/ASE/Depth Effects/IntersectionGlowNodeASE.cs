using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Intersection Glow", EFFECT_CATEGORY_DEPTH_EFFECTS, "")]
	public class IntersectionGlowNodeASE : AbstractNodeASE
	{
		private const string FUNC_NAME = @"IntersectionGlowInternal({0})";

		private const WirePortDataType OUTPUT_DATA_TYPE = WirePortDataType.FLOAT3;

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			base.Init(MasterNodePortCategory.Fragment);
		}

		protected override void AddDefines(ref MasterNodeDataCollector dataCollector)
		{
			base.AddDefines(ref dataCollector);

			AddDefine(ASEConstants.DEFINE_NEEDS_SCENE_DEPTH_DIFF, ref dataCollector);
		}

		protected override bool NeedsSceneDepthDiff()
		{
			return true;
		}

		protected override void CreateCustomPorts()
		{
			AddFloat3CustomPort("inputRGB", Vector3.one);
			AddFloatCustomPort("inputAlpha", 1.0f);

			AddFloatCustomPort("_DepthGlowDist", 0.2f);

			AddFloatCustomPort("_DepthGlowPower", 25f);

			AddFloatCustomPort("_DepthGlowColorlIntensity", 25f);

			AddFloatCustomPort("_DepthGlowGlobalIntensity", 2.0f);

			AddColorCustomPort("_DepthGlowColor", new Color(1f, 0.987f, 0.6f, 1f));
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}

		protected override void CreateOutputPorts()
		{
			CreateOutputPortsDefault(OUTPUT_DATA_TYPE);
		}

		protected override string CustomGenerateShaderForOutput(int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			return GenerateShaderForOutputDefault(outputId, ref dataCollector, ignoreLocalvar);
		}
	}
}
#endif