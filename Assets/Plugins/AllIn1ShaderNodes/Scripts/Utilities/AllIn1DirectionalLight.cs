#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace AllIn1ShaderNodes
{
	[ExecuteInEditMode]
	public class AllIn1DirectionalLight : MonoBehaviour
	{
		private Light directionalLight;

		private void Reset()
		{
			Configure();
		}

		private void OnEnable()
		{
			Configure();
		}

		private void Configure()
		{
			int missingScriptCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObject);

			if (missingScriptCount > 0)
			{
				GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);

#if ALLIN13DSHADER_BIRP
				ConfigureBIRP();
#elif ALLIN13DSHADER_URP
			ConfigureURP();
#elif ALLIN13DSHADER_HDRP
			ConfigureHDRP();
#endif
			}
		}

#if ALLIN13DSHADER_BIRP
		private void ConfigureBIRP()
		{
			directionalLight = transform.GetComponent<Light>();

			if (directionalLight == null)
			{
				directionalLight = gameObject.AddComponent<Light>();
			}

			directionalLight.intensity = 1.0f;
			directionalLight.type = LightType.Directional;
			directionalLight.useColorTemperature = false;
		}
#endif

#if ALLIN13DSHADER_URP
	private void ConfigureURP()
	{
		directionalLight = transform.GetComponent<Light>();

		if (directionalLight == null)
		{ 
			directionalLight = gameObject.AddComponent<Light>();
		}

		directionalLight.intensity = 1.0f;
		directionalLight.type = LightType.Directional;
	}
#endif

#if ALLIN13DSHADER_HDRP
	private void ConfigureHDRP()
	{
		directionalLight = transform.GetComponent<Light>();

		directionalLight.intensity = 1000f;
		directionalLight.type = LightType.Directional;
	}

#endif
	}
}
#endif