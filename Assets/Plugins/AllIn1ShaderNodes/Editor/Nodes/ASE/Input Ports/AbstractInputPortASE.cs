#if AMPLIFY_SHADER_EDITOR
using AmplifyShaderEditor;

namespace AllIn1ShaderNodes
{
	public abstract class AbstractInputPortASE
	{
		public InputPort inputPort;

		public AbstractInputPortASE(InputPort inputPort)
		{
			this.inputPort = inputPort;
		}

		public void SetInputPort(InputPort inputPort)
		{
			this.inputPort = inputPort;
		}

		public abstract void ApplyDefaultValue();

		public bool IsConnected()
		{
			bool res = false;

			if(inputPort != null)
			{
				res = inputPort.IsConnected;
			}

			return res;
		}

		public virtual string GenerateShaderForOutput(ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			string res = string.Empty;

			if (IsConnected())
			{
				res = inputPort.GenerateShaderForOutput(ref dataCollector, ignoreLocalvar);
			}
			else
			{
				res = GenerateNotConnectedCode(ref dataCollector, ignoreLocalvar);
			}

			return res;
		}

		public virtual string GenerateNotConnectedCode(ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
		{
			string res = inputPort.GenerateShaderForOutput(ref dataCollector, ignoreLocalvar);
			return res;
		}
	}
}
#endif