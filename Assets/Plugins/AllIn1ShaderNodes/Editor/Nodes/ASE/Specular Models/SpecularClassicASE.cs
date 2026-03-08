using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Specular - Classic", EFFECT_CATEGORY_SPECULAR, "")]
	public class SpecularClassicASE : AbstractSpecularASE
	{
		private const string FUNC_NAME = @"SpecularModel_Classic_ASE({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif