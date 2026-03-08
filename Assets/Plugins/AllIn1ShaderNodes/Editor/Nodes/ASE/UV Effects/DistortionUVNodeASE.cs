using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("UV Distortion", EFFECT_CATEGORY_UV, "")]
	public class DistortionUVNodeASE : AbstractUVNodeASE
	{
		private const string FUNC_NAME = @"UVDistortion({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddCustomShaderTimePort();

			AddFloatCustomPort("_DistortTexXSpeed", 2f);
			AddFloatCustomPort("_DistortTexYSpeed", 2f);
			AddFloatCustomPort("_DistortAmount", 0.3f);
			AddSampler2DCustomPort("_DistortTex", string.Empty);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif