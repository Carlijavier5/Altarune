using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Procedural Grid", EFFECT_CATEGORY_PROCEDURAL_SHAPES, "")]

	public class ProceduralGridASE : AbstractNodeASE
	{
		private const WirePortDataType OUTPUT_DATA_TYPE = WirePortDataType.FLOAT2;

		private const string FUNC_NAME = @"ProceduralGrid({0})";

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

			AddInclude("Assets/Plugins/AllIn1ShaderNodes/Shaders/AllIn1NodeLibrary_ProceduralShapes.hlsl", ref dataCollector);
		}

		protected override void CreateCustomPorts()
		{
			AddCustomUVPort();
			AddFloatCustomPort("tileX", 10f);
			AddFloatCustomPort("tileY", 10f);
			AddFloat2CustomPort("widthHeight", new Vector2(100f, 100f));
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