using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Diffuse Classic", EFFECT_CATEGORY_DIFFUSE, "")]
	public class DiffuseClassicASE : AbstractDiffuseASE
	{
		private const string FUNC_NAME = @"DiffuseLight_Classic({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
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