using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Alpha Remap", EFFECT_CATEGORY_ALPHA_EFFECTS, "")]
	public class AlphaRemapNodeASE : AbstractAlphaEffectNode
	{
		private const string FUNC_NAME = @"AlphaRemap({0})";
		
		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);
			
			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("_AlphaStepMin", 0f);
			AddFloatCustomPort("_AlphaStepMax", 1f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif
