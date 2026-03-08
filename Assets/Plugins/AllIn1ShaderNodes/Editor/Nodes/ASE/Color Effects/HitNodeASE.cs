using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Hit", EFFECT_CATEGORY_COLOR_EFFECTS, "")]
	public class HitNodeASE : AbstractColorRGBAEffectNode
	{
		private const string FUNC_NAME = @"Hit({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddColorCustomPort("_HitColor", Color.red);
			AddFloatCustomPort("_HitGlow", 1f);
			AddFloatCustomPort("_HitBlend", 0.5f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif