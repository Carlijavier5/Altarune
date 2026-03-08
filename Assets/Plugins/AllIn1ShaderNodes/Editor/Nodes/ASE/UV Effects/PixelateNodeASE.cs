using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Pixelate", EFFECT_CATEGORY_UV, "")]
	public class PixelateNodeASE : AbstractUVNodeASE
	{
		private const string FUNC_NAME = @"Pixelate({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("_PixelateSize", 32f);
			AddCustomTexelSizePort();
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif