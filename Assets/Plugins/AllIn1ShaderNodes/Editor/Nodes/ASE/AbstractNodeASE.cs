#if AMPLIFY_SHADER_EDITOR
using AllIn1ShaderNodes;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public abstract class AbstractNodeASE : ParentNode
	{
		private const string STR_EMPTY_FUNC = "EMPTY_FUNC";

		protected MasterNodePortCategory portCategory;

		private List<AbstractInputPortASE> customPorts;

		[SerializeField]
		protected bool m_adjustPorts = false;


		protected virtual void Init(MasterNodePortCategory portCategory)
		{
			this.portCategory = portCategory;

			this.customPorts = new List<AbstractInputPortASE>();

			AddInputPorts();
			CreateOutputPorts();

			m_insideSize.Set(50, 25);
		}

		#region CUSTOM ADD INPUT PORT METHODS
		private InputPort CreateASEPortFloat(string portName, bool typeLocked, float defaultValue)
		{
			InputPort port = AddInputPort(WirePortDataType.FLOAT, typeLocked, portName);
			port.FloatInternalData = defaultValue;

			return port;
		}

		private InputPort CreateASEPortVector2(string portName, bool typeLocked, Vector2 defaultValue)
		{
			InputPort port = AddInputPort(WirePortDataType.FLOAT2, typeLocked, portName);
			port.Vector2InternalData = defaultValue;

			return port;
		}

		private InputPort CreateASEPortVector3(string portName, bool typeLocked, Vector3 defaultValue)
		{
			InputPort port = AddInputPort(WirePortDataType.FLOAT3, typeLocked, portName);
			port.Vector3InternalData = defaultValue;

			return port;
		}

		private InputPort CreateASEPortVector4(string portName, bool typeLocked, Vector4 defaultValue)
		{
			InputPort port = AddInputPort(WirePortDataType.FLOAT4, typeLocked, portName);
			port.Vector4InternalData = defaultValue;

			return port;
		}

		private InputPort CreateASEPortColor(string portName, bool typeLocked, Color defaultValue)
		{
			InputPort port = AddInputPort(WirePortDataType.COLOR, typeLocked, portName);
			port.ColorInternalData = defaultValue;

			return port;
		}

		private InputPort CreateASEPortSampler2D(string portName, bool typeLocked)
		{
			InputPort port = AddInputPort(WirePortDataType.SAMPLER2D, typeLocked, portName);
			return port;
		}

		#endregion

		private void AddInputPorts()
		{
			if (m_inputPorts.Count != customPorts.Count || 
				(m_inputPorts.Count == 0 && customPorts.Count == 0))
			{
				customPorts.Clear();
				m_inputPorts.Clear();

				CreateCustomPorts();
			}
		}

		protected abstract void CreateCustomPorts();

		protected abstract void CreateOutputPorts();

		protected void CreateOutputPortsDefault(WirePortDataType outputType)
		{
			CreateOutputPort(outputType);
		}

		private void ApplyDefaultValueToDisconnectedPorts()
		{
			for(int i = 0; i < customPorts.Count; i++)
			{
				if (!customPorts[i].IsConnected())
				{
					customPorts[i].ApplyDefaultValue();
				}
			}
		}

		public override void OnEnable()
		{
			ApplyDefaultValueToDisconnectedPorts();
		}

		protected InputPort AddASEPort(WirePortDataType wirePortData, bool typeLocked, string portName)
		{
			InputPort res = AddInputPort(wirePortData, typeLocked, portName);
			return res;
		}


		protected void AddFloat4CustomPort(string portName, Vector4 defaultValue, bool typeLocked = true)
		{
			InputPort inputPort = CreateASEPortVector4(portName, typeLocked, defaultValue);

			AbstractInputPortASE customPort = new Vector4InputPortASE(inputPort, defaultValue);
			customPorts.Add(customPort);
		}

		protected void AddFloat3CustomPort(string portName, Vector3 defaultValue, bool typeLocked = true)
		{
			InputPort inputPort = CreateASEPortVector3(portName, typeLocked, defaultValue);

			AbstractInputPortASE customPort = new Vector3InputPortASE(inputPort, defaultValue);
			customPorts.Add(customPort);
		}

		protected void AddFloat2CustomPort(string portName, Vector2 defaultValue, bool typeLocked = true)
		{
			InputPort inputPort = CreateASEPortVector2(portName, typeLocked, defaultValue);

			AbstractInputPortASE customPort = new Vector2InputPortASE(inputPort, defaultValue);
			customPorts.Add(customPort);
		}

		protected void AddColorCustomPort(string portName, Color defaultValue, bool typeLocked = true)
		{
			InputPort inputPort = CreateASEPortColor(portName, typeLocked, defaultValue);

			AbstractInputPortASE customPort = new ColorInputPortASE(inputPort, defaultValue);
			customPorts.Add(customPort);
		}

		protected void AddFloatCustomPort(string portName, float defaultValue, bool typeLocked = true)
		{
			InputPort inputPort = CreateASEPortFloat(portName, typeLocked, defaultValue);
			AbstractInputPortASE customPort = new FloatInputPortASE(inputPort, defaultValue);
			customPorts.Add(customPort);
		}

		protected void AddSampler2DCustomPort(string portName, string defaultTexPath, bool typeLocked = true)
		{
			InputPort inputPort = CreateASEPortSampler2D(portName, typeLocked);
			AbstractInputPortASE sampler2DPort = new Sampler2DInputPortASE(inputPort);

			customPorts.Add(sampler2DPort);
		}

		protected void AddCustomUVPort()
		{
			InputPort inputPort = CreateASEPortVector2("UV", true, Vector2.zero);

			AbstractInputPortASE customPort = new UVInputPortASE(inputPort, Vector2.zero);
			customPorts.Add(customPort);
		}

		protected void AddCustomTexelSizePort()
		{
			AddFloat4CustomPort("texelSize", Vector4.one);
		}

		protected void AddCustomTilingAndOffsetPort()
		{
			AddFloat4CustomPort("tilingAndOffset", new Vector4(1f, 1f, 0f, 0f));
		}

		protected void AddCustomViewDirWSPort()
		{
			InputPort inputPort = CreateASEPortVector3("View Dir (World Space)", true, Vector3.zero);

			AbstractInputPortASE customPort = new ViewDirWSInputPortASE(inputPort, Vector3.zero);
			customPorts.Add(customPort);
		}

		protected void AddCustomCameraPositionWSPort()
		{
			InputPort inputPort = CreateASEPortVector3("Camera Position (World Space)", true, Vector3.zero);

			AbstractInputPortASE customPort = new CameraPositionWSInputPortASE(inputPort, Vector3.zero);
			customPorts.Add(customPort);
		}

		protected void AddVertexPositionWSCustomPort()
		{
			InputPort inputPort = CreateASEPortVector3("Vertex Position (World Space)", true, Vector3.zero);

			AbstractInputPortASE customPort = new VertexPositionWSInputPortASE(inputPort, Vector3.zero);
			customPorts.Add(customPort);
		}

		protected void AddCustomVertexPositionOSPort()
		{
			InputPort inputPort = CreateASEPortVector3("Vertex Position (Object Space)", true, Vector3.zero);

			AbstractInputPortASE customPort = new VertexPositionOSInputPortASE(inputPort, Vector3.zero);
			customPorts.Add(customPort);
		}

		protected void AddCustomShaderTimePort()
		{
			InputPort inputPort = CreateASEPortFloat("Shader Time", true, 0f);

			AbstractInputPortASE customPort = new TimeInputPortASE(inputPort, 0f);
			customPorts.Add(customPort);
		}

		protected void AddVertexColorCustomPort()
		{
			InputPort inputPort = CreateASEPortColor("Vertex Color", true, Color.white);

			AbstractInputPortASE customPort = new VertexColorInputPortASE(inputPort, Color.white);
			customPorts.Add(customPort);
		}

		protected void AddNormalWSCustomPort()
		{
			InputPort inputPort = CreateASEPortVector3("Normal (World Space)", true, Vector3.up);

			AbstractInputPortASE customPort = new NormalWSPortASE(inputPort, Vector3.up);
			customPorts.Add(customPort);
		}

		protected void AddTangentWSCustomPort()
		{
			InputPort inputPort = CreateASEPortVector3("Tangent (World Space)", true, Vector3.right);

			AbstractInputPortASE customPort = new TangentWSPortASE(inputPort, Vector3.right);
			customPorts.Add(customPort);
		}

		protected void AddBitangentWSCustomPort()
		{
			InputPort inputPort = CreateASEPortVector3("Bitangent (World Space)", true, Vector3.forward);

			AbstractInputPortASE customPort = new BitangentWSPortASE(inputPort, Vector3.forward);
			customPorts.Add(customPort);
		}

		protected void AddNormalOSCustomPort()
		{
			InputPort inputPort = CreateASEPortVector3("Normal (Object Space)", true, Vector3.up);

			AbstractInputPortASE customPort = new NormalOSPortASE(inputPort, Vector3.up);
			customPorts.Add(customPort);
		}

		protected void AddSceneDepthDiffCustomPort()
		{
			InputPort inputPort = CreateASEPortFloat("Scene Depth Diff", true, 1.0f);

			AbstractInputPortASE customPort = new SceneDepthDiffPortASE(inputPort, 1.0f);
			customPorts.Add(customPort);
		}

		protected void CreateOutputPort(WirePortDataType outputType, string portName = "Out")
		{
			AddOutputPort(outputType, portName);
		}

		protected void AddInclude(string path, ref MasterNodeDataCollector dataCollector)
		{
			int uniqueID = Guid.NewGuid().GetHashCode();
			dataCollector.AddToIncludes(uniqueID, path);
		}

		protected void AddDefine(string define, ref MasterNodeDataCollector dataCollector)
		{
			int uniqueID = Guid.NewGuid().GetHashCode();
			dataCollector.AddToDefines(uniqueID, define);
		}

		protected virtual string GetFuncName()
		{
			string res = STR_EMPTY_FUNC;
			return res;
		}

		protected virtual void AddIncludes(ref MasterNodeDataCollector dataCollector)
		{
		
		}

		protected virtual void AddDefines(ref MasterNodeDataCollector dataCollector)
		{
		
		}

		protected virtual bool NeedsSceneDepthDiff()
		{
			return false;
		}

		protected virtual string GetParamsByInputPorts(ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			string res = string.Empty;

			for (int i = 0; i < m_inputPorts.Count; i++)
			{
				m_inputPorts[i].GeneratePortInstructions(ref dataCollector);
			}

			for (int i = 0; i < m_inputPorts.Count; i++)
			{
				string outputValue = customPorts[i].GenerateShaderForOutput(ref dataCollector, ignoreLocalvar);

				res += outputValue;

				if (i < m_inputPorts.Count - 1)
				{
					res += ", ";
				}
			}

			if (NeedsSceneDepthDiff())
			{
				res += ", sceneDepthDiff";
			}

			return res;
		}

		protected string GetFinalCalculation(string funcName, params string[] funcParams)
		{
			string paramsCombined = string.Empty;

			for (int i = 0; i < funcParams.Length; i++)
			{
				paramsCombined += funcParams[i];
				if (i < funcParams.Length - 1)
				{
					paramsCombined += ", ";
				}
			}

			string res = string.Format(funcName, paramsCombined);
			return res;
		}

		public override void OnConnectedOutputNodeChanges(int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type)
		{
			UpdateConnection(inputPortId);
		}

		public override void OnInputPortConnected(int inputPortId, int otherNodeId, int otherPortId, bool activateNode = true)
		{
			base.OnInputPortConnected(inputPortId, otherNodeId, otherPortId, activateNode);
			UpdateConnection(inputPortId);
			ApplyDefaultValueToDisconnectedPorts();
		}

		public override void OnInputPortDisconnected(int portId)
		{
			ApplyDefaultValue(portId);
			ApplyDefaultValueToDisconnectedPorts();
		}

		void UpdateConnection(int portId)
		{
			if (m_adjustPorts)
			{
				m_inputPorts[portId].MatchPortToConnection();

				WirePortDataType mostPriorityType = WirePortDataType.OBJECT;
				for (int i = 0; i < m_inputPorts.Count; i++)
				{
					if (UIUtils.GetPriority(m_inputPorts[i].DataType) > UIUtils.GetPriority(mostPriorityType))
					{
						mostPriorityType = m_inputPorts[i].DataType;
					}
				}

				m_outputPorts[0].ChangeType(mostPriorityType, false);
			}
		}

		private void ApplyDefaultValue(int portId)
		{
			customPorts[portId].ApplyDefaultValue();
		}

		public override void Draw(DrawInfo drawInfo)
		{
			base.Draw(drawInfo);
			Rect newPos = m_remainingBox;
			newPos.x += newPos.width * 0.3f * drawInfo.InvertedZoom;
			newPos.y += newPos.height * 0.3f * drawInfo.InvertedZoom;
			m_adjustPorts = EditorGUI.Toggle(newPos, m_adjustPorts);
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_adjustPorts = EditorGUILayout.Toggle("Auto-Adjust", m_adjustPorts);
		}

		protected abstract string CustomGenerateShaderForOutput(int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar);

		protected string GenerateShaderForOutputDefault(int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			AddIncludes(ref dataCollector);
			AddDefines(ref dataCollector);

			// If local variable is already created then you need only to re-use it
			if (m_outputPorts[0].IsLocalValue(portCategory))
			{
				return m_outputPorts[0].LocalValue(portCategory);
			}

			string funcName = GetFuncName();
			string funcParams = GetParamsByInputPorts(ref dataCollector, ignoreLocalvar);

			string finalCalculation = GetFinalCalculation(funcName, funcParams);

			//Register the final operation on a local variable associated with our output port
			RegisterLocalVariable(0, finalCalculation, ref dataCollector, "myLocalVar" + OutputId);
			
			// Use the newly created local variable
			return m_outputPorts[0].LocalValue(portCategory);
		}

		public override string GenerateShaderForOutput(int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			return CustomGenerateShaderForOutput(outputId, ref dataCollector, ignoreLocalvar);
		}

		public override void WriteToString(ref string nodeInfo, ref string connectionsInfo)
		{
			base.WriteToString(ref nodeInfo, ref connectionsInfo);
			IOUtils.AddFieldValueToString(ref nodeInfo, m_adjustPorts);
		}

		public override void ReadFromString(ref string[] nodeParams)
		{
			base.ReadFromString(ref nodeParams);
			m_adjustPorts = Convert.ToBoolean(GetCurrentParam(ref nodeParams));
		}
	}
}
#endif