using UnityEngine;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	public class AbstractUVNodeASE : AbstractNodeASE
	{
		private const WirePortDataType OUTPUT_DATA_TYPE = WirePortDataType.FLOAT2;

		protected void Init()
		{
			base.Init(MasterNodePortCategory.Fragment);
		}

		protected override void AddIncludes(ref MasterNodeDataCollector dataCollector)
		{
			base.AddIncludes(ref dataCollector);


			AddInclude("Assets/Plugins/AllIn1ShaderNodes/Shaders/AllIn1NodeLibrary_UVEffects.hlsl", ref dataCollector);
		}

		protected override void CreateCustomPorts()
		{
			AddCustomUVPort();
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


