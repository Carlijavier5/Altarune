using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Vertex Voxel", EFFECT_CATEGORY_VERTEX, "")]
	public class VertexVoxelNodeASE : AbstractVertexEffectNodeASE
	{
		private const string FUNC_NAME = @"VertexVoxel({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override void CreateCustomPorts()
		{
			base.CreateCustomPorts();

			AddFloatCustomPort("_VoxelSize", 7f);
			AddFloatCustomPort("_VoxelBlend", 1f);
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif