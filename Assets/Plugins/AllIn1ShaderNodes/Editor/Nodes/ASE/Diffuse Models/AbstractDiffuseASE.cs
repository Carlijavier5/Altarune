using UnityEngine;

#if AMPLIFY_SHADER_EDITOR

namespace AmplifyShaderEditor
{
	public abstract class AbstractDiffuseASE : AbstractNodeASE
	{
		private const WirePortDataType OUTPUT_DATA_TYPE = WirePortDataType.FLOAT3;

		protected void Init()
		{
			base.Init(MasterNodePortCategory.Fragment);
		}

		protected override void AddIncludes(ref MasterNodeDataCollector dataCollector)
		{
			base.AddIncludes(ref dataCollector);

			AddInclude("Assets/Plugins/AllIn1ShaderNodes/Shaders/AllIn1NodeLibrary_DiffuseModels.hlsl", ref dataCollector);
		}

		protected override void CreateCustomPorts()
		{
			AddNormalWSCustomPort();
		}

		protected override void CreateOutputPorts()
		{
			CreateOutputPortsDefault(OUTPUT_DATA_TYPE);
		}
	}
}
#endif