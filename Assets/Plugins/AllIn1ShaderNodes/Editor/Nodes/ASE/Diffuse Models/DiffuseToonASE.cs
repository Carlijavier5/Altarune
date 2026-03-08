using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Diffuse Toon", EFFECT_CATEGORY_DIFFUSE, "")]
	public class DiffuseToonASE : AbstractDiffuseASE
	{
		private const string FUNC_NAME = @"DiffuseLight_Toon({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("toonCutoff", 0.15f);;
			AddFloatCustomPort("toonSmoothness", 0.05f);
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