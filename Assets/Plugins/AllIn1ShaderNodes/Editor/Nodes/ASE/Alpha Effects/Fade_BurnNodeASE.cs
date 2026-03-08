using System;
using UnityEngine;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Fade Burn", EFFECT_CATEGORY_ALPHA_EFFECTS, "")]
	public class Fade_BurnNodeASE : AbstractNodeASE
	{
		private const WirePortDataType OUTPUT_RGBA_DATA_TYPE = WirePortDataType.FLOAT4;
		private const WirePortDataType OUTPUT_RGB_DATA_TYPE = WirePortDataType.FLOAT3;
		private const WirePortDataType OUTPUT_ALPHA_DATA_TYPE = WirePortDataType.FLOAT;

		private const string FUNC_NAME = @"Fade_Burn({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			base.Init(MasterNodePortCategory.Fragment);
		}

		protected override void AddIncludes(ref MasterNodeDataCollector dataCollector)
		{
			base.AddIncludes(ref dataCollector);

			AddInclude("Assets/Plugins/AllIn1ShaderNodes/Shaders/AllIn1NodeLibrary_AlphaEffects.hlsl", ref dataCollector);
		}

		protected override void CreateCustomPorts()
		{
			AddFloat3CustomPort("inputRGB", Vector3.one);
			AddFloatCustomPort("inputAlpha", 1.0f);

			AddFloatCustomPort("_FadePower", 1f);

			AddFloatCustomPort("_FadeAmount", 0f);

			AddFloatCustomPort("_FadeTransition", 0.2f);

			AddFloatCustomPort("_FadeBurnWidth", 0.01f);

			AddColorCustomPort("_FadeBurnColor", new Color(1f, 1f, 0f, 1f));

			AddSampler2DCustomPort("_FadeTex", string.Empty);

			AddCustomUVPort();

			AddFloat2CustomPort("tiling", new Vector2(5f, 5f));
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}

		protected override void CreateOutputPorts()
		{
			CreateOutputPort(OUTPUT_RGBA_DATA_TYPE, "RGBA");
			CreateOutputPort(OUTPUT_RGB_DATA_TYPE, "RGB");
			CreateOutputPort(OUTPUT_ALPHA_DATA_TYPE, "Alpha");
		}

		private void CallFunctionIfMainOutputNotConnected(string funcName, string funcParams, string varName, ref MasterNodeDataCollector dataCollector)
		{
			if (!m_outputPorts[0].IsConnected)
			{
				string finalCalculation = GetFinalCalculation(funcName, funcParams);
				RegisterLocalVariable(0, finalCalculation, ref dataCollector, varName);
			}
		}

		protected override string CustomGenerateShaderForOutput(int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			AddIncludes(ref dataCollector);

			string res = string.Empty;

			string functionResVarName = "fadeBurnRGBA";
			string funcName = GetFuncName();
			string funcParams = GetParamsByInputPorts(ref dataCollector, ignoreLocalvar);

			if(outputId == 0)
			{
				string finalCalculation = GetFinalCalculation(funcName, funcParams);
				RegisterLocalVariable(0, finalCalculation, ref dataCollector, functionResVarName);

				res = functionResVarName;
			}
			else
			{
				CallFunctionIfMainOutputNotConnected(funcName, funcParams, functionResVarName, ref dataCollector);

				if (outputId == 1)
				{
					res = "fadeBurnRGBA.rgb";
				}
				else if (outputId == 2)
				{
					res = "fadeBurnRGBA.a";
				}
			}

			return res;
		}
	}
}
#endif