using Adobe.Substance;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Adobe.SubstanceEditor.ProjectSettings
{
    /// <summary>
    /// Settings provider for the Adobe Substance tab in the Unity project settings UI.
    /// </summary>
    internal class SubstanceEditorSettingsProvider : SettingsProvider
    {
        private SerializedObject _editorSettings;

        private SerializedProperty _generateAllTextureProp;

        private SerializedProperty _targetResolutionProp;

        private SerializedProperty _searchSubfoldersOnly;

        private SerializedProperty _disableRuntimeOnly;

        private static string SubstanceAssetLogoPath => $"{PathUtils.SubstanceRootPath}/Editor/Assets/S_3DHeart_18_N_nudged.png";
        private static string SubstanceCommunityAssetLogoPath => $"{PathUtils.SubstanceRootPath}/Editor/Assets/S_3DCummunityAssets_18_N.png";
        private const string SubstanceCommunityURL = "https://substance3d.adobe.com/community-assets";
        private const string SubstanceAssetURL = "https://substance3d.adobe.com/assets";
        private const string SearchSubfoldersOnlyPopup = "WARNING: Enabling this optimization requires graphs to be placed in subfolder of the .sbsar file following the naming convention {FILE_NAME}_{GRAPH_NAME}. Graphs that don't follow this convention will no longer be listed in the .sbsar file UI.";
        private const string DisableRuntimeOnlyPopup = "WARNING: Disabling support for 'Runtime Only' will automatically remove that option from all graphs in the project.";

        private class Contents
        {
            public static readonly GUIContent GenerateAllTexturesText = new GUIContent("Generate all outputs", "Generate all output textures for the substance graphs.");
            public static readonly GUIContent TextureResoltuionText = new GUIContent("Texture resolution", "Default output texture resolution for all graphs outputs.");
            public static readonly GUIContent SubstanceAssetsIcon = new GUIContent();
            public static readonly GUIContent SubstanceCommunityIcon = new GUIContent();
            public static readonly GUIContent SearchSubfoldersOnly = new GUIContent("Only search graphs in subfolders", "Restrict the search of graphs to subfolders of sbsar file that follow the naming convention {FILE_NAME}_{GRAPH_NAME}. This can improve Editor UI performance for projects with a big number of substance graphs, but it requires graph folders to follow the original naming convention.");
            public static readonly GUIContent DisableRuntimeOnly = new GUIContent("Disable support for 'Runtime Only' graphs", "Remove the support for graphs that are marked as 'Runtime Only'. This will foce graphs to generate textures as PNG files in the project.");
        }

        private class Styles
        {
            public static GUIStyle AssetButtonsStyle;
            public static readonly GUIStyle SubstanceAssetButtonsPanelStyle = new GUIStyle();
            public static readonly GUIStyle RichTextStyle = new GUIStyle() { richText = true };
        }

        public SubstanceEditorSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _editorSettings = SubstanceEditorSettingsSO.GetSerializedSettings();
            _generateAllTextureProp = _editorSettings.FindProperty("_generateAllTexture");
            _targetResolutionProp = _editorSettings.FindProperty("_targetResolution");
            _searchSubfoldersOnly = _editorSettings.FindProperty("_searchSubfoldersOnly");
            _disableRuntimeOnly = _editorSettings.FindProperty("_disableRuntimeOnly");


            Contents.SubstanceAssetsIcon.image = AssetDatabase.LoadAssetAtPath<Texture2D>(SubstanceAssetLogoPath);
            Contents.SubstanceAssetsIcon.tooltip = SubstanceAssetURL;

            Contents.SubstanceCommunityIcon.image = AssetDatabase.LoadAssetAtPath<Texture2D>(SubstanceCommunityAssetLogoPath);
            Contents.SubstanceCommunityIcon.tooltip = SubstanceCommunityURL;
        }

        public override void OnGUI(string searchContext)
        {
            _editorSettings.Update();

            if (Styles.AssetButtonsStyle == null)
            {
                Styles.AssetButtonsStyle = new GUIStyle(GUI.skin.label);
                Styles.AssetButtonsStyle.fixedHeight = 24;
                Styles.AssetButtonsStyle.fixedWidth = 24;
            }
            
            EditorGUILayout.Space();

            EditorGUI.indentLevel++;
            {
                DrawGeneralSettings();

                DrawPerformanceSettings();

                EditorGUILayout.Space();

                DrawTextLinksAndAbout();
            }
            EditorGUI.indentLevel--;

            _editorSettings.ApplyModifiedProperties();
        }

        private void DrawGeneralSettings()
        {
            EditorDrawUtilities.DrawUILine();

            EditorGUILayout.LabelField("General Settings:", EditorStyles.boldLabel);

            if (_generateAllTextureProp != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_generateAllTextureProp, Contents.GenerateAllTexturesText, GUILayout.Width(100));
            }

            if (_targetResolutionProp != null)
            {
                EditorGUILayout.Space();
                EditorDrawUtilities.DrawResolutionSelection(_targetResolutionProp, Contents.TextureResoltuionText);
            }

            DrawEngineInfo();
        }

        #region Performance Settings

        private void DrawPerformanceSettings()
        {
            EditorGUILayout.Space();
            EditorDrawUtilities.DrawUILine();
            EditorGUILayout.LabelField("Editor performance optimizations:", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (_searchSubfoldersOnly != null)
                DrawSearchSubfoldersOnly();

            if (_disableRuntimeOnly != null)
            {
                DrawDisableRuntimeOnly();
            }

            EditorDrawUtilities.DrawUILine();
        }

        private const int PERFORMANCE_TEXT_SIZE = 275;

        private void DrawSearchSubfoldersOnly()
        {
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(Contents.SearchSubfoldersOnly, GUILayout.Width(PERFORMANCE_TEXT_SIZE));
                var toggleValue = EditorGUILayout.Toggle(GUIContent.none, _searchSubfoldersOnly.boolValue, GUILayout.MinWidth(50));

                if (toggleValue != _searchSubfoldersOnly.boolValue)
                {
                    if (toggleValue)
                    {
                        if (EditorUtility.DisplayDialog("WARNING", SearchSubfoldersOnlyPopup, "Confirm", "Cancel"))
                        {
                            _searchSubfoldersOnly.boolValue = toggleValue;                        
                        }
                    }
                    else
                    {
                        _searchSubfoldersOnly.boolValue = toggleValue;
                    }

                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawDisableRuntimeOnly()
        {
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(Contents.DisableRuntimeOnly, GUILayout.Width(PERFORMANCE_TEXT_SIZE));
                var toggleValue = EditorGUILayout.Toggle(GUIContent.none, _disableRuntimeOnly.boolValue, GUILayout.MinWidth(50));

                if (toggleValue != _disableRuntimeOnly.boolValue)
                {
                    if (toggleValue)
                    {
                        if (EditorUtility.DisplayDialog("WARNING", DisableRuntimeOnlyPopup, "Confirm", "Cancel"))
                        {
                            _disableRuntimeOnly.boolValue = toggleValue;
                            SubstanceEditorEngine.instance.DisableAllRuntimeOnly();
                        }
                    }
                    else
                    {
                        _disableRuntimeOnly.boolValue = toggleValue;
                    }

                }
            }
            GUILayout.EndHorizontal();
        }

        #endregion

        private void DrawEngineInfo()
        {
            string label = PlatformUtils.IsCPU() ? "CPU" : "GPU";
            var content = new GUIContent($"Computing textures with: {label}", "Engine used for rendering textures");

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(content);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawClickableText(string text, GUIStyle style, Action callback)
        {
            var labelRect = EditorGUILayout.GetControlRect();

            if (Event.current.type == EventType.MouseUp && labelRect.Contains(Event.current.mousePosition))
                callback();

            GUI.Label(labelRect, text, style);
        }

        private void DrawTextLinksAndAbout()
        {
            var textStyle = new GUIStyle();
            textStyle.normal.textColor = new Color(75f / 255f, 122f / 255f, 243f / 255f);
            textStyle.alignment = TextAnchor.LowerLeft;
            textStyle.fixedWidth = 150;

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space(10, false);

                if (GUILayout.Button(Contents.SubstanceAssetsIcon, Styles.AssetButtonsStyle))
                    Application.OpenURL(SubstanceAssetURL);

                DrawClickableText("Substance 3D assets", textStyle, () => Application.OpenURL(SubstanceAssetURL));
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space(10, false);

                if (GUILayout.Button(Contents.SubstanceCommunityIcon, Styles.AssetButtonsStyle))
                    Application.OpenURL(SubstanceCommunityURL);

                DrawClickableText("Substance 3D community assets", textStyle, () => Application.OpenURL(SubstanceCommunityURL));
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space(40, false);
                DrawClickableText("About", textStyle, () => Extensions.DrawAboutWindow());
            }
            GUILayout.EndHorizontal();
        }   

        #region Registration

        [SettingsProvider]
        public static SettingsProvider CreateSubstanceSettingsProvider()
        {
            if (SubstanceEditorSettingsSO.IsSettingsAvailable())
            {
                return new SubstanceEditorSettingsProvider("Project/Adobe Substance 3D", SettingsScope.Project)
                {
                    label = "Adobe Substance 3D",
                    keywords = GetSearchKeywordsFromGUIContentProperties<Contents>()
                };
            }

            return null;
        }

        #endregion Registration
    }
}