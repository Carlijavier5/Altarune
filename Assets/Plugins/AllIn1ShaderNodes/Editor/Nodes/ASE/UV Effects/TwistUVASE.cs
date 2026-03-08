using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Twist UV", EFFECT_CATEGORY_UV, "")]
	public class TwistUVASE : AbstractUVNodeASE
	{
		private const string FUNC_NAME = @"TwistUV({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("_TwistUvAmount", 1f);
			AddFloatCustomPort("_TwistUvPosX", 0.75f);
			AddFloatCustomPort("_TwistUvPosY", 0.5f);
			AddFloatCustomPort("_TwistUvRadius", 0.5f);

			AddCustomTilingAndOffsetPort();
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif