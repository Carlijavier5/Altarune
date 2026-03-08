using System;
using UnityEngine;

namespace AllIn1ShaderNodes
{
	public class DemoElement : MonoBehaviour
	{
		public string label;
		public DemoElementUI demoElementUI;

		public void Init()
		{
			demoElementUI.Init(this);
		}

		private void OnValidate()
		{
			if(demoElementUI != null)
			{
				demoElementUI.UpdateGUI(this);
			}
		}
	}
}