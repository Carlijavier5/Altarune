using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("AO Map", EFFECT_CATEGORY_GLOBAL_ILLUMINATION, "")]
	public class AOMapASE : AbstractNodeASE
	{
		private const string FUNC_NAME = @"AOMap({0})";

		private const WirePortDataType OUTPUT_DATA_TYPE = WirePortDataType.FLOAT3;

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected void Init()
		{
			base.Init(MasterNodePortCategory.Fragment);
		}

		protected override void AddIncludes(ref MasterNodeDataCollector dataCollector)
		{
			base.AddIncludes(ref dataCollector);

			AddInclude("Assets/Plugins/AllIn1ShaderNodes/Shaders/AllIn1NodeLibrary_LightingEffects.hlsl", ref dataCollector);
		}

		protected override void CreateCustomPorts()
		{
			AddFloatCustomPort("_AOContrast", 1f);
			AddFloatCustomPort("_AOMapStrength", 1f);
			AddFloat3CustomPort("_AOColor", Vector3.zero);
			AddCustomUVPort();
			AddSampler2DCustomPort("_AOMap", string.Empty);
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