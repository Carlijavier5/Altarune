using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Scene Depth Diff", EFFECT_CATEGORY_DEPTH_EFFECTS, "")]
	public class SceneDepthDiffASE : AbstractNodeASE
	{
		private const string FUNC_NAME = @"SceneDepthDiff({0})";

		private const WirePortDataType OUTPUT_DATA_TYPE = WirePortDataType.FLOAT;

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
			AddIncludes(ref dataCollector);
			AddDefines(ref dataCollector);

			string res = "sceneDepthDiff";

			return res;
		}
	}
}
#endif