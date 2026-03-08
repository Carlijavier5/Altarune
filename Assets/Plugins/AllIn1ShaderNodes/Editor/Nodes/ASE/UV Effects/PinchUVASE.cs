using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Pinch UV", EFFECT_CATEGORY_UV, "")]
	public class PinchUVASE : AbstractUVNodeASE
	{
		private const string FUNC_NAME = @"PinchUV({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("_PinchAmount", 0.35f);

			AddCustomTilingAndOffsetPort();
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif