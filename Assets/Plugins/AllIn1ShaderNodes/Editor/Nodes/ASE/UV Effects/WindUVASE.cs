using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Wind UV", EFFECT_CATEGORY_UV, "")]
	public class WindUVASE : AbstractUVNodeASE
	{
		private const string FUNC_NAME = @"WindUV({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("_GrassSpeed", 2f);

			AddFloatCustomPort("_GrassWind", 20f);

			AddFloatCustomPort("_GrassRadialBend", 0.1f);

			AddCustomShaderTimePort();
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif