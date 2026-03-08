using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Soft Particles", EFFECT_CATEGORY_ALPHA_EFFECTS, "")]
	public class SoftParticlesNodeASE : AbstractAlphaEffectNode
	{
		private const string FUNC_NAME = @"SoftParticles({0})";
		
		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);
			
			base.Init();
		}

		protected override void AddDefines(ref MasterNodeDataCollector dataCollector)
		{
			base.AddDefines(ref dataCollector);

			AddDefine(ASEConstants.DEFINE_NEEDS_SCENE_DEPTH_DIFF, ref dataCollector);
		}

		protected override bool NeedsSceneDepthDiff()
		{
			return true;
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("_SoftFactor", 1f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif
