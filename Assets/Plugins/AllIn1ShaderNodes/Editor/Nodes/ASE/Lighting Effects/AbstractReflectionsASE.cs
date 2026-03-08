using UnityEngine;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	public class AbstractReflectionsASE : AbstractNodeASE
	{
		private const WirePortDataType OUTPUT_DATA_TYPE = WirePortDataType.FLOAT3;

		protected void Init()
		{
			base.Init(MasterNodePortCategory.Fragment);
		}

		protected override void AddIncludes(ref MasterNodeDataCollector dataCollector)
		{
			base.AddIncludes(ref dataCollector);


			AddInclude("Assets/Plugins/AllIn1ShaderNodes/Shaders/AllIn1Node_GlobalIllumination.hlsl", ref dataCollector);
		}

		protected override void CreateCustomPorts()
		{
			AddNormalWSCustomPort();
			AddCustomViewDirWSPort();

			AddFloatCustomPort("reflectionsAtten", 0.5f);
			AddFloatCustomPort("reflectionsLod", 0.1f);
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