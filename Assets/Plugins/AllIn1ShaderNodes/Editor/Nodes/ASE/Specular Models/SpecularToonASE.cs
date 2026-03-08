using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Specular - Toon", EFFECT_CATEGORY_SPECULAR, "")]
	public class SpecularToonASE : AbstractSpecularASE
	{
		private const string FUNC_NAME = @"SpecularModel_Toon_ASE({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("specularSmoothness", 0f);
			AddFloatCustomPort("specularToonCutoff", 0.35f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif