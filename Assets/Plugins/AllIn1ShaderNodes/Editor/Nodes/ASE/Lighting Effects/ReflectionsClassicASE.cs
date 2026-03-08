using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Reflections Classic", EFFECT_CATEGORY_GLOBAL_ILLUMINATION, "")]
	public class ReflectionsClassicASE : AbstractReflectionsASE
	{
		private const string FUNC_NAME = @"GetSkyColor({0})";

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