using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Fade By Cam Distance - Near Fade", EFFECT_CATEGORY_ALPHA_EFFECTS, "")]
	public class FadeByCamDistanceNearFadeNodeASE : AbstractAlphaEffectNode
	{
		private const string FUNC_NAME = @"FadeByCamDistance_NearFade({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("_MinDistanceToFade", 0f);

			AddFloatCustomPort("_MaxDistanceToFade", 100f);


			AddCustomCameraPositionWSPort();
			
			AddVertexPositionWSCustomPort();
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif