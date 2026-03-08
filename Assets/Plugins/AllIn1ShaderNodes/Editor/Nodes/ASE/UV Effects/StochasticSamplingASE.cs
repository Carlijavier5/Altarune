using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Stochastic Sampling", EFFECT_CATEGORY_UV, "")]
	public class StochasticSamplingASE : AbstractNodeASE
	{
		private const WirePortDataType OUTPUT_DATA_TYPE = WirePortDataType.FLOAT3;

		private const string FUNC_NAME = @"StochasticSampling({0})";

		protected void Init()
		{
			base.Init(MasterNodePortCategory.Fragment);
		}

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void AddIncludes(ref MasterNodeDataCollector dataCollector)
		{
			base.AddIncludes(ref dataCollector);

			AddInclude("Assets/Plugins/AllIn1ShaderNodes/Shaders/AllIn1NodeLibrary_UVEffects.hlsl", ref dataCollector);
		}

		protected override void CreateCustomPorts()
		{
			AddCustomUVPort();

			AddSampler2DCustomPort("_Texture", string.Empty);
			AddFloatCustomPort("_StochasticScale", 3.464f);

			AddFloatCustomPort("_StochasticSkew", 0.57735027f);
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