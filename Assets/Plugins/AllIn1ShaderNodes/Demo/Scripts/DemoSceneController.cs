using UnityEngine;
using TMPro;
using AllIn1SpriteShader;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

namespace AllIn1ShaderNodes
{
    public class DemoSceneController : MonoBehaviour
    {
		[Header("References")]
		public Camera cam;
		public TextMeshProUGUI demoNameText;
		public TextMeshProUGUI informationText;
		public AllIn1DemoScaleTween informationTween;

		[Header("Demo Elements")]
		public GameObject[] demoElementPrefabs;

		[Header("Demo Elements Parent")]
		public GameObject demoElementsParent;

		[Header("Lerp Speed")]
		public float lerpSpeed;

		[Header("Pre-instantiated Instances")]
		[SerializeField] private DemoExpositor[] demoElementInstances;

		private int currentDemoIndex;
		private DemoExpositor currentDemoElement;
		private DemoExpositor lastDemoElement;
		private bool isTransitioning;

		private EventSystem eventSystem;

		[ContextMenu("Create Demo Instances")]
       private void CreateDemoInstances()
       {
          // Clean up existing instances
          ClearExistingInstances();

          if(demoElementPrefabs == null || demoElementPrefabs.Length == 0)
          {
             Debug.LogWarning("No demo element prefabs assigned!");
             return;
          }

          demoElementInstances = new DemoExpositor[demoElementPrefabs.Length];

          for(int i = 0; i < demoElementPrefabs.Length; i++)
          {
             if(demoElementPrefabs[i] == null)
             {
                Debug.LogWarning($"Demo element prefab at index {i} is null!");
                continue;
             }

             GameObject instance = InstantiatePrefabConnected(demoElementPrefabs[i], demoElementsParent.transform);
             instance.name = demoElementPrefabs[i].name + "_Instance";
             instance.SetActive(false);

             demoElementInstances[i] = instance.GetComponent<DemoExpositor>();
             
             if(demoElementInstances[i] == null)
             {
                Debug.LogError($"Prefab {demoElementPrefabs[i].name} doesn't have DemoExpositor component!");
             }
          }

          demoElementInstances[0].gameObject.SetActive(true);
          Debug.Log($"Created {demoElementInstances.Length} demo instances (prefab-connected)");
       }

       private GameObject InstantiatePrefabConnected(GameObject prefab, Transform parent)
       {
#if UNITY_EDITOR
          if(Application.isPlaying)
          {
             // In play mode, use regular instantiate
             return Instantiate(prefab, parent);
          }
          else
          {
             // In edit mode, use PrefabUtility to maintain connection
             GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
             return instance;
          }
#else
          // In builds, always use regular instantiate
          return Instantiate(prefab, parent);
#endif
       }

		private void ClearExistingInstances()
		{
			if(demoElementInstances != null)
			{
				for(int i = 0; i < demoElementInstances.Length; i++)
				{
					if(demoElementInstances[i] != null)
					{
						if(Application.isPlaying)
							Destroy(demoElementInstances[i].gameObject);
						else
							DestroyImmediate(demoElementInstances[i].gameObject);
					}
				}
			}
		}

		private void Start()
		{
			if(demoElementInstances == null || demoElementInstances.Length == 0)
			{
				Debug.LogError("No demo instances found! Use Context Menu 'Create Demo Instances' first.");
				return;
			}

			CheckEventSystem();

			// Ensure all instances are deactivated
			for (int i = 0; i < demoElementInstances.Length; i++)
			{
				if(demoElementInstances[i] != null)
				demoElementInstances[i].gameObject.SetActive(false);
			}

			isTransitioning = false;
			currentDemoIndex = 0;
			ShowCurrentDemo(DemoMovementDir.RIGHT);
		}

		private void CheckEventSystem()
		{
			EventSystem eventSystemInScene = GameObject.FindAnyObjectByType<EventSystem>();
			if (eventSystemInScene != null)
			{
				GameObject.Destroy(eventSystemInScene.gameObject);
			}

			GameObject goEventSystem = new GameObject("Event System");
			eventSystem = goEventSystem.AddComponent<EventSystem>();

#if ENABLE_INPUT_SYSTEM
			goEventSystem.AddComponent<InputSystemUIInputModule>();
#elif ENABLE_LEGACY_INPUT_MANAGER
			goEventSystem.AddComponent<StandaloneInputModule>();
#endif
		}

		public void Update()
       {
          // Block input during transitions
          if(isTransitioning) return;

		  if (AllIn1InputSystem.GetKeyDown(KeyCode.RightArrow) || AllIn1InputSystem.GetKeyDown(KeyCode.D))
          {
             NextDemo();
          }
          if (AllIn1InputSystem.GetKeyDown(KeyCode.LeftArrow) || AllIn1InputSystem.GetKeyDown(KeyCode.A))
          {
             PreviousDemo();
          }
       }

       public void NextDemo()
       {
          currentDemoIndex++;
          ClampDemoIndex();
          ShowCurrentDemo(DemoMovementDir.RIGHT);
       }

		public void PreviousDemo()
       {
          currentDemoIndex--;
          ClampDemoIndex();
          ShowCurrentDemo(DemoMovementDir.LEFT);
       }

       private void ShowCurrentDemo(DemoMovementDir dir)
       {
          if(demoElementInstances == null || currentDemoIndex >= demoElementInstances.Length)
          {
             Debug.LogError("Invalid demo instance index or instances not created!");
             return;
          }

          if(isTransitioning) return; // Extra safety check

          StartCoroutine(HandleDemoTransition(dir));
       }

       private System.Collections.IEnumerator HandleDemoTransition(DemoMovementDir dir)
       {
          isTransitioning = true;

          lastDemoElement = currentDemoElement;

          // Get the pre-instantiated element
          currentDemoElement = demoElementInstances[currentDemoIndex];
          
          if(currentDemoElement == null)
          {
             Debug.LogError($"Demo instance at index {currentDemoIndex} is null!");
             isTransitioning = false;
             yield break;
          }

          // Update the demo name text
          UpdateDemoNameText();

          // Activate and initialize the current demo element
          currentDemoElement.gameObject.SetActive(true);
          currentDemoElement.Init(cam, lerpSpeed);

          if (lastDemoElement != null)
          {
             // Setup positions and start animations
             currentDemoElement.PlaceNextTo(lastDemoElement, dir);
             currentDemoElement.MoveToCenter();

             // Start exit animation for previous element
             if(dir == DemoMovementDir.LEFT)
             {
                lastDemoElement.MoveToLeft();
             }
             else
             {
                lastDemoElement.MoveToRight();
             }

             // Wait for animations to complete
             // This assumes your DemoExpositor has animation duration based on lerpSpeed
             // Adjust this timing based on your actual animation implementation
             float animationDuration = 1f / lerpSpeed;
             yield return new WaitForSeconds(animationDuration);

             // Deactivate the previous demo element after exit animation completes
             lastDemoElement.gameObject.SetActive(false);
          }

          isTransitioning = false;
       }

       private void UpdateDemoNameText()
       {
          if(demoNameText == null) return;

          if(currentDemoElement != null && demoElementPrefabs != null && currentDemoIndex < demoElementPrefabs.Length)
          {
             demoNameText.text = demoElementInstances[currentDemoIndex].expositorName;
             informationText.text = demoElementInstances[currentDemoIndex].expositorDescription;
             informationTween.ScaleUpTween();
          }
          else
          {
             demoNameText.text = "Unknown Demo";
          }
       }

       private void ClampDemoIndex()
       {
          if(demoElementInstances == null) return;

          if (currentDemoIndex >= demoElementInstances.Length)
          {
             currentDemoIndex = 0;
          }
          else if (currentDemoIndex < 0)
          {
             currentDemoIndex = demoElementInstances.Length - 1;
          }
       }
    }
}