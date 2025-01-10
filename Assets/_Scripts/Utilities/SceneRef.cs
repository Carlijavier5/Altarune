using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// Utility sourced from https://github.com/NibbleByte/UnitySceneReference/tree/master;
/// All due credits to @NibbleByte and @JohannesMP for their commendable work on this asset;
/// Modifications were made to the source files for dedicated usage in the present project, Altarune.
/// The original work and henceforth this single script are protected under the following license:
///
/// MIT License
///
/// Copyright(c) 2021 Filip Slavov
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in all
/// copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
/// SOFTWARE.
/// </summary>

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[System.Serializable]
public class SceneRef : ISerializationCallbackReceiver {

	#if UNITY_EDITOR
	[SerializeField] private SceneAsset sceneAsset;
	#pragma warning disable 0414
	[SerializeField] private bool isDirty;
	#pragma warning restore 0414
	#endif

	[SerializeField] private string scenePath = string.Empty;

	/// <summary>
	/// Returns the scene path to be used in the <see cref="UnityEngine.SceneManagement.SceneManager"/> API.
	/// While in the editor, this path will always be up to date (if asset was moved or renamed).
	/// If the referred scene asset was deleted, the path will remain as is.
	/// </summary>
	public string ScenePath {
		get {
			#if UNITY_EDITOR
			AutoUpdateReference();
			#endif
			return scenePath;
		}

		set {
			scenePath = value;

			#if UNITY_EDITOR
			if (string.IsNullOrEmpty(scenePath)) {
				sceneAsset = null;
				return;
			}

			sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
			if (sceneAsset == null) {
				Debug.LogError($"Setting {nameof(SceneRef)} to {value}, but no scene could be located there.");
			}
			#endif
		}
	}

	private int buildIndex = -1;

	public int BuildIndex {
		get {
			if (!IsEmpty && buildIndex < 0) {
				buildIndex = SceneUtility.GetBuildIndexByScenePath(scenePath);
            }
			return buildIndex;
        }
    }

	public bool IsEmpty => string.IsNullOrEmpty(ScenePath);

	public SceneRef() { }

	public SceneRef(string scenePath) {
		ScenePath = scenePath;
	}

	public SceneRef(SceneRef other) {
		scenePath = other.scenePath;

		#if UNITY_EDITOR
		sceneAsset = other.sceneAsset;
		isDirty = other.isDirty;

		AutoUpdateReference();
		#endif
	}

	#if UNITY_EDITOR
	private static bool engineIsReloadingAssemblies = false;

	static SceneRef() {
		AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
	}

	private static void OnBeforeAssemblyReload() {
		engineIsReloadingAssemblies = true;
	}
	#endif

	public SceneRef Clone() => new(this);

	public override string ToString() {
		return scenePath;
	}

	[System.Obsolete("Needed for the editor, don't use it in runtime code!", true)]
	public void OnBeforeSerialize() {
		#if UNITY_EDITOR
		// In rare cases this error may be logged when trying to change SceneReference while assembly is reloading:
		// "Objects are trying to be loaded during a domain backup. This is not allowed as it will lead to undefined behaviour!"
		if (engineIsReloadingAssemblies) return;

		AutoUpdateReference();
		#endif
	}

	[System.Obsolete("Needed for the editor, don't use it in runtime code!", true)]
	public void OnAfterDeserialize() {
		#if UNITY_EDITOR
		// OnAfterDeserialize is called in the deserialization thread so we can't touch Unity API.
		// Wait for the next update frame to do it.
		EditorApplication.update += OnAfterDeserializeHandler;
		#endif
	}


	#if UNITY_EDITOR
	private void OnAfterDeserializeHandler() {
		EditorApplication.update -= OnAfterDeserializeHandler;
		AutoUpdateReference();
	}

	private void AutoUpdateReference() {
		if (sceneAsset == null) {
			if (string.IsNullOrEmpty(scenePath)) return;

			SceneAsset foundAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
			if (foundAsset) {
				sceneAsset = foundAsset;
				isDirty = true;

				if (!Application.isPlaying) {
					EditorSceneManager.MarkAllScenesDirty();
				}
			}
		} else {
			string foundPath = AssetDatabase.GetAssetPath(sceneAsset);
			if (string.IsNullOrEmpty(foundPath)) return;

			if (foundPath != scenePath) {
				scenePath = foundPath;
				isDirty = true;

				if (!Application.isPlaying) {
					EditorSceneManager.MarkAllScenesDirty();
				}
			}
		}
	}
	#endif
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneRef))] [CanEditMultipleObjects]
internal class SceneRefPropertyDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		SerializedProperty isDirtyProperty = property.FindPropertyRelative("isDirty");
		if (isDirtyProperty.boolValue) {
			/// Marks the asset as dirty;
			isDirtyProperty.boolValue = false;
		}

		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		const float cueWidth = 18f;
		const float startPadding = 1f;
		const float midPadding = 5f;

		Rect assetPos = new(position) { x = position.x + startPadding,
										width = position.width - cueWidth - midPadding - startPadding};

		Rect settingsCue = new(position) { x = position.x + position.width - cueWidth - startPadding,
										   width = cueWidth };

		var sceneAssetProperty = property.FindPropertyRelative("sceneAsset");
		bool hadReference = sceneAssetProperty.objectReferenceValue != null;

		EditorGUI.ObjectField(assetPos, sceneAssetProperty, typeof(SceneAsset), new GUIContent());

		string guid = string.Empty;
		int indexInSettings = -1;

		if (sceneAssetProperty.objectReferenceValue) {
			if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sceneAssetProperty.objectReferenceValue, out guid, out long localId)) {
				indexInSettings = System.Array.FindIndex(EditorBuildSettings.scenes, s => s.guid.ToString() == guid);
			}
		} else if (hadReference) {
			property.FindPropertyRelative("scenePath").stringValue = string.Empty;
		}

		GUIContent settingsContent = indexInSettings >= 0 ? new GUIContent("-", "Scene is already in the Editor Build Settings. Click here to remove it")
								   : hadReference ? new GUIContent("+", "Scene is missing in the Editor Build Settings. Click here to add it")
												  : new GUIContent("?", "No scene assigned");

		Color prevBackgroundColor = GUI.backgroundColor;
		GUI.backgroundColor = indexInSettings >= 0 ? CJUtils.UIColors.Red  
							: hadReference ? CJUtils.UIColors.Green
										   : CJUtils.UIColors.Yellow;

		GUIStyle messageStyle = hadReference ? new(EditorStyles.miniButtonRight) { fontSize = 16,
																				   contentOffset = new Vector2(1, -1),
																				   alignment = TextAnchor.MiddleCenter }
							  : new(EditorStyles.helpBox) { fontSize = 12,
														    fontStyle = FontStyle.Bold,
															contentOffset = new Vector2(2, 0),
															alignment = TextAnchor.MiddleCenter };
		if (hadReference) {
			if (GUI.Button(settingsCue, settingsContent, messageStyle) && sceneAssetProperty.objectReferenceValue) {
				if (indexInSettings != -1) {
					var scenes = EditorBuildSettings.scenes.ToList();
					scenes.RemoveAt(indexInSettings);

					EditorBuildSettings.scenes = scenes.ToArray();

				} else {
					EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[] {
					new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(sceneAssetProperty.objectReferenceValue), true)
				};

					EditorBuildSettings.scenes = EditorBuildSettings.scenes.Concat(newScenes).ToArray();
				}
			}
		} else {
			EditorGUI.LabelField(settingsCue, settingsContent, messageStyle);
        }

		GUI.backgroundColor = prevBackgroundColor;
		EditorGUI.EndProperty();
	}
}
#endif