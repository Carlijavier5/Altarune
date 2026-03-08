using TMPro;
using UnityEngine;

namespace AllIn1ShaderNodes
{
	public class DemoUI : MonoBehaviour
	{
		//public AllIn13dShaderDemoController demoController;
		public TMP_Text txtExpositorName, txtDemoName, txtDemoViewing;
		//public AllIn1DemoScaleTween viewingTween;

		//public DemoInfoUI demoInfoUI;

		public void Refresh(DemoExpositor currentExpositor, DemoElement demoElement)
		{
			//int currentExpositorIndex = demoController.GetCurrentExpositorIndex() + 1;
			//int currentDemoElementIndex = demoController.CurrentExpositor.CurrentDemoElementIndex + 1;
			//txtExpositorName.text = $"{currentExpositorIndex}. {currentExpositor.expositorName}";
			//txtDemoName.text = $"{currentDemoElementIndex}. {demoElement.demoName}";

			//demoInfoUI.DemoChanged(demoElementData);
			
			//txtDemoViewing.text = $"Viewing: {currentExpositorIndex} - {currentDemoElementIndex}";
			//viewingTween.ScaleUpTween();
		}

		public void ShowOrHideDemoInfo()
		{
			//demoInfoUI.ShowHideToggle();
		}
	}
}