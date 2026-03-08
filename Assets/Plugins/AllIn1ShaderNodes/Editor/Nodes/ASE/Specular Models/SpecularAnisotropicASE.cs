using System;
using UnityEditor;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Specular - Anisotropic", EFFECT_CATEGORY_SPECULAR, "")]
	public class SpecularAnisotropicASE : AbstractNodeASE
	{
		private const WirePortDataType OUTPUT_DATA_TYPE = WirePortDataType.FLOAT3;

		private const string FUNC_NAME = @"SpecularModel_Anisotropic_ASE({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init(MasterNodePortCategory.Fragment);
		}

		protected override void AddIncludes(ref MasterNodeDataCollector dataCollector)
		{
			base.AddIncludes(ref dataCollector);

			AddInclude("Assets/Plugins/AllIn1ShaderNodes/Shaders/AllIn1NodeLibrary_SpecularModels_ASE.hlsl", ref dataCollector);
		}

		protected override void CreateCustomPorts()
		{
			AddNormalWSCustomPort();
			AddTangentWSCustomPort();
			AddBitangentWSCustomPort();

			AddCustomViewDirWSPort();
			AddFloatCustomPort("specularAtten", 0.5f);
			AddFloatCustomPort("anisotropy", 0.45f);
			AddFloatCustomPort("anisoShininess", 0.85f);
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