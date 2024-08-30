using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
# if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Search;
# endif

/// <summary>
/// A few utility classes wrote by yours truly ;D
/// </summary>
namespace CJUtils {

    public static class UIColors {

        public static Color Blue {
            get {
                return new Vector4(0.861f, 0.925f, 0.994f, 1);
            }
        }

        public static Color Green {
            get {
                return new Vector4(0.825f, 0.99f, 0.99f, 1);
            }
        }

        public static Color Red {
            get {
                return new Vector4(0.99f, 0.825f, 0.825f, 1);
            }
        }

        public static Color DarkBlue {
            get {
                return new Vector4(0.3f, 0.8f, 1.0f, 1);
            }
        }

        public static Color DefinedBlue {
            get {
                return new Vector4(0.75f, 1.2f, 2f, 0.8f);
            }
        }

        public static Color DefinedGreen {
            get {
                return new Vector4(1f, 1.85f, 0.5f, 0.8f);
            }
        }

        public static Color MidBlue {
            get {
                return new Vector4(0.75f, 0.9f, 1.0f, 1);
            }
        }

        public static Color DarkGreen {
            get {
                return new Vector4(0.6f, 0.9f, 0.6f, 1);
            }
        }

        public static Color DarkRed {
            get {
                return new Vector4(1.0f, 0.5f, 0.5f, 1);
            }
        }

        public static Color Yellow {
            get {
                return new Vector4(0.9804f, 0.8156f, 0.1725f, 1);
            }
        }

        public static Color Cyan {
            get {
                return new Vector4(0.25f, 0.95f, 1.0f, 1);
            }
        }

        public static Color DarkGray {
            get {
                return new Vector4(0.1f, 0.1f, 0.1f, 0.6f);
            }
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// A collection of functions to facilitate changes to the Scene View and the Hierarchy Window;
    /// </summary>
    public static class SceneUtils {

        private const string SINGLE_OUTLINE_PREF_PATH = "Scene/Selected Outline";
        private const string CHILDREN_OUTLINE_PREF_PATH = "Scene/Selected Children Outline";

        /// <summary>
        /// Simple transform sorting method, based on a public rendition by <a href="https://gist.github.com/AShim3D/d76e2026c5655b3b34e2">AShim3D</a>;
        /// </summary>
        /// <param name="transform"> Transform whose children must be reordered; </param>
        public static void SortChildren(Transform transform) {
            List<Transform> children = new List<Transform>();
            for (int i = transform.childCount - 1; i >= 0; i--) {
                Transform child = transform.GetChild(i);
                children.Add(child);
                child.parent = null;
            }
            children.Sort((Transform t1, Transform t2) => { return t1.name.CompareTo(t2.name); });
            foreach (Transform child in children) {
                child.parent = transform;
            }
        }

        /// <summary>
        /// Accessor and setter for the Outline Color of the SceneView selection shader;
        /// </summary>
        public static Color SelectedOutlineSingleColor {
            get {
                string propertyString = EditorPrefs.GetString(SINGLE_OUTLINE_PREF_PATH);
                return ParsePrefColor(propertyString);
            } set {
                string propertyString = EditorPrefs.GetString(SINGLE_OUTLINE_PREF_PATH);
                EditorPrefs.SetString(SINGLE_OUTLINE_PREF_PATH, ReplacePrefColor(propertyString, value));
            }
        }

        /// <summary>
        /// Accessor and setter for the Children Outline Color of the SceneView selection shader;
        /// </summary>
        public static Color SelectedOutlineChildrenColor {
            get {
                string propertyString = EditorPrefs.GetString(CHILDREN_OUTLINE_PREF_PATH);
                return ParsePrefColor(propertyString);
            } set {
                string propertyString = EditorPrefs.GetString(CHILDREN_OUTLINE_PREF_PATH);
                EditorPrefs.SetString(CHILDREN_OUTLINE_PREF_PATH, ReplacePrefColor(propertyString, value));
            }
        }

        /// <summary>
        /// Parse the color at the end of a Pref string;
        /// </summary>
        /// <param name="propertyString"> Pref string; </param>
        /// <returns> Color at the end of the string; </returns>
        private static Color ParsePrefColor(string propertyString) {
            Color color = Color.white;
            propertyString = propertyString.RemovePathEnd(";", out string a);
            float.TryParse(a, out color.a);
            propertyString = propertyString.RemovePathEnd(";", out string b);
            float.TryParse(b, out color.b);
            propertyString = propertyString.RemovePathEnd(";", out string g);
            float.TryParse(g, out color.g);
            string r = propertyString.IsolatePathEnd(";");
            float.TryParse(r, out color.r);
            return color;
        }

        /// <summary>
        /// Replace the color value in a Unity Pref string;
        /// </summary>
        /// <param name="propertyString"> Pref string; </param>
        /// <param name="color"> Color to append; </param>
        /// <returns></returns>
        private static string ReplacePrefColor(string propertyString, Color color) {
            propertyString = propertyString.RemovePathEnd(";", 4);
            propertyString += $";{color.r};{color.g};{color.b};{color.a}";
            return propertyString;
        }
    }

    public enum GridAxis { XZ, XY, YZ }

    public static class GridUtils {

        private const int distance = 20;
        private const float offset = 0.5f;

        public static void DrawGrid(GridAxis axis, SceneView sceneView, int depth) {
            if (Event.current.type != EventType.Repaint) return;
            Vector3Int center = sceneView.camera.transform.position.Round();
            using (new Handles.DrawingScope(new Vector4(1, 1, 1, 0.25f))) {
                switch (axis) {
                    case GridAxis.XZ:
                        center = new Vector3Int(center.x, depth, center.z);
                        DrawXZGrid(center);
                        break;
                    case GridAxis.XY:
                        center = new Vector3Int(center.x, center.y, depth);
                        DrawXYGrid(center);
                        break;
                    case GridAxis.YZ:
                        center = new Vector3Int(depth, center.y, center.z);
                        DrawYZGrid(center);
                        break;
                }
            }
        }

        private static void DrawXZGrid(Vector3Int center) {
            for (int i = -distance; i <= distance; i++) {
                Handles.DrawLine(center + new Vector3(i, 0, -distance) - Vector3.one * offset,
                                 center + new Vector3(i, 0, distance) - Vector3.one * offset);
                Handles.DrawLine(center + new Vector3(distance, 0, i) - Vector3.one * offset,
                                 center + new Vector3(-distance, 0, i) - Vector3.one * offset);
            }
        }

        private static void DrawXYGrid(Vector3Int center) {
            for (int i = -distance; i < distance; i++) {
                Handles.DrawLine(center + new Vector3(i, -distance, 0) - Vector3.one * offset,
                                 center + new Vector3(i, distance, 0) - Vector3.one * offset);
                Handles.DrawLine(center + new Vector3(-distance, i, 0) - Vector3.one * offset,
                                 center + new Vector3(distance, i, 0) - Vector3.one * offset);
            }
        }

        private static void DrawYZGrid(Vector3Int center) {
            for (int i = -distance; i < distance; i++) {
                Handles.DrawLine(center + new Vector3(0, i, -distance) - Vector3.one * offset,
                                 center + new Vector3(0, i, distance) - Vector3.one * offset);
                Handles.DrawLine(center + new Vector3(0, -distance, i) - Vector3.one * offset,
                                 center + new Vector3(0, distance, i) - Vector3.one * offset);
            }
        }
    }

    /// <summary>
    /// A collection of utilities to draw custom handles in the SceneView;
    /// </summary>
    public static class HandleUtils {

        /// <summary>
        /// Draw a dotted octohedron, similar to Handles.DrawWireCube();
        /// </summary>
        public static void DrawDottedOctohedron(Vector3 center, Vector3 size,
                                                Color color, float thickness = 1) {
            if (Event.current.type == EventType.Repaint) {
                Vector3 bounds = size / 2;
                Vector3[] segments = new Vector3[24];
                Vector3[] leftFace = GetOctohedronFace(Vector3.left, bounds);
                Vector3[] rightFace = GetOctohedronFace(Vector3.right, bounds);
                int jointLength = leftFace.Length + rightFace.Length;
                for (int i = 0; i < jointLength; i++) {
                    if (i % 2 == 0) segments[i] = leftFace[i / 2]; 
                    else segments[i] = rightFace[i / 2];
                } leftFace.InjectSegments(segments, jointLength);
                rightFace.InjectSegments(segments, jointLength + leftFace.Length * 2);
                segments.Offset(center);
                using (new Handles.DrawingScope(color)) {
                    Handles.DrawDottedLines(segments, thickness);
                }
            }
        }

        /// <summary>
        /// Draw an octohedron with the specified center, size, and color;
        /// </summary>
        public static void DrawOctohedralVolume(Vector3 center, Vector3 size,
                                                Color surfaceColor, Color outlineColor) {
            if (Event.current.type == EventType.Repaint) {
                Vector3 bounds = size / 2;
                Vector3[] normals = new[] { Vector3.left, Vector3.right,
                                            Vector3.up, Vector3.down,
                                            Vector3.back, Vector3.forward };
                foreach (Vector3 normal in normals) {
                    DrawOctohedronFace(GetOctohedronFace(normal, bounds), center,
                                       surfaceColor, outlineColor);
                }
            }
        }

        /// <summary>
        /// Draw a face offset from a 
        /// </summary>
        /// <param name="vertices"> Origin-centered, bounding vertices of the face; </param>
        /// <param name="center"> Center of the octohedron whose face should be drawn; </param>
        private static void DrawOctohedronFace(Vector3[] vertices, Vector3 center,
                                               Color surfaceColor, Color outlineColor) {
            vertices.Offset(center);
            Handles.DrawSolidRectangleWithOutline(vertices, surfaceColor, outlineColor);
        }

        /// <summary>
        /// Compute the bounding points of a cube face;
        /// </summary>
        /// <param name="normal"> Normal of the face; </param>
        /// <param name="bounds"> Absolute bounds of the cube; </param>
        /// <returns></returns>
        private static Vector3[] GetOctohedronFace(Vector3 normal, Vector3 bounds) {
            /// Isolate constant bound through normal;
            Vector3 normalBound = VectorUtils.Mult(bounds, normal);
            /// Split remaining bounds;
            VectorUtils.SplitVectorByNormal(normal, bounds,
                            out Vector3 vec1, out Vector3 vec2);
            /// Gray-code non-normal bounds to build quad bounds;
            return new Vector3[] { -vec1 + vec2 + normalBound,
                                   vec1 + vec2 + normalBound,
                                   vec1 + -vec2 + normalBound,
                                   -vec1 + -vec2 + normalBound };
        }

        /// <summary>
        /// Draw an Arrow Handle that moves along a given axis;
        /// </summary>
        /// <param name="id"> ID of the handle control; </param>
        /// <param name="normal"> Axis to which the arrow is pointing; </param>
        /// <param name="size"> Size of the handle; </param>
        /// <param name="color"> Color of the handle; </param>
        /// <param name="position"> Handle position; </param>
        /// <param name="activeID"> Control ID selected within the calling GUI; </param>
        /// <param name="prevMousePos"> Position of the mouse on the previous frame; </param>
        /// <param name="mouseUpCallback"> Callback for the MouseUp editor event; </param>
        public static void DrawArrowHandle(int id, Vector3 normal, float size, float offset, Color color,
                                           ref Vector3 position, ref int activeID, ref Vector3 prevMousePos,
                                           System.Action mouseUpCallback = null) {
            if (Event.current.type == EventType.Layout) {
                Handles.ArrowHandleCap(id, position - normal * offset,
                                       Quaternion.LookRotation(normal),
                                       size, EventType.Layout);
            }

            if (Event.current.type == EventType.Repaint) {
                Handles.color = HandleUtility.nearestControl == id ? activeID == id ? Color.yellow
                                                                                    : color * 1.1f
                                                                   : color;
                Handles.ArrowHandleCap(id, position - normal * offset,
                                       Quaternion.LookRotation(normal),
                                       size, EventType.Repaint);
            }

            switch (Event.current.type) {
                case EventType.MouseDown:
                    activeID = HandleUtility.nearestControl;
                    prevMousePos = Event.current.mousePosition;
                    break;
                case EventType.MouseUp:
                    activeID = -1;
                    prevMousePos = Vector3.zero;
                    mouseUpCallback?.Invoke();
                    break;
            };

            if (Event.current.type == EventType.MouseDrag
                && Event.current.button == 0) {
                if (activeID == id) {
                    float move = HandleUtility.CalcLineTranslation(prevMousePos, Event.current.mousePosition,
                                                                   position + normal * offset, normal);
                    position += normal * move;
                } prevMousePos = Event.current.mousePosition;
            }
        }
    }

    /// <summary>
    /// A collection of functions to faciliate asset creation and access;
    /// </summary>
    public static class AssetUtils {

        /// <summary>
        /// Attempt to retrieve an arbitrary occurrence of a scriptable object asset;
        /// </summary>
        /// <typeparam name="T"> Type of the asset to retrieve; </typeparam>
        /// <param name="assetOutput"> Retrieved asset if any. Will be null if none is found; </param>
        /// <returns> True if an asset was found, false otherwise; </returns>
        public static bool TryRetrieveAsset<T>(out T assetOutput) where T : ScriptableObject {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).FullName}");
            if (guids.Length > 0) {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                assetOutput = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                return true;
            } else {
                assetOutput = null;
                return false;
            }
        }

        /// <summary>
        /// Create an asset of a given type. Does not overwrite files;
        /// </summary>
        /// <typeparam name="T"> Type of asset to create; </typeparam>
        /// <param name="explorerPrompt"> Name prompt for the file explorer; </param>
        /// <param name="defaultName"> Default name of the asset to create in the file explorer; </param>
        /// <returns> Created asset, null if asset ctreation failed; </returns>
        public static T CreateAsset<T>(string explorerPrompt, string defaultName) where T : ScriptableObject {
            string[] guids = AssetDatabase.FindAssets($"t:Script {typeof(T).Name}");
            string startPath = AssetDatabase.GUIDToAssetPath(guids[0]).RemovePathEnd("/");
            string location = EditorUtility.OpenFolderPanel(explorerPrompt, startPath, "").ToRelativePath();
            if (AssetDatabase.IsValidFolder(location)) {
                string targetLocation = ProduceValidAssetNotation(location, defaultName);
                T newAsset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(newAsset, targetLocation);
                AssetDatabase.Refresh();
                return newAsset;
            } Debug.LogWarning($"Failed to create asset at {location}");
            return null;
        }

        public static string ProduceValidAssetNotation(string location, string name, int appendix = 0) {
            string newName = location + $"/{name}{(appendix == 0 ? "" : " " + appendix)}.asset";
            return File.Exists(newName) ? ProduceValidAssetNotation(location, name, appendix + 1)
                                        : newName;
        }

        public static string ProduceValidAssetNotation(string location, string name,
                                                       string extension, int appendix = 0) {
            string newLocation = location + $"/{name}{(appendix == 0 ? "" : " " + appendix)}{extension}";
            return File.Exists(newLocation) ? ProduceValidAssetNotation(location, name, extension, appendix + 1)
                                        : newLocation;
        }

        public static string ProduceValidAssetName(string location, string name,
                                                   string extension, int appendix = 0) {
            string newName = $"{name}{(appendix == 0 ? "" : " " + appendix)}";
            string newLocation = location + $"/{newName}{extension}";
            return File.Exists(newLocation) ? ProduceValidAssetName(location, name, extension, appendix + 1)
                                            : newName;
        }

        /// <summary>
        /// Checks if there's no asset at the given path;
        /// </summary>
        /// <param name="path"> Path to check; </param>
        /// <returns> True if the asset <b>doesn't</b> exist; </returns>
        public static bool NoAssetAtPath(string path) => string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path,
                                                                              AssetPathToGUIDOptions.OnlyExistingAssets));

        /// <summary> Potential results for the name validation process; </summary>
        public enum InvalidNameCondition { None, Empty, Overwrite, Symbol, Convention, Success }

        /// <summary>
        /// Validate a filename in terms of content, convention, and File I/O availability;
        /// </summary>
        /// <returns> True if the name is valid, false otherwise; </returns>
        public static InvalidNameCondition ValidateFilename(string path, string name) {
            if (!NoAssetAtPath(path)) {
                return InvalidNameCondition.Overwrite;
            } else return ValidateFilename(name);
        }

        public static InvalidNameCondition ValidateFilename(string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                return InvalidNameCondition.Empty;
            }
            if (NameViolatesConvention(name)) {
                return InvalidNameCondition.Convention;
            }
            List<char> invalidChars = new (Path.GetInvalidFileNameChars());
            foreach (char character in name) {
                if (invalidChars.Contains(character)) {
                    return InvalidNameCondition.Symbol;
                }
            }
            return InvalidNameCondition.None;
        }

        /// <summary>
        /// Validate a filename for project conventions;
        /// <br></br> Included rules:
        /// <br></br> - Name cannot be null, empty, or whitespace;
        /// <br></br> - First letter is capitalized or underscore;
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns> True if a convention was violated, false otherwise; </returns>
        private static bool NameViolatesConvention(string fileName) {
            if (string.IsNullOrWhiteSpace(fileName)) return true;
            if (!char.IsUpper(fileName[0])
                && fileName[0] != '_') return true;
            return false;
        }
    }

    /// <summary>
    /// Utilities for drawing stuff to the Editor GUI;
    /// </summary>
    public static class GUIUtils {

        /// <summary>
        /// Draws a centered Label wrapped around a WindowBox scope;
        /// </summary>
        /// <param name="text"> Text to display on the label; </param>
        /// <param name="style"> Custom style for the label; </param>
        public static void WindowBoxLabel(string text, GUIStyle style, params GUILayoutOption[] options) {
            using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                GUILayout.Label(text, style, options);
            }
        }

        /// <summary>
        /// Draws a centered Label wrapped around a WindowBox scope;
        /// </summary>
        /// <param name="text"> Text to display on the label; </param>
        public static void WindowBoxLabel(string text, params GUILayoutOption[] options) {
            using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                GUILayout.Label(text, UIStyles.CenteredLabelBold, options);
            }
        }

        /// <summary>
        /// Draws a centered Label wrapped around a WindowBox scope;
        /// </summary>
        /// <param name="content"> Label content; </param>
        public static Rect WindowBoxLabel(GUIContent content, params GUILayoutOption[] options) {
            Rect rect;
            using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                rect = GUILayoutUtility.GetRect(18, 14);
                GUI.Label(rect, content.image);
                GUILayout.Label(content.text, UIStyles.CenteredLabelBold, options);
            } return rect;
        }
        
        /// <summary>
        /// Draws a fancy on/off button;
        /// </summary>
        public static void OnOffButton(bool toggle, out bool newToggle,
                                       params GUILayoutOption[] options) {
            GUI.color = toggle ? UIColors.Green : UIColors.Red;
            if (GUILayout.Button(toggle ? "On" : "Off", options)) {
                toggle = !toggle;
            } GUI.color = Color.white;
            newToggle = toggle;
        }

        /// <summary>
        /// Draws a fancy on/off button;
        /// </summary>
        public static void OnOffButton(bool toggle, out bool newToggle, 
                                       GUIStyle style, params GUILayoutOption[] options) {
            GUI.color = toggle ? UIColors.Green : UIColors.Red;
            if (GUILayout.Button(toggle ? "On" : "Off", style, options)) {
                toggle = !toggle;
            } GUI.color = Color.white;
            newToggle = toggle;
        }

        /// <summary>
        /// Draws a bold text label at the center of the current scope;
        /// </summary>
        /// <param name="text"> Text to display; </param>
        public static void DrawScopeCenteredText(string text) {
            GUILayout.FlexibleSpace();
            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.FlexibleSpace();
                GUILayout.Label(text, UIStyles.CenteredLabelBold);
                GUILayout.FlexibleSpace();
            } GUILayout.FlexibleSpace();
        }

        /// <summary>
        /// Draw a single separator line;
        /// </summary>
        public static void DrawSeparatorLine() => DrawSeparatorLine(Color.gray);

        /// <summary>
        /// Draw a single separator line;
        /// </summary>
        public static void DrawSeparatorLine(Color color) {
            using (var hscope = new EditorGUILayout.HorizontalScope()) {
                Handles.color = color;
                Handles.DrawLine(new Vector2(hscope.rect.x, hscope.rect.y), new Vector2(hscope.rect.xMax, hscope.rect.y));
                Handles.color = Color.white;
            }
        }

        /// <summary>
        /// Non-handle version of a line;
        /// </summary>
        /// <param name="height"> Height of the line in pixels; </param>
        public static void DrawSeparatorLine(int height) {
            GUILayout.Space(4);
            Rect rect = GUILayoutUtility.GetRect(1, height, GUILayout.ExpandWidth(true));
            rect.height = height;
            rect.xMin = 0;
            rect.xMax = EditorGUIUtility.currentViewWidth;
            EditorGUI.DrawRect(rect, Color.gray);
            GUILayout.Space(4);
        }

        /// <summary>
        /// Draws two horizontal lines with a title in the middle;
        /// </summary>
        /// <param name="title"> Text drawn between the lines; </param>
        public static void DrawSeparatorLines(string title, bool centered = false, bool spaceOut = true) {
            using (new EditorGUILayout.VerticalScope()) {
                if (spaceOut) EditorGUILayout.Space();
                DrawSeparatorLine();
                if (centered) GUILayout.Label(title, UIStyles.CenteredLabelBold);
                else EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                DrawSeparatorLine();
                if (spaceOut) EditorGUILayout.Space();
            }
        }

        /// <summary>
        /// Draws two labels on a horizontal scope, left and right aligned respectively;
        /// </summary>
        /// <param name="leftLabel"> Content of the left label; </param>
        /// <param name="rightLabel"> Content of the right label; </param>
        /// <param name="options"> Array of GUILayoutOptions to apply to the labels; </param>
        public static void DrawLabelPair(string leftLabel, string rightLabel, params GUILayoutOption[] options) {
            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.Label(leftLabel, options);
                GUILayout.Label(rightLabel, UIStyles.RightAlignedLabel, options);
            }
        }

        /// <summary>
        /// Draws two labels on a horizontal scope, left and right aligned respectively;
        /// </summary>
        /// <param name="leftLabel"> Content of the left label; </param>
        /// <param name="rightLabel"> Content of the right label; </param>
        /// <param name="style"> GUIStyle to apply to both labels (overriding alignment); </param>
        /// <param name="options"> Array of GUILayoutOptions to apply to the labels; </param>
        public static void DrawLabelPair(string leftLabel, string rightLabel, GUIStyle style, params GUILayoutOption[] options) {
            GUIStyle leftStyle = new GUIStyle(style) { alignment = TextAnchor.MiddleLeft };
            GUIStyle rightStyle = new GUIStyle(style) { alignment = TextAnchor.MiddleRight };
            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.Label(leftLabel, leftStyle, options);
                GUILayout.Label(rightLabel, rightStyle, options);
            }
        }

        /// <summary>
        /// Draws a Help Box with a custom icon;
        /// </summary>
        /// <param name="text"> Help Box message; </param>
        /// <param name="texture"> Help Box icon; </param>
        public static void DrawCustomHelpBox(string text, Texture texture, float width, float height) {
            DrawCustomHelpBox(text, texture, GUILayout.Width(width), GUILayout.Height(height),
                              GUILayout.ExpandWidth(width == 0), GUILayout.ExpandHeight(height == 0));
        }

        /// <summary>
        /// Draws a Help Box with a custom icon;
        /// </summary>
        /// <param name="text"> Help Box message; </param>
        /// <param name="texture"> Help Box icon; </param>
        public static void DrawCustomHelpBox(string text, Texture texture, params GUILayoutOption[] options) {
            GUIContent messageContent = new GUIContent(text, texture);
            GUILayout.Label(messageContent, UIStyles.HelpBoxLabel, options);
        }

        /// <summary>
        /// Draws a Help Box with a custom icon;
        /// </summary>
        /// <param name="text"> Help Box message; </param>
        /// <param name="texture"> Help Box icon; </param>
        public static void DrawCustomHelpBox(string text, Texture texture) {
            GUIContent messageContent = new GUIContent(text, texture);
            GUILayout.Label(messageContent, UIStyles.HelpBoxLabel,
                            GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
        }

        /// <summary>
        /// Draws a Help Box with a custom icon;
        /// </summary>
        /// <param name="text"> Help Box message; </param>
        /// <param name="texture"> Help Box icon; </param>
        public static void DrawCustomHelpBox(string text, Texture texture, GUIStyle style) {
            GUIContent messageContent = new GUIContent(text, texture);
            GUILayout.Label(messageContent, style,
                            GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
        }

        /* WORK IN PROGRESS
        /// <summary>
        /// Draws a Help Box with a custom icon;
        /// </summary>
        /// <param name="text"> Help Box message; </param>
        /// <param name="texture"> Help Box icon; </param>
        public static void DrawSplitHelpBox(string text, Texture texture, GUIStyle style) {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                GUILayout.Label(texture, GUILayout.ExpandWidth(false),
                                GUILayout.ExpandHeight(true));
                GUILayout.Label(text, style, GUILayout.ExpandWidth(true),
                                GUILayout.ExpandHeight(false));
            }
        }*/

        /// <summary>
        /// Draw a texture on the current layout;
        /// </summary>
        /// <param name="texture"> Texture to draw; </param>
        /// <param name="width"> Width of the Box containing the texture; </param>
        /// <param name="height"> Height of the Box containing the texture; </param>
        public static void DrawTexture(Texture2D texture, float width, float height) {
            GUILayout.Box(texture, GUILayout.Width(width), GUILayout.Height(height));
        }

        /// <summary>
        /// Draw a texture on the current layout;
        /// </summary>
        /// <param name="texture"> Texture to draw; </param>
        public static void DrawTexture(Texture2D texture, params GUILayoutOption[] options) {
            GUILayout.Box(texture, options);
        }
    }

    public static class UndoUtils {

        public static void RecordScopeUndo(Object target, string undoText) {
            Undo.RecordObject(target, undoText);
            PrefabUtility.RecordPrefabInstancePropertyModifications(target);
        }

        public static void RecordFullScopeUndo(Object target, string undoText) {
            Undo.RegisterCompleteObjectUndo(target, undoText);
            PrefabUtility.RecordPrefabInstancePropertyModifications(target);
        }

        public static int StartUndoGroup(string groupName) {
            Undo.SetCurrentGroupName(groupName);
            return Undo.GetCurrentGroup();
        }
    }

    /// <summary>
    /// A collection of functions to draw custom bundles of UI Elements;
    /// </summary>
    public static class EditorUtils {

        public const string OBJECT_PICKER_UPDATED = "ObjectSelectorUpdated";
        public const string OBJECT_PICKER_CLOSED = "ObjectSelectorClosed";

        /// <summary>
        /// Request a menu dropdown displaying options and returning integers;
        /// </summary>
        /// <param name="rect"> Dropdown origin; </param>
        /// <param name="elements"> Strings to display; </param>
        /// <param name="func"> Callback to fetch results; </param>
        public static void RequestDropdownCallback(Rect rect, string[] elements,
                                                   GenericMenu.MenuFunction2 func) {
            GenericMenu menu = new();
            for (int i = 0; i < elements.Length; i++) {
                menu.AddItem(new GUIContent(elements[i]), false, func, i);
            } menu.DropDown(rect);
        }

        /// <summary>
        /// Request a menu dropdown displaying options and returning objects;
        /// </summary>
        /// <param name="rect"> Dropdown origin; </param>
        /// <param name="elements"> Strings to display; </param>
        /// <param name="contents"> Objects to return; </param>
        /// <param name="disabled"> Whether each element is enabled; </param>
        /// <param name="func"> Callback to fetch results; </param>
        public static void RequestDropdownCallback<T>(Rect rect, string[] elements, T[] contents,
                                                      bool[] disabled, GenericMenu.MenuFunction2 func) {
            GenericMenu menu = new();
            for (int i = 0; i < elements.Length; i++) {
                if (disabled[i]) menu.AddDisabledItem(new GUIContent(elements[i]));
                else menu.AddItem(new GUIContent(elements[i]), false, func, contents[i]);
            } menu.DropDown(rect);
        }

        /// <summary>
        /// Request a menu dropdown displaying options and returning objects;
        /// </summary>
        /// <param name="rect"> Dropdown origin; </param>
        /// <param name="elements"> Strings to display; </param>
        /// <param name="contents"> Objects to return; </param>
        /// <param name="func"> Callback to fetch results; </param>
        public static void RequestDropdownCallback<T>(Rect rect, string[] elements, T[] contents,
                                                      GenericMenu.MenuFunction2 func) {
            bool[] disabled = new bool[elements.Length];
            RequestDropdownCallback(rect, elements, contents, disabled, func);
        }

        /// <summary>
        /// Bring inspector focus to an asset;
        /// </summary>
        /// <param name="asset"> Asset to focus the inspector on; </param>
        /// <param name="ping"> Whether to ping the asset in the project window as well; </param>
        public static void InspectorFocusAsset<T>(T asset, bool ping = false) where T : ScriptableObject {
            Selection.activeObject = asset;
            EditorApplication.ExecuteMenuItem("Window/General/Inspector");
            if (ping) EditorGUIUtility.PingObject(asset);
        }

        /// <summary>
        /// Quick extension to show the Object picker for a given object type;
        /// <br></br> The result must be captured with CatchOPEvent;
        /// <br></br> The value must be checked on the Editor that calls this method;
        /// <br></br> This version is unreliable, as the OP may not always fire callbacks;
        /// Use ShowAdvancedObjectPicker() instead to pass in direct callbacks;
        /// </summary>
        /// <param name="obj"> Object whose type will be included in the Object Picker; </param>
        public static void ShowObjectPicker<T>(T obj, string filter = "") where T : Object {
            EditorGUIUtility.ShowObjectPicker<T>(obj, false, filter,
                                                 GUIUtility.GetControlID(FocusType.Passive) + 100);
        }

        /// <summary>
        /// Quick extension to catch Object picker output;
        /// </summary>
        /// <returns> Object captured in the picker event; </returns>
        public static T CatchOPEvent<T>() where T : Object {
            if (Event.current.commandName == OBJECT_PICKER_UPDATED) {
                Object obj = EditorGUIUtility.GetObjectPickerObject();
                if (obj is T) return obj as T;
            } return null;
        }

        /// Sample use case for above methods ///
        /// 
        /// private bool awaitOPCallback;
        /// 
        /// void SomeMethod() {
        ///     EditorUtils.ShowObjectPicker(null);
        ///     awaitOPCallback = true;
        /// }
        /// 
        /// void OnGUI() {
        ///     if (awaitOPCallback) {
        ///         TileData newTile = EditorUtils.CatchOPEvent<TileData>();
        ///         if (Event.current.commandName == EditorUtils.OBJECT_PICKER_CLOSED) {
        ///             if (newTile is not null) prefs.activePalette.Tiles.Add(newTile);
        ///             awaitOPCallback = false;
        ///         }
        ///     }
        /// }

        /// <summary>
        /// Quick extension to show the SearchView in context to retrieve an object;
        /// <br></br> Accepts a callback that fires when an object is chosen;
        /// </summary>
        public static void ShowAdvancedObjectPicker<T>(System.Action<Object, bool> callback,
                                                    string filter = "") {
            SearchService.ShowObjectPicker(callback, (obj) => { }, filter, "", typeof(T));
        }

        /// <summary>
        /// Return a rect within the bounds of a view rect to prevent window overflow;
        /// </summary>
        /// <param name="view"> Rect of the View window; </param>
        /// <param name="rect"> Rect to keep within bounds; </param>
        /// <returns> New rect within the bounds of the view rect; </returns>
        public static Rect PreventWindowOverflow(Rect view, Rect rect) {
            return new Rect(rect) {
                x = Mathf.Min(view.width - rect.width, Mathf.Max(0, rect.x)),
                y = Mathf.Min(view.height - rect.height, Mathf.Max(0, rect.y)),
            };
        }

        /// <summary>
        /// Opens and/or focuses the Project Window;
        /// </summary>
        public static void OpenProjectWindow() {
            System.Type projectBrowserType = typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
            EditorWindow.GetWindow(projectBrowserType);
        }

        /// <summary>
        /// Opens/focuses the Project Window and Pings Object;
        /// </summary>
        /// <param name="obj"> Object to ping in the Project Window; </param>
        public static void PingObject(Object obj) {
            OpenProjectWindow();
            EditorGUIUtility.PingObject(obj);
        }

        /// <summary>
        /// Turn window into a drop zone that accepts drag operations;
        /// <br></br> Verify whether mouse is contained within control to restrict the window;
        /// <br></br>
        /// </summary>
        public static object[] DropZone() {
            bool isAccepted = false;
            if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform) {
                if (Event.current.type == EventType.DragPerform) {
                    DragAndDrop.AcceptDrag();
                    isAccepted = true;
                } Event.current.Use();
            } return isAccepted ? DragAndDrop.objectReferences : null;
        }

        /// <summary>
        /// Returns the width of a text string in pixels;
        /// </summary>
        /// <param name="text"> Text string to measure; </param>
        /// <param name="font"> Font to get the width from; </param>
        /// <returns> Width of the text in question; </returns>
        public static float MeasureTextWidth(string text, Font font) {
            float width = 0;
            foreach (char letter in text) {
                CharacterInfo info;
                font.GetCharacterInfo(letter, out info);
                width += info.advance;
            } return width;
        }

        /// <summary> Common icon string; </summary>
        public const string ICON_INFO = "d_console.infoicon.sml",
                            ICON_WARNING = "Warning",
                            ICON_ERROR = "Error",
                            ICON_PLUS = "Toolbar Plus",
                            ICON_LESS = "Toolbar Minus",
                            ICON_PLUS_MORE = "d_Toolbar Plus More",
                            ICON_HELP = "_Help",
                            ICON_SEARCH = "Search Icon",
                            ICON_HGRIP = "grip_horizontalcontainer",
                            ICON_VGRIP = "grip_verticalcontainer",
                            ICON_CHECK_BLUE = "d_P4_CheckOutRemote",
                            ICON_CHECK_YELLOW = "d_P4_OutOfSync",
                            ICON_CHECK_RED = "d_P4_Offline",
                            ICON_SETTINGS = "_Popup";

        /// <summary>
        /// Fetch an icon texture from the database.
        /// <br></br> A list of Unity's Built-In Icon Names can be found <a href="https://github.com/Zxynine/UnityEditorIcons/tree/main">here</a>;
        /// </summary>
        /// <param name="iconName"> A list of Unity's Built-In Icon Names can be found <a href="https://github.com/Zxynine/UnityEditorIcons/tree/main">here</a>; </param>
        /// <returns> Icon texture; </returns>
        public static Texture2D FetchIcon(string iconName) {
            return (Texture2D) EditorGUIUtility.IconContent(iconName).image;
        }

        /// <summary>
        /// Assign an icon to a variable if it's unnassigned;
        /// </summary>
        /// <param name="icon"> Variable to which the icon will be assigned; </param>
        /// <param name="identifier"> Identifying string for the icon to load; </param>
        public static void LoadIcon(ref Texture2D icon, string identifier) {
            icon = icon != null ? icon : FetchIcon(identifier);
        }

        /// <summary>
        /// Takes an integer (long) number of bytes and returns a file size string with reasonable units;
        /// <br></br> PS: The code for this method is intentionally a dumpster fire. I mean, look at it. It's just pretty >.>;
        /// </summary>
        /// <param name="bytes"> File length to parse; </param>
        /// <returns> File size string with reasonable units in the format "{size} {units}"; </returns>
        public static string ProcessFileSize(long bytes) {
            return bytes / Mathf.Pow(1024, 3) > 1 ? (int) (bytes / Mathf.Pow(1024f, 3) * 100f) / 100f + " GB"
                   : bytes / Mathf.Pow(1024, 2) > 1 ? (int) (bytes / Mathf.Pow(1024f, 2) * 100f) / 100f + " MB"
                   : bytes / Mathf.Pow(1024, 1) > 1 ? (int) (bytes / Mathf.Pow(1024f, 1) * 100f) / 100f + " KB"
                   : bytes + " bytes";
        }

        /// <summary>
        /// Pulls up the inspector properties associated with an asset on a separate window;
        /// </summary>
        /// <param name="path"> Path to the asset to pull up the inspector for; </param>
        public static void OpenAssetProperties(string path) {
            EditorUtility.OpenPropertyEditor(AssetDatabase.LoadAssetAtPath(path, typeof(Object)));
        }

        /// <summary>
        /// Pulls up the inspector properties associated with an asset on a separate window;
        /// </summary>
        /// <param name="targetObject"> Asset to pull up the inspector for; </param>
        public static void OpenAssetProperties(Object targetObject) {
            EditorUtility.OpenPropertyEditor(targetObject);
        }
    }

    public static class AssemblyUtils {

        /// <summary>
        /// Get Type from Assembly using reflection;
        /// <br></br> Use with caution and avoid iterative calls;
        /// <br></br> Adapted from an official 
        /// <a href="https://blog.unity.com/engine-platform/isometric-2d-environments-with-tilemap">Unity Tutorial</a>;
        /// </summary>
        /// <param name="TypeName"> Name of the type to search; </param>
        /// <returns> System.Type corresponding to the given name, or null if none is found; </returns>
        public static System.Type GetType(string TypeName) {
            System.Type type = System.Type.GetType(TypeName);
            if (type != null) return type;

            if (TypeName.Contains(".")) {
                string assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));
                Assembly assembly = Assembly.Load(assemblyName);
                if (assembly == null) return null;
                type = assembly.GetType(TypeName);
                if (type != null) return type;
            }

            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            AssemblyName[] referencedAssemblies = currentAssembly.GetReferencedAssemblies();
            foreach (var assemblyName in referencedAssemblies) {
                Assembly assembly = Assembly.Load(assemblyName);
                if (assembly != null) {
                    type = assembly.GetType(TypeName);
                    if (type != null) return type;
                }
            } return null;
        }
    }

    /// <summary>
    /// A collection of Custom Editor UI Styles to make things pretty;
    /// </summary>
    public class UIStyles {

        public static GUIStyle TemplateStyle {
            get {
                GUIStyle style = new GUIStyle();
                return style;
            }
        }

        #region | Views and Bars |

        public static GUIStyle WindowBox {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.window) {
                    padding = new RectOffset(5, 5, 5, 5),
                    stretchWidth = false,
                    stretchHeight = false,
                }; return style;
            }
        }

        public static GUIStyle ToolbarText {
            get {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = GUI.skin.label.normal.textColor;
                style.alignment = TextAnchor.MiddleRight;
                style.padding = new RectOffset(0, 10, 0, 1);
                style.contentOffset = new Vector2(15, 0);
                return style;
            }
        }

        public static GUIStyle ToolbarPaddedPopUp {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.toolbarPopup);
                style.contentOffset = new Vector2(2, 0);
                return style;
            }
        }

        public static GUIStyle PaddedToolbar {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.toolbar);
                style.padding = new RectOffset(10, 10, 0, 0);
                return style;
            }
        }

        public static GUIStyle PaddedScrollView {
            get {
                GUIStyle style = new GUIStyle();
                style.padding = new RectOffset(7, 7, 3, 3);
                return style;
            }
        }
        public static GUIStyle MorePaddingScrollView {
            get {
                GUIStyle style = new GUIStyle();
                style.padding = new RectOffset(15, 15, 15, 15);
                return style;
            }
        }

        #endregion

        #region | Buttons & Toggles |

        public static GUIStyle HButtonSelected {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.miniTextField);
                style.normal.textColor = new Color(0.725f, 0.83f, 0.84f);
                style.hover.textColor = new Color(0.725f, 0.83f, 0.84f);
                style.margin = new RectOffset(EditorGUI.indentLevel * 15 + 5, 0, -4, -4);
                style.padding = new RectOffset(7, 0, 1, 1);
                style.alignment = TextAnchor.MiddleLeft;
                return style;
            }
        }

        public static GUIStyle HButton {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.miniButton);
                style.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
                style.hover.textColor = Color.white;
                style.margin = new RectOffset(EditorGUI.indentLevel * 15 + 5, 0, -3, -3);
                style.alignment = TextAnchor.MiddleLeft;
                return style;
            }
        }

        public static GUIStyle HFButtonSelected {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.miniTextField);
                style.normal.textColor = new Color(0.725f, 0.83f, 0.84f);
                style.hover.textColor = new Color(0.725f, 0.83f, 0.84f);
                style.margin = new RectOffset(EditorGUI.indentLevel * 15 + 5, 0, 0, 0);
                style.padding = new RectOffset(7, 0, 0, 0);
                style.alignment = TextAnchor.MiddleLeft;
                style.fixedHeight = 18;
                return style;
            }
        }

        public static GUIStyle HFButton {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.miniButton);
                style.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
                style.hover.textColor = Color.white;
                style.margin = new RectOffset(EditorGUI.indentLevel * 15 + 5, 0, 0, 0);
                style.alignment = TextAnchor.MiddleLeft;
                return style;
            }
        }

        public static GUIStyle TextureButton {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.button);
                style.alignment = TextAnchor.MiddleCenter;
                return style;
            }
        }

        public static GUIStyle SquashedButton {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.miniButton);
                style.richText = true;
                return style;
            }
        }

        public static GUIStyle LowerToggle {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.toggle);
                style.fixedHeight = GUI.skin.label.fixedHeight;
                style.alignment = TextAnchor.LowerCenter;
                return style;
            }
        }

        public static GUIStyle SelectedToolbar {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.box);
                style.margin = new RectOffset(0, 0, -10, 0);
                style.alignment = TextAnchor.UpperCenter;
                style.normal.textColor = GUI.skin.label.normal.textColor;
                return style;
            }
        }

        public static GUIStyle ArrangedBoxUnselected {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.box) {
                    normal = { textColor = EditorStyles.label.normal.textColor },
                    margin = new RectOffset(2, 2, 0, 2),
                    fixedHeight = 20,
                }; style.fontSize--;
                return style;
            }
        }

        public static GUIStyle ArrangedButtonSelected {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.numberField) {
                    alignment = TextAnchor.MiddleCenter,
                    margin = new RectOffset(0, 0, 4, 0),
                    normal = { textColor = UIColors.Blue }
                }; return style;
            }
        }

        public static GUIStyle ArrangedLabel {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.label) {
                    contentOffset = new Vector2(0, 1)
                }; return style;
            }
        }


        #endregion

        #region | Labels |

        public static GUIStyle CenteredLabel {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleCenter;
                return style;
            }
        }

        public static GUIStyle CenteredLabelBold {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                style.alignment = TextAnchor.MiddleCenter;
                return style;
            }
        }

        public static GUIStyle RightAlignedLabel {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleRight;
                return style;
            }
        }

        public static GUIStyle LeftAlignedLabel {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleLeft;
                return style;
            }
        }

        public static GUIStyle ItalicLabel {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.fontStyle = FontStyle.Italic;
                return style;
            }
        }

        public static GUIStyle HelpBoxLabel {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.helpBox);
                style.richText = true;
                style.padding = new RectOffset(4, 0, 1, 1);
                return style;
            }
        }

        public static GUIStyle TextBoxLabel {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.helpBox);
                style.richText = true;
                style.margin = new RectOffset(0, 0, 0, 0);
                style.alignment = TextAnchor.UpperCenter;
                return style;
            }
        }

        public static GUIStyle TextHelpLabel {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.font = EditorStyles.helpBox.font;
                style.fontSize = EditorStyles.helpBox.fontSize;
                style.normal = EditorStyles.helpBox.normal;
                style.richText = true;
                style.margin = new RectOffset(0, 0, 0, 0);
                style.alignment = TextAnchor.UpperLeft;
                return style;
            }
        }

        #endregion
    }

#endif
}