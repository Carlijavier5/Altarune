using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Reflections Toon", EFFECT_CATEGORY_GLOBAL_ILLUMINATION, "")]
	public class ReflectionsToonASE : AbstractReflectionsASE
	{
		private const string FUNC_NAME = @"GetSkyColorToon({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("_ToonFactor", 1f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif