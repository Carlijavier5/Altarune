using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Scroll Texture", EFFECT_CATEGORY_UV, "")]
	public class ScrollTextureNodeASE : AbstractUVNodeASE
	{

		private const string FUNC_NAME = @"ScrollTexture({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddCustomShaderTimePort();
			AddFloatCustomPort("_ScrollTextureX", 1f);
			AddFloatCustomPort("_ScrollTextureY", 1f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif