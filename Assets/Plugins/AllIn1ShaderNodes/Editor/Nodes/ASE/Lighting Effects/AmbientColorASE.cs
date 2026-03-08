using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Ambient Color", EFFECT_CATEGORY_GLOBAL_ILLUMINATION, "")]
	public class AmbientColorASE : AbstractNodeASE
	{
		private const WirePortDataType OUTPUT_DATA_TYPE = WirePortDataType.FLOAT3;

		private const string FUNC_NAME = "GetAmbientColor({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			base.Init(MasterNodePortCategory.Fragment);
		}

		protected override void CreateCustomPorts()
		{
			AddNormalWSCustomPort();
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

