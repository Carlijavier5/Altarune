using UnityEngine;
using TMPro;

namespace AllIn1ShaderNodes
{
	public class DemoElementUI : MonoBehaviour
	{
		public TMP_Text txtLabel;

		public void Init(DemoElement demoElement)
		{
			UpdateGUI(demoElement);
		}

		public void UpdateGUI(DemoElement demoElement)
		{
			txtLabel.text = demoElement.label;
		}
	}
}