using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Sprite Data", EFFECT_CATEGORY_SPRITES, "")]
	public class SimpleSpriteASE : AbstractNodeASE
	{
		private const WirePortDataType OUTPUT_SPRITE_RGB_DATA_TYPE		= WirePortDataType.FLOAT3;
		private const WirePortDataType OUTPUT_SPRITE_ALPHA_DATA_TYPE	= WirePortDataType.FLOAT;


		private const string FUNC_NAME = @"GetSpriteData_ASE({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			base.Init(MasterNodePortCategory.Fragment);
		}

		protected override void AddIncludes(ref MasterNodeDataCollector dataCollector)
		{
			base.AddIncludes(ref dataCollector);

			AddInclude("Assets/Plugins/AllIn1ShaderNodes/Shaders/AllIn1NodeLibrary_Sprites_ASE.hlsl", ref dataCollector);
		}

		protected override void CreateCustomPorts()
		{
			AddCustomUVPort();
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}

		protected override void CreateOutputPorts()
		{
			CreateOutputPort(OUTPUT_SPRITE_RGB_DATA_TYPE, "RGB");
			CreateOutputPort(OUTPUT_SPRITE_ALPHA_DATA_TYPE, "Alpha");
		}

		protected override string CustomGenerateShaderForOutput(int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			AddIncludes(ref dataCollector);

			//if (outputId == 0 && m_outputPorts[outputId].IsLocalValue(portCategory))
			//{
			//	return m_outputPorts[0].LocalValue(portCategory);
			//}


			string funcName = GetFuncName();
			string funcParams = GetParamsByInputPorts(ref dataCollector, ignoreLocalvar);
			funcParams += ", spriteAlpha";

			string finalCalculation = GetFinalCalculation(funcName, funcParams);

			

			string res = string.Empty;
			if(outputId == 0)
			{
				RegisterLocalVariable(outputId, finalCalculation, ref dataCollector, "spriteRGB");
				res = m_outputPorts[outputId].LocalValue(portCategory);
			}
			else if (outputId == 1)
			{
				if (!m_outputPorts[0].IsConnected)
				{
					RegisterLocalVariable(outputId, finalCalculation, ref dataCollector, "__spriteAlpha");
				}

				res = "spriteAlpha";

				//RegisterLocalVariable(outputId, "spriteAlpha", ref dataCollector, "spriteAlpha");
			}

			return res;
		}
	}
}
#endif