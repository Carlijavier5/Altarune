using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Alpha Round", EFFECT_CATEGORY_ALPHA_EFFECTS, "")]
	public class AlphaRoundNodeASE : AbstractNodeASE
	{
		private const string FUNC_NAME = @"AlphaRound_ASE({0})";
		private const WirePortDataType OUTPUT_DATA_TYPE = WirePortDataType.FLOAT;

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);
			base.Init(MasterNodePortCategory.Fragment);
		}

		protected override void AddIncludes(ref MasterNodeDataCollector dataCollector)
		{
			base.AddIncludes(ref dataCollector);
			AddInclude("Assets/Plugins/AllIn1ShaderNodes/Shaders/AllIn1NodeLibrary_AlphaEffects_ASE.hlsl", ref dataCollector);
		}

		protected override void CreateCustomPorts()
		{
			AddFloatCustomPort("alpha", 1f);
		}

		protected override void CreateOutputPorts()
		{
			CreateOutputPortsDefault(OUTPUT_DATA_TYPE);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}

		protected override string CustomGenerateShaderForOutput(int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			return GenerateShaderForOutputDefault(outputId, ref dataCollector, ignoreLocalvar);
		}
	}
}
#endif
