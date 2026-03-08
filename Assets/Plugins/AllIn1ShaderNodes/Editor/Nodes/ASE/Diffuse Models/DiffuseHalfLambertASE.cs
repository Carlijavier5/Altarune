using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Half Lambert", EFFECT_CATEGORY_DIFFUSE, "")]
	public class DiffuseHalfLambertASE : AbstractDiffuseASE
	{
		private const string FUNC_NAME = @"DiffuseLight_HalfLambert({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("wrap", 0.6f);
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