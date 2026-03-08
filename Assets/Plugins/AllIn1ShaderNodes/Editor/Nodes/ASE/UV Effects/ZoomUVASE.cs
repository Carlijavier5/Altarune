using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Zoom UV", EFFECT_CATEGORY_UV, "")]
	public class ZoomUVASE : AbstractUVNodeASE
	{
		private const string FUNC_NAME = @"ZoomUV({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("_ZoomUvAmount", 0.35f);

			AddCustomTilingAndOffsetPort();
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif