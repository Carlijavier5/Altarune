#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace AllIn1ShaderNodes
{
    public class AllIn1ShaderNodesWindow : EditorWindow
    {
        private const string Version = "1.13";

        [MenuItem("Tools/AllIn1/ShaderNodesWindow")]
        public static void ShowAllIn1ShaderNodesWindowWindow() => GetWindow<AllIn1ShaderNodesWindow>("All In 1 Shader Nodes Window");

        public static readonly string CUSTOM_EDITOR_HEADER = "AllIn1ShaderNodesCustomEditorImage";
        private static string basePath = "Assets/Plugins/AllIn1ShaderNodes";
        public static readonly string normalMapSavesPath = "/DefaultTextures";
        public static readonly string gradientSavesPath = "/DefaultTextures";
        public static readonly string noiseSavesPath = "/DefaultTextures";
        public static readonly string atlasSavesPath = "/DefaultTextures";

        private Vector2 scrollPosition = Vector2.zero;
        private DefaultAsset materialTargetFolder = null;
        private GUIStyle style, bigLabel = new GUIStyle();
        private const int BigFontSize = 16;
        private Texture2D imageInspector;
        private const int BUTTON_WIDTH = 600;

        private enum TextureSizes
        {
            _2 = 2,
            _4 = 4,
            _8 = 8,
            _16 = 16,
            _32 = 32,
            _64 = 64,
            _128 = 128,
            _256 = 256,
            _512 = 512,
            _1024 = 1024,
            _2048 = 2048
        }

        private TextureSizes gradientSizes = TextureSizes._128;
        [SerializeField] private Gradient gradient = new Gradient();
        private FilterMode gradientFiltering = FilterMode.Bilinear;

        private TextureSizes atlasSizesX = TextureSizes._512;
        private TextureSizes atlasSizesY = TextureSizes._512;
        private FilterMode atlasFiltering = FilterMode.Bilinear;

        private Texture2D targetNormalImage;
        private float normalStrength = 5f;
        private int normalSmoothing = 1;
        private int isComputingNormals = 0;
        private int currTab = 0;

        private Texture2D editorTex, editorTexInput, cleanEditorTex;

        private void OnGUI()
        {
            style = new GUIStyle(EditorStyles.helpBox);
            style.margin = new RectOffset(0, 0, 0, 0);
            bigLabel = new GUIStyle(EditorStyles.boldLabel);
            bigLabel.fontSize = BigFontSize;

            using(var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height)))
            {
                scrollPosition = scrollView.scrollPosition;

                if(imageInspector == null) imageInspector = GetInspectorImage();
                if(imageInspector)
                {
                    Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(50));
                    GUI.DrawTexture(rect, imageInspector, ScaleMode.ScaleToFit, true);
                }

                DrawLine(Color.grey, 1, 3);
                currTab = GUILayout.Toolbar(currTab, new string[] {"Texture Editor", "Texture Creators"});
                DrawLine(Color.grey, 1, 3);

                if(currTab == 0)
                {
                    TextureEditor();
                }
                else if(currTab == 1)
                {
                    NormalMapCreator();
                    DrawLine(Color.grey, 1, 3);
                    GradientCreator();
                    DrawLine(Color.grey, 1, 3);
                    TextureAtlasPacker();
                    DrawLine(Color.grey, 1, 3);
                    NoiseCreator();
                }

                GUILayout.Space(10);
                DrawLine(Color.grey, 1, 3);
                RenderPipelineSection();

                DrawLine(Color.grey, 1, 3);
                GUILayout.Label("Current asset version is " + Version, EditorStyles.boldLabel);
            }
        }

        private void RenderPipelineSection()
        {
#if UNITY_2019_3_OR_NEWER
            GUILayout.Label("Render Pipeline Detection", bigLabel);
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Current Pipeline: ", GUILayout.MaxWidth(120));
                string pipelineName = "Unknown";

                if(RenderPipelineChecker.IsHDRP)
                {
                    pipelineName = "HDRP (High Definition)";
                }
                else if(RenderPipelineChecker.IsURP)
                {
                    pipelineName = "URP (Universal)";
                }
                else if(RenderPipelineChecker.IsStandardRP)
                {
                    pipelineName = "Built-in (Standard)";
                }

                GUILayout.Label(pipelineName, EditorStyles.boldLabel);
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            if(GUILayout.Button("Refresh Pipeline Detection", GUILayout.MaxWidth(BUTTON_WIDTH)))
            {
                RenderPipelineChecker.CheckRenderPipeline();
                string currentPipeline = RenderPipelineChecker.IsHDRP ? "HDRP" : RenderPipelineChecker.IsURP ? "URP" : "Built-in";
                Debug.Log($"Pipeline refreshed, current pipeline is: {currentPipeline}");
            }
#else
            GUILayout.Label("Render Pipeline Detection requires Unity 2019.3 or newer", EditorStyles.boldLabel);
#endif
        }

        public static Texture2D GetInspectorImage()
        {
            string[] guids = AssetDatabase.FindAssets($"{CUSTOM_EDITOR_HEADER} t:texture");
            if(guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
            return null;
        }

        private void HandleSaveFolderEditorPref(string keyName, string defaultPath, string logsFeatureName)
        {
            if (!PlayerPrefs.HasKey(keyName)) PlayerPrefs.SetString(keyName, defaultPath);
            materialTargetFolder = (DefaultAsset)AssetDatabase.LoadAssetAtPath(PlayerPrefs.GetString(keyName), typeof(DefaultAsset));
            if (materialTargetFolder == null)
            {
                PlayerPrefs.SetString(keyName, defaultPath);
                materialTargetFolder = (DefaultAsset)AssetDatabase.LoadAssetAtPath(PlayerPrefs.GetString(keyName), typeof(DefaultAsset));
                if (materialTargetFolder == null)
                {
                    materialTargetFolder = (DefaultAsset)AssetDatabase.LoadAssetAtPath("Assets/", typeof(DefaultAsset));
                    if(materialTargetFolder == null)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(600));
                        EditorGUILayout.HelpBox("Folder is invalid, please select a valid one", MessageType.Error, true);
                        EditorGUILayout.EndHorizontal();
                    }
                    else PlayerPrefs.SetString("Assets/", defaultPath);
                }
            }
            materialTargetFolder = (DefaultAsset)EditorGUILayout.ObjectField("New " + logsFeatureName + " Folder",
                materialTargetFolder, typeof(DefaultAsset), false, GUILayout.MaxWidth(500));

            if (materialTargetFolder != null && IsAssetAFolder(materialTargetFolder))
            {
                string path = AssetDatabase.GetAssetPath(materialTargetFolder);
                PlayerPrefs.SetString(keyName, path);
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(600));
                EditorGUILayout.HelpBox("Valid folder! " + logsFeatureName + " save path: " + path, MessageType.Info);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(600));
                EditorGUILayout.HelpBox("Select the new " + logsFeatureName + " Folder", MessageType.Warning, true);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void NormalMapCreator()
        {
            GUILayout.Label("Normal/Distortion Map Creator", bigLabel);

            GUILayout.Space(20);
            GUILayout.Label("Select the folder where new Normal Maps will be saved when the Create Normal Map button of the asset component is pressed", EditorStyles.boldLabel);
            GUILayout.Label("*These Normal Maps can then be used with the Screen Distortion effect", EditorStyles.boldLabel);
            HandleSaveFolderEditorPref("All1ShaderNodesNormals", basePath + normalMapSavesPath, "Normal Maps");

            GUILayout.Space(20);
            GUILayout.Label("Assign a texture you want to create a normal map from. Choose the normal map settings and press the 'Create And Save Normal Map' button", EditorStyles.boldLabel);
            targetNormalImage = (Texture2D) EditorGUILayout.ObjectField("Target Image", targetNormalImage, typeof(Texture2D), false, GUILayout.MaxWidth(225));

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Normal Strength:", GUILayout.MaxWidth(150));
                normalStrength = EditorGUILayout.Slider(normalStrength, 1f, 20f, GUILayout.MaxWidth(400));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Normal Smoothing:", GUILayout.MaxWidth(150));
                normalSmoothing = EditorGUILayout.IntSlider(normalSmoothing, 0, 3, GUILayout.MaxWidth(400));
            }
            EditorGUILayout.EndHorizontal();

            if(isComputingNormals == 0)
            {
                if(targetNormalImage != null)
                {
                    if(GUILayout.Button("Create And Save Normal Map", GUILayout.MaxWidth(BUTTON_WIDTH)))
                    {
                        isComputingNormals = 1;
                        return;
                    }
                }
                else
                {
                    GUILayout.Label("Add a Target Image to use this feature", EditorStyles.boldLabel);
                }
            }
            else
            {
                GUILayout.Label("Normal Map is currently being created, be patient", EditorStyles.boldLabel, GUILayout.Height(40));
                Repaint();
                isComputingNormals++;
                if(isComputingNormals > 5)
                {
                    SetTextureReadWrite(AssetDatabase.GetAssetPath(targetNormalImage), true);

                    Texture2D normalToSave = CreateNormalMap(targetNormalImage, normalStrength, normalSmoothing);

                    string prefSavedPath = PlayerPrefs.GetString("All1ShaderNodesNormals") + "/";
                    string path = prefSavedPath + "NormalMap.png";
                    if(System.IO.File.Exists(path)) path = GetNewValidPath(path);
                    string texName = path.Replace(prefSavedPath, "");

                    path = EditorUtility.SaveFilePanel("Save texture as PNG", prefSavedPath, texName, "png");
                    if(path.Length != 0)
                    {
                        byte[] pngData = normalToSave.EncodeToPNG();
                        if(pngData != null) File.WriteAllBytes(path, pngData);
                        AssetDatabase.Refresh();

                        if(path.IndexOf("Assets/") >= 0)
                        {
                            string subPath = path.Substring(path.IndexOf("Assets/"));
                            TextureImporter importer = AssetImporter.GetAtPath(subPath) as TextureImporter;
                            if(importer != null)
                            {
                                SceneViewNotificationAndLog("Normal Map saved inside the project: " + subPath);
                                importer.filterMode = FilterMode.Bilinear;
                                importer.textureType = TextureImporterType.NormalMap;
                                importer.wrapMode = TextureWrapMode.Repeat;
                                importer.SaveAndReimport();
                                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(subPath, typeof(Texture)));
                            }
                        }
                        else SceneViewNotificationAndLog("Normal Map saved outside the project: " + path);
                    }

                    isComputingNormals = 0;
                }
            }

            GUILayout.Label("*This process will freeze the editor for some seconds, larger images will take longer", EditorStyles.boldLabel);
        }

        private static void SetTextureReadWrite(string assetPath, bool enable)
        {
            TextureImporter tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if(tImporter != null)
            {
                tImporter.isReadable = enable;
                tImporter.SaveAndReimport();
            }
        }

        private void GradientCreator()
        {
            GUILayout.Label("Color Gradient Creator", bigLabel);
            GUILayout.Space(20);
            GUILayout.Label("This feature can be used to create textures for the Color Ramp Effect", EditorStyles.boldLabel);

            EditorGUILayout.GradientField("Color Gradient: ", gradient, GUILayout.Height(25), GUILayout.MaxWidth(BUTTON_WIDTH));

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Texture Size:", GUILayout.MaxWidth(145));
                gradientSizes = (TextureSizes) EditorGUILayout.EnumPopup(gradientSizes, GUILayout.MaxWidth(200));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("New Textures Filtering: ", GUILayout.MaxWidth(145));
                gradientFiltering = (FilterMode) EditorGUILayout.EnumPopup(gradientFiltering, GUILayout.MaxWidth(200));
            }
            EditorGUILayout.EndHorizontal();

            int textureSize = (int) gradientSizes;
            Texture2D gradTex = new Texture2D(textureSize, 1, TextureFormat.RGBA32, false);
            for(int i = 0; i < textureSize; i++) gradTex.SetPixel(i, 0, gradient.Evaluate((float) i / (float) textureSize));
            gradTex.Apply();

            GUILayout.Space(20);
            GUILayout.Label("Select the folder where new Color Gradient Textures will be saved", EditorStyles.boldLabel);
            HandleSaveFolderEditorPref("All1ShaderNodesGradients", basePath + gradientSavesPath, "Gradients");

            string prefSavedPath = PlayerPrefs.GetString("All1ShaderNodesGradients") + "/";
            if(Directory.Exists(prefSavedPath))
            {
                if(GUILayout.Button("Save Color Gradient Texture", GUILayout.MaxWidth(BUTTON_WIDTH)))
                {
                    string path = prefSavedPath + "ColorGradient.png";
                    if(System.IO.File.Exists(path)) path = GetNewValidPath(path);
                    string texName = path.Replace(prefSavedPath, "");

                    path = EditorUtility.SaveFilePanel("Save texture as PNG", prefSavedPath, texName, "png");
                    if(path.Length != 0)
                    {
                        byte[] pngData = gradTex.EncodeToPNG();
                        if(pngData != null) File.WriteAllBytes(path, pngData);
                        AssetDatabase.Refresh();

                        if(path.IndexOf("Assets/") >= 0)
                        {
                            string subPath = path.Substring(path.IndexOf("Assets/"));
                            TextureImporter importer = AssetImporter.GetAtPath(subPath) as TextureImporter;
                            if(importer != null)
                            {
                                SceneViewNotificationAndLog("Gradient saved inside the project: " + subPath);
                                importer.filterMode = gradientFiltering;
                                importer.SaveAndReimport();
                                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(subPath, typeof(Texture)));
                            }
                        }
                        else SceneViewNotificationAndLog("Gradient saved outside the project: " + path);
                    }
                }
            }
        }

        public Texture2D[] Atlas = Array.Empty<Texture2D>();
        private int atlasXCount = 1;
        private int atlasYCount = 1;
        private bool squareAtlas = true;
        private void TextureAtlasPacker()
        {
            GUILayout.Label("Texture Atlas / Spritesheet Packer", bigLabel);
            GUILayout.Space(20);
            GUILayout.Label("Add Textures to the Atlas array", EditorStyles.boldLabel);

            ScriptableObject target = this;
            SerializedObject so = new SerializedObject(target);
            SerializedProperty stringsProperty = so.FindProperty("Atlas");
            EditorGUILayout.PropertyField(stringsProperty, true, GUILayout.MaxWidth(200));
            so.ApplyModifiedProperties();

            squareAtlas = EditorGUILayout.Toggle("Square Atlas?", squareAtlas, GUILayout.MaxWidth(200));
            EditorGUILayout.BeginHorizontal();
            {
                if(squareAtlas)
                {
                    atlasXCount = EditorGUILayout.IntSlider("Column and Row Count", atlasXCount, 1, 8, GUILayout.MaxWidth(302));
                    atlasYCount = atlasXCount;
                }
                else
                {
                    atlasXCount = EditorGUILayout.IntSlider("Column Count", atlasXCount, 1, 8, GUILayout.MaxWidth(302));
                    GUILayout.Space(10);
                    atlasYCount = EditorGUILayout.IntSlider("Row Count", atlasYCount, 1, 8, GUILayout.MaxWidth(302));
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                if(squareAtlas)
                {
                    GUILayout.Label("Atlas Size:", GUILayout.MaxWidth(100));
                    atlasSizesX = (TextureSizes) EditorGUILayout.EnumPopup(atlasSizesX, GUILayout.MaxWidth(200));
                    atlasSizesY = atlasSizesX;
                }
                else
                {
                    GUILayout.Label("Atlas Size X:", GUILayout.MaxWidth(100));
                    atlasSizesX = (TextureSizes) EditorGUILayout.EnumPopup(atlasSizesX, GUILayout.MaxWidth(200));
                    GUILayout.Space(10);
                    GUILayout.Label("Atlas Size Y:", GUILayout.MaxWidth(100));
                    atlasSizesY = (TextureSizes) EditorGUILayout.EnumPopup(atlasSizesY, GUILayout.MaxWidth(200));
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Atlas Filtering: ", GUILayout.MaxWidth(100));
                atlasFiltering = (FilterMode) EditorGUILayout.EnumPopup(atlasFiltering, GUILayout.MaxWidth(200));
            }
            EditorGUILayout.EndHorizontal();

            int atlasElements = atlasXCount * atlasYCount;
            int atlasWidth = (int) atlasSizesX;
            int atlasHeight = (int) atlasSizesY;
            GUILayout.Label("Output will be a " + atlasXCount + " X " + atlasYCount + " atlas, " + atlasElements + " elements in total. In a " +
                            atlasWidth + "pixels X " + atlasHeight + "pixels texture", EditorStyles.boldLabel);

            int usedAtlasSlots = 0;
            for(int i = 0; i < Atlas.Length; i++) if(Atlas[i] != null) usedAtlasSlots++;
            if(usedAtlasSlots > atlasElements) GUILayout.Label("*Please reduce the Atlas texture slots by " + Mathf.Abs(atlasElements - Atlas.Length) + " (extra textures will be ignored)", EditorStyles.boldLabel);
            if(atlasElements > usedAtlasSlots) GUILayout.Label("*" + (atlasElements - usedAtlasSlots) + " atlas slots unused or null (it will be filled with black)", EditorStyles.boldLabel);

            GUILayout.Space(20);
            GUILayout.Label("Select the folder where new Atlases will be saved", EditorStyles.boldLabel);
            HandleSaveFolderEditorPref("All1ShaderNodesAtlas", basePath + atlasSavesPath, "Atlas");

            string prefSavedPath = PlayerPrefs.GetString("All1ShaderNodesAtlas") + "/";
            if(Directory.Exists(prefSavedPath))
            {
                if(GUILayout.Button("Create And Save Atlas Texture", GUILayout.MaxWidth(BUTTON_WIDTH)))
                {
                    string path = prefSavedPath + "Atlas.png";
                    if(System.IO.File.Exists(path)) path = GetNewValidPath(path);
                    string texName = path.Replace(prefSavedPath, "");

                    path = EditorUtility.SaveFilePanel("Save texture as PNG", prefSavedPath, texName, "png");
                    if(path.Length != 0)
                    {
                        Texture2D[] AtlasCopy = (Texture2D[]) Atlas.Clone();
                        int textureXTargetWidth = atlasWidth / atlasXCount;
                        int textureYTargetHeight = atlasHeight / atlasYCount;
                        Texture2D newAtlas = new Texture2D(atlasWidth, atlasHeight);
                        for(int i = 0; i < atlasYCount; i++)
                        {
                            for(int j = 0; j < atlasXCount; j++)
                            {
                                int currIndex = (i * atlasXCount) + j;
                                bool hasImageForThisIndex = currIndex < AtlasCopy.Length && AtlasCopy[currIndex] != null;
                                if(hasImageForThisIndex)
                                {
                                    SetTextureReadWrite(AssetDatabase.GetAssetPath(AtlasCopy[currIndex]), true);
                                    Texture2D copyTexture = new Texture2D(AtlasCopy[currIndex].width, AtlasCopy[currIndex].height);
                                    copyTexture.SetPixels(AtlasCopy[currIndex].GetPixels());
                                    copyTexture.Apply();
                                    AtlasCopy[currIndex] = copyTexture;
                                    AtlasCopy[currIndex] = ScaleTexture(AtlasCopy[currIndex], textureXTargetWidth, textureYTargetHeight);
                                    AtlasCopy[currIndex].Apply();
                                }

                                for(int y = 0; y < textureYTargetHeight; y++)
                                {
                                    for(int x = 0; x < textureXTargetWidth; x++)
                                    {
                                        if(hasImageForThisIndex) newAtlas.SetPixel((j * textureXTargetWidth) + x, (i * textureYTargetHeight) + y, AtlasCopy[currIndex].GetPixel(x, y));
                                        else newAtlas.SetPixel((j * textureXTargetWidth) + x, (i * textureYTargetHeight) + y, new Color(0, 0, 0, 1));
                                    }
                                }
                            }
                        }
                        newAtlas.Apply();

                        byte[] pngData = newAtlas.EncodeToPNG();
                        if(pngData != null) File.WriteAllBytes(path, pngData);
                        AssetDatabase.Refresh();

                        if(path.IndexOf("Assets/") >= 0)
                        {
                            string subPath = path.Substring(path.IndexOf("Assets/"));
                            TextureImporter importer = AssetImporter.GetAtPath(subPath) as TextureImporter;
                            if(importer != null)
                            {
                                SceneViewNotificationAndLog("Atlas saved inside the project: " + subPath);
                                importer.filterMode = atlasFiltering;
                                importer.SaveAndReimport();
                                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(subPath, typeof(Texture)));
                            }
                        }
                        else SceneViewNotificationAndLog("Atlas saved outside the project: " + path);
                    }
                }
            }
        }

        private Texture2D noisePreview = null;
        RenderTexture noiseRenderTarget = null;
        Material noiseMaterial;
        private float noiseScaleX = 10f, noiseScaleY = 10f, noiseContrast = 1f, noiseBrightness = 0f;
        private float noiseFractalAmount = 1f, noiseJitter = 1f;
        private int noiseSeed = 0;
        private bool noiseSquareScale = false, noiseInverted = false, isFractalNoise;
        private void CheckCreationNoiseTextures()
        {
            if(noisePreview == null) noisePreview = new Texture2D(256, 256);
            if(noiseRenderTarget == null) noiseRenderTarget = new RenderTexture(noisePreview.width, noisePreview.height, 0, RenderTextureFormat.ARGB32);
        }
        private void NoiseSetMaterial()
        {
            if(noiseType == NoiseTypes.Fractal || noiseType == NoiseTypes.Perlin || noiseType == NoiseTypes.Billow)
            {
                isFractalNoise = true;
                noiseMaterial = new Material(FindShader("AllIn1ShaderNodesFractalNoise"));
                noiseScaleX = 4f;
                noiseScaleY = 4f;
            }
            else
            {
                isFractalNoise = false;
                noiseMaterial = new Material(FindShader("AllIn1ShaderNodesWorleyNoise"));
                noiseScaleX = 10f;
                noiseScaleY = 10f;
            }

            switch(noiseType)
            {
                case NoiseTypes.Fractal:
                    noiseFractalAmount = 8f;
                    noiseMaterial.SetFloat("_Fractal", 1);
                    break;
                case NoiseTypes.Perlin:
                    noiseFractalAmount = 1f;
                    noiseMaterial.SetFloat("_Fractal", 1);
                    break;
                case NoiseTypes.Billow:
                    noiseFractalAmount = 4f;
                    noiseMaterial.SetFloat("_Fractal", 0);
                    break;
                case NoiseTypes.Voronoi:
                    noiseMaterial.SetFloat("_NoiseType", 0f);
                    break;
                case NoiseTypes.Water:
                    noiseMaterial.SetFloat("_NoiseType", 3f);
                    break;
                case NoiseTypes.Cellular:
                    noiseMaterial.SetFloat("_NoiseType", 4f);
                    break;
                case NoiseTypes.Cells1:
                    noiseMaterial.SetFloat("_NoiseType", 1f);
                    break;
                case NoiseTypes.Cells2:
                    noiseMaterial.SetFloat("_NoiseType", 2f);
                    break;
            }
        }

        private TextureSizes noiseSize = TextureSizes._512;
        private FilterMode noiseFiltering = FilterMode.Bilinear;

        private enum NoiseTypes
        {
            Fractal,
            Perlin,
            Billow,
            Voronoi,
            Water,
            Cellular,
            Cells1,
            Cells2
        }
        private NoiseTypes noiseType = NoiseTypes.Fractal;

        private void NoiseCreator()
        {
            GUILayout.Label("Tileable Noise Creator", bigLabel);
            GUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical(GUILayout.MaxWidth(550));
                {
                    if(noisePreview == null) GUILayout.Label("*Change a property to start editing a Noise texture", EditorStyles.boldLabel);

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Noise Type:", GUILayout.MaxWidth(145));
                        noiseType = (NoiseTypes) EditorGUILayout.EnumPopup(noiseType, GUILayout.MaxWidth(200));
                    }
                    EditorGUILayout.EndHorizontal();
                    if(EditorGUI.EndChangeCheck())
                    {
                        NoiseSetMaterial();
                        CheckCreationNoiseTextures();
                        UpdateNoiseMatAndRender();
                    }

                    EditorGUI.BeginChangeCheck();
                    if(isFractalNoise)
                    {
                        TextureEditorFloatParameter("Scale X", ref noiseScaleX, 0.1f, 50f, 4f);
                        if(!noiseSquareScale) TextureEditorFloatParameter("Scale Y", ref noiseScaleY, 0.1f, 50f, 4f);
                    }
                    else
                    {
                        TextureEditorFloatParameter("Scale X", ref noiseScaleX, 0.1f, 50f, 10f);
                        if(!noiseSquareScale) TextureEditorFloatParameter("Scale Y", ref noiseScaleY, 0.1f, 50f, 10f);
                    }
                    noiseSquareScale = EditorGUILayout.Toggle("Square Scale?", noiseSquareScale, GUILayout.MaxWidth(200));
                    if(noiseSquareScale) noiseScaleY = noiseScaleX;
                    if(noiseType == NoiseTypes.Fractal) TextureEditorFloatParameter("Fractal Amount", ref noiseFractalAmount, 1f, 10f, 8f);
                    else if(noiseType == NoiseTypes.Perlin) TextureEditorFloatParameter("Fractal Amount", ref noiseFractalAmount, 1f, 10f, 1f);
                    else if(noiseType == NoiseTypes.Billow) TextureEditorFloatParameter("Fractal Amount", ref noiseFractalAmount, 1f, 10f, 4f);
                    else TextureEditorFloatParameter("Jitter", ref noiseJitter, 0.0f, 2f, 1f);
                    TextureEditorFloatParameter("Contrast", ref noiseContrast, 0.1f, 10f, 1f);
                    TextureEditorFloatParameter("Brightness", ref noiseBrightness, -1f, 1f, 0f);
                    TextureEditorIntParameter("Random Seed", ref noiseSeed, 0, 100, 0);
                    noiseInverted = EditorGUILayout.Toggle("Inverted?", noiseInverted);

                    if(EditorGUI.EndChangeCheck())
                    {
                        if(noiseMaterial == null) NoiseSetMaterial();
                        CheckCreationNoiseTextures();

                        UpdateNoiseMatAndRender();
                    }

                    GUILayout.Space(20);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Noise Size:", GUILayout.MaxWidth(145));
                        noiseSize = (TextureSizes) EditorGUILayout.EnumPopup(noiseSize, GUILayout.MaxWidth(200));
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("New Noise Filtering: ", GUILayout.MaxWidth(145));
                        noiseFiltering = (FilterMode) EditorGUILayout.EnumPopup(noiseFiltering, GUILayout.MaxWidth(200));
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                if(noisePreview != null) GUILayout.Label(noisePreview, GUILayout.MaxWidth(450), GUILayout.MaxHeight(450));
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);
            GUILayout.Label("Select the folder where new Noise Textures will be saved", EditorStyles.boldLabel);
            HandleSaveFolderEditorPref("All1ShaderNodesNoise", basePath + noiseSavesPath, "Noises");

            string prefSavedPath = PlayerPrefs.GetString("All1ShaderNodesNoise") + "/";
            if(Directory.Exists(prefSavedPath) && noisePreview != null)
            {
                if(GUILayout.Button("Save Noise Texture", GUILayout.MaxWidth(BUTTON_WIDTH)))
                {
                    string path = prefSavedPath + "Noise.png";
                    if(System.IO.File.Exists(path)) path = GetNewValidPath(path);
                    string texName = path.Replace(prefSavedPath, "");

                    path = EditorUtility.SaveFilePanel("Save texture as PNG", prefSavedPath, texName, "png");
                    if(path.Length != 0)
                    {
                        int texSize = (int)noiseSize;
                        Texture2D finalNoiseTex = new Texture2D(texSize, texSize);
                        RenderTexture finalRenderTarget = new RenderTexture(finalNoiseTex.width, finalNoiseTex.height, 0, RenderTextureFormat.ARGB32);
                        Graphics.Blit(finalNoiseTex, finalRenderTarget, noiseMaterial);
                        finalNoiseTex.ReadPixels(new Rect(0, 0, finalRenderTarget.width, finalRenderTarget.height), 0, 0);
                        finalNoiseTex.Apply();

                        byte[] pngData = finalNoiseTex.EncodeToPNG();
                        if(pngData != null) File.WriteAllBytes(path, pngData);
                        AssetDatabase.Refresh();

                        if(path.IndexOf("Assets/") >= 0)
                        {
                            string subPath = path.Substring(path.IndexOf("Assets/"));
                            TextureImporter importer = AssetImporter.GetAtPath(subPath) as TextureImporter;
                            if(importer != null)
                            {
                                SceneViewNotificationAndLog("Noise saved inside the project: " + subPath);
                                importer.filterMode = noiseFiltering;
                                importer.SaveAndReimport();
                                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(subPath, typeof(Texture)));
                            }
                        }
                        else SceneViewNotificationAndLog("Noise saved outside the project: " + path);
                    }
                }
            }
        }

        private void UpdateNoiseMatAndRender()
        {
            if(noiseType == NoiseTypes.Fractal || noiseType == NoiseTypes.Perlin || noiseType == NoiseTypes.Billow)
            {
                noiseMaterial.SetFloat("_EndBand", noiseFractalAmount);
            }
            else noiseMaterial.SetFloat("_Jitter", noiseJitter);

            noiseMaterial.SetFloat("_ScaleX", noiseScaleX);
            noiseMaterial.SetFloat("_ScaleY", noiseScaleY);
            noiseMaterial.SetFloat("_Offset", (float) noiseSeed);
            noiseMaterial.SetFloat("_Contrast", noiseContrast);
            noiseMaterial.SetFloat("_Brightness", noiseBrightness);
            noiseMaterial.SetFloat("_Invert", noiseInverted ? 1f : 0f);

            Graphics.Blit(noisePreview, noiseRenderTarget, noiseMaterial);
            noisePreview.ReadPixels(new Rect(0, 0, noiseRenderTarget.width, noiseRenderTarget.height), 0, 0);
            noisePreview.Apply();
        }

        private static bool IsAssetAFolder(Object obj)
        {
            string path = "";

            if(obj == null) return false;

            path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

            if(path.Length > 0)
            {
                if(Directory.Exists(path)) return true;
                else return false;
            }

            return false;
        }

        private void DrawLine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += (padding / 2f);
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        private void OnFocus()
        {
            currTab = PlayerPrefs.GetInt("AllIn1ShaderNodesWindowTab");
        }

        private void OnLostFocus()
        {
            PlayerPrefs.SetInt("AllIn1ShaderNodesWindowTab", currTab);
        }

        private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            targetWidth = Mathf.ClosestPowerOfTwo(targetWidth);
            targetHeight = Mathf.ClosestPowerOfTwo(targetHeight);

            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
            Color[] scaledPixels = result.GetPixels(0);
            float incX = ((float) 1 / source.width) * ((float) source.width / targetWidth);
            float incY = ((float) 1 / source.height) * ((float) source.height / targetHeight);
            for(int px = 0; px < scaledPixels.Length; px++) scaledPixels[px] = source.GetPixelBilinear(incX * ((float) px % targetWidth), incY * (float) Mathf.Floor(px / targetWidth));

            result.SetPixels(scaledPixels, 0);
            result.Apply();
            return result;
        }

        private static Texture2D CreateNormalMap(Texture2D t, float normalMult = 5f, int normalSmooth = 0)
        {
            int width = t.width;
            int height = t.height;
            Color[] sourcePixels = t.GetPixels();
            Color[] resultPixels = new Color[width * height];
            Vector3 vScale = new Vector3(0.3333f, 0.3333f, 0.3333f);

            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    int index = x + y * width;
                    Vector3 cSampleNegXNegY = GetPixelClamped(sourcePixels, x - 1, y - 1, width, height);
                    Vector3 cSampleZerXNegY = GetPixelClamped(sourcePixels, x, y - 1, width, height);
                    Vector3 cSamplePosXNegY = GetPixelClamped(sourcePixels, x + 1, y - 1, width, height);
                    Vector3 cSampleNegXZerY = GetPixelClamped(sourcePixels, x - 1, y, width, height);
                    Vector3 cSamplePosXZerY = GetPixelClamped(sourcePixels, x + 1, y, width, height);
                    Vector3 cSampleNegXPosY = GetPixelClamped(sourcePixels, x - 1, y + 1, width, height);
                    Vector3 cSampleZerXPosY = GetPixelClamped(sourcePixels, x, y + 1, width, height);
                    Vector3 cSamplePosXPosY = GetPixelClamped(sourcePixels, x + 1, y + 1, width, height);

                    float fSampleNegXNegY = Vector3.Dot(cSampleNegXNegY, vScale);
                    float fSampleZerXNegY = Vector3.Dot(cSampleZerXNegY, vScale);
                    float fSamplePosXNegY = Vector3.Dot(cSamplePosXNegY, vScale);
                    float fSampleNegXZerY = Vector3.Dot(cSampleNegXZerY, vScale);
                    float fSamplePosXZerY = Vector3.Dot(cSamplePosXZerY, vScale);
                    float fSampleNegXPosY = Vector3.Dot(cSampleNegXPosY, vScale);
                    float fSampleZerXPosY = Vector3.Dot(cSampleZerXPosY, vScale);
                    float fSamplePosXPosY = Vector3.Dot(cSamplePosXPosY, vScale);

                    float edgeX = (fSampleNegXNegY - fSamplePosXNegY) * 0.25f + (fSampleNegXZerY - fSamplePosXZerY) * 0.5f + (fSampleNegXPosY - fSamplePosXPosY) * 0.25f;
                    float edgeY = (fSampleNegXNegY - fSampleNegXPosY) * 0.25f + (fSampleZerXNegY - fSampleZerXPosY) * 0.5f + (fSamplePosXNegY - fSamplePosXPosY) * 0.25f;

                    Vector2 vEdge = new Vector2(edgeX, edgeY) * normalMult;
                    Vector3 norm = new Vector3(vEdge.x, vEdge.y, 1.0f).normalized;
                    resultPixels[index] = new Color(norm.x * 0.5f + 0.5f, norm.y * 0.5f + 0.5f, norm.z * 0.5f + 0.5f, 1);
                }
            }

            if(normalSmooth > 0)
            {
                resultPixels = SmoothNormals(resultPixels, width, height, normalSmooth);
            }

            Texture2D texNormal = new Texture2D(width, height, TextureFormat.RGB24, false, false);
            texNormal.SetPixels(resultPixels);
            texNormal.Apply();
            return texNormal;
        }

        private static Vector3 GetPixelClamped(Color[] pixels, int x, int y, int width, int height)
        {
            x = Mathf.Clamp(x, 0, width - 1);
            y = Mathf.Clamp(y, 0, height - 1);
            Color c = pixels[x + y * width];
            return new Vector3(c.r, c.g, c.b);
        }

        private static Color[] SmoothNormals(Color[] pixels, int width, int height, int normalSmooth)
        {
            Color[] smoothedPixels = new Color[pixels.Length];
            float step = 0.00390625f * normalSmooth;

            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    float pixelsToAverage = 0.0f;
                    Color c = pixels[x + y * width];
                    pixelsToAverage++;

                    for(int offsetY = -normalSmooth; offsetY <= normalSmooth; offsetY++)
                    {
                        for(int offsetX = -normalSmooth; offsetX <= normalSmooth; offsetX++)
                        {
                            if(offsetX == 0 && offsetY == 0) continue;

                            int sampleX = Mathf.Clamp(x + offsetX, 0, width - 1);
                            int sampleY = Mathf.Clamp(y + offsetY, 0, height - 1);

                            c += pixels[sampleX + sampleY * width];
                            pixelsToAverage++;
                        }
                    }

                    smoothedPixels[x + y * width] = c / pixelsToAverage;
                }
            }

            return smoothedPixels;
        }

        private Color editorColorTint = Color.white;
        private float brightness = 0f, contrast = 1f, gamma = 1f, exposure = 0f, saturation = 1f, hue = 0f;
        private bool invert = false, greyscale = false, fullWhite = false, blackBackground = false, alphaGreyscale = false, showOriginalImage = false;
        private bool isFlipHorizontal = false, isFlipVertical = false;
        private int rotationAmount = 0;
        private float exportScale = 1f;

        private void TextureEditor()
        {
            EditorGUI.BeginChangeCheck();
            editorTexInput = EditorGUILayout.ObjectField("Image to Edit", editorTexInput, typeof(Texture2D), false, GUILayout.Width(300), GUILayout.Height(50)) as Texture2D;
            if(EditorGUI.EndChangeCheck())
            {
                if(editorTexInput != null)
                {
                    SetTextureReadWrite(AssetDatabase.GetAssetPath(editorTexInput), true);

                    editorTex = new Texture2D(editorTexInput.width, editorTexInput.height);
                    editorTex.SetPixels(editorTexInput.GetPixels());
                    editorTex.Apply();

                    float aspectRatio = (float) editorTex.width / (float) editorTex.height;
                    int width = Mathf.Min(editorTex.width, 256);
                    editorTex = ScaleTexture(editorTex, width, (int) (width / aspectRatio));

                    cleanEditorTex = new Texture2D(editorTex.width, editorTex.height);
                    cleanEditorTex.SetPixels(editorTex.GetPixels());
                    cleanEditorTex.Apply();

                    SetTextureEditorDefaultValues();
                    RecalculateEditorTexture();
                }
                else editorTex = null;
            }

            DrawLine(Color.grey, 1, 3);

            if(editorTex != null)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if(!showOriginalImage) GUILayout.Label(editorTex);
                    else GUILayout.Label(cleanEditorTex);
                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUI.BeginChangeCheck();
                        TextureEditorColorParameter("Color Tint", ref editorColorTint, Color.white);
                        TextureEditorFloatParameter("Brightness", ref brightness, -1f, 5f);
                        TextureEditorFloatParameter("Contrast", ref contrast, 0.0f, 5.0f, 1f);
                        TextureEditorFloatParameter("Gamma", ref gamma, 0.0f, 10f, 1f);
                        TextureEditorFloatParameter("Exposure", ref exposure, -5f, 5f, 0f);
                        TextureEditorFloatParameter("Saturation", ref saturation, 0f, 5f, 1f);
                        TextureEditorFloatParameter("Hue", ref hue, 0f, 360f, 0f);

                        EditorGUILayout.BeginHorizontal();
                        {
                            invert = EditorGUILayout.Toggle("Invert", invert, GUILayout.Width(253));
                            greyscale = EditorGUILayout.Toggle("Greyscale", greyscale);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            fullWhite = EditorGUILayout.Toggle("Fully white", fullWhite, GUILayout.Width(253));
                            blackBackground = EditorGUILayout.Toggle("Black background", blackBackground);
                        }
                        EditorGUILayout.EndHorizontal();
                        alphaGreyscale = EditorGUILayout.Toggle("Greyscale is alpha", alphaGreyscale);
                        if(EditorGUI.EndChangeCheck()) RecalculateEditorTexture();

                        EditorGUILayout.Space();
                        EditorGUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("Rotate Left 90", GUILayout.MaxWidth(210))) RotateEditorTextureLeft();
                            if(GUILayout.Button("Rotate Right 90", GUILayout.MaxWidth(210))) for (int i = 0; i < 3; i++) RotateEditorTextureLeft();
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            if(GUILayout.Button("Flip Horizontal", GUILayout.MaxWidth(210))) FlipEditorTexture(true);
                            if(GUILayout.Button("Flip Vertical", GUILayout.MaxWidth(210))) FlipEditorTexture(false);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();
                        if(!showOriginalImage)
                        {
                            if(GUILayout.Button("Press to show Original Image", GUILayout.MaxWidth(425))) showOriginalImage = true;
                        }
                        else
                        {
                            Color backgroundColor = GUI.backgroundColor;
                            GUI.backgroundColor = Color.red;
                            if(GUILayout.Button("Press to show Editor Image",  GUILayout.MaxWidth(425))) showOriginalImage = false;
                            GUI.backgroundColor = backgroundColor;
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Label("*Preview is locked to 256px maximum (bigger textures are scaled down), but the image will be saved to its full resolution", EditorStyles.boldLabel);
            }
            else
            {
                GUILayout.Label("Please select an Image to Edit above", EditorStyles.boldLabel);
                return;
            }

            DrawLine(Color.grey, 1, 3);
            EditorGUILayout.Space();
            TextureEditorFloatParameter("Export Scale", ref exportScale, 0.01f, 2f, 1f);
            int currWidth = Mathf.ClosestPowerOfTwo((int)(editorTexInput.width * exportScale));
            int currHeight = Mathf.ClosestPowerOfTwo((int)(editorTexInput.height * exportScale));
            GUILayout.Label("Current export size is: "+ currWidth + " x " + currHeight + " (size snaps to the closest power of 2)", EditorStyles.boldLabel);
            if(GUILayout.Button("Save Resulting Image as PNG file", GUILayout.MaxWidth(BUTTON_WIDTH)))
            {
                string fullPath = AssetDatabase.GetAssetPath(editorTexInput);
                string path = fullPath.Replace(Path.GetFileName(fullPath), "");

                if(File.Exists(fullPath)) fullPath = GetNewValidPath(path + Path.GetFileName(fullPath));

                string fileName = fullPath.Replace(path, "");
                fileName = fileName.Replace(".png", "");
                fullPath = EditorUtility.SaveFilePanel("Save Render Image", path, fileName, "png");
                if(fullPath.Length == 0) return;
                string pingPath = fullPath;

                ComputeFinalTexture();

                byte[] bytes = editorTexInput.EncodeToPNG();
                File.WriteAllBytes(pingPath, bytes);
                AssetDatabase.ImportAsset(pingPath);
                AssetDatabase.Refresh();
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(pingPath, typeof(Texture)));
                SceneViewNotificationAndLog("Edited Image saved to: " + fullPath);

                editorTexInput = null;
                editorTex = null;
                cleanEditorTex = null;
                SetTextureEditorDefaultValues();
            }
        }

        private void ComputeFinalTexture()
        {
            Color[] pixels;
            int texWidth, texHeight;

            for(int i = 0; i < rotationAmount; i++)
            {
                texWidth = editorTexInput.width;
                texHeight = editorTexInput.height;
                pixels = editorTexInput.GetPixels();
                pixels = RotateClockWise(pixels, texWidth, texHeight);
                editorTexInput = new Texture2D(texHeight, texWidth);
                editorTexInput.SetPixels(pixels);
                editorTexInput.Apply();
            }

            pixels = editorTexInput.GetPixels();
            texWidth = editorTexInput.width;
            texHeight = editorTexInput.height;
            if(isFlipHorizontal) pixels = FlipHorizontal(pixels, texWidth, texHeight);
            if(isFlipVertical) pixels = FlipVertical(pixels, texWidth, texHeight);

            ComputeImageColorFilters(pixels);
            editorTexInput = new Texture2D(texWidth, texHeight);
            editorTexInput.SetPixels(pixels);
            editorTexInput.Apply();

            if(Math.Abs(exportScale - 1f) > 0.05f) editorTexInput = ScaleTexture(editorTexInput, (int)(texWidth * exportScale), (int)(texHeight * exportScale));
        }

        private void SetTextureEditorDefaultValues()
        {
            editorColorTint = Color.white;
            brightness = 0f;
            contrast = 1f;
            gamma = 1f;
            exposure = 0f;
            saturation = 1f;
            hue = 0f;
            invert = false;
            greyscale = false;
            fullWhite = false;
            blackBackground = false;
            alphaGreyscale = false;
            showOriginalImage = false;
            isFlipHorizontal = false;
            isFlipVertical = false;
            rotationAmount = 0;
            exportScale = 1f;
        }

        private string GetNewValidPath(string path, int i = 1)
        {
            int number = i;
            path = path.Replace(".png", "");
            string newPath = path + "_" + number.ToString();
            string fullPath = newPath + ".png";
            if(System.IO.File.Exists(fullPath))
            {
                number++;
                fullPath = GetNewValidPath(path, number);
            }

            return fullPath;
        }

        private void RecalculateEditorTexture()
        {
            Color[] pixels = cleanEditorTex.GetPixels();
            int texWidth = cleanEditorTex.width;
            int texHeight = cleanEditorTex.height;

            ComputeImageColorFilters(pixels);

            editorTex = new Texture2D(texWidth, texHeight);
            editorTex.SetPixels(pixels);
            editorTex.Apply();
        }

        private void ComputeImageColorFilters(Color[] pixels)
        {
            float cosHsv = saturation * Mathf.Cos(hue * 3.14159265f / 180f);
            float sinHsv = saturation * Mathf.Sin(hue * 3.14159265f / 180f);

            for(int i = 0; i < pixels.Length; i++)
            {
                pixels[i].r = Mathf.Clamp01(((pixels[i].r - 0.5f) * contrast) + 0.5f);
                pixels[i].g = Mathf.Clamp01(((pixels[i].g - 0.5f) * contrast) + 0.5f);
                pixels[i].b = Mathf.Clamp01(((pixels[i].b - 0.5f) * contrast) + 0.5f);

                pixels[i] = new Color(Mathf.Clamp01(pixels[i].r * (1 + brightness)), Mathf.Clamp01(pixels[i].g * (1 + brightness)), Mathf.Clamp01(pixels[i].b * (1 + brightness)), pixels[i].a);

                pixels[i].r = Mathf.Pow(Mathf.Abs(pixels[i].r), gamma);
                pixels[i].g = Mathf.Pow(Mathf.Abs(pixels[i].g), gamma);
                pixels[i].b = Mathf.Pow(Mathf.Abs(pixels[i].b), gamma);

                pixels[i].r = Mathf.Clamp01(pixels[i].r * Mathf.Pow(2, exposure));
                pixels[i].g = Mathf.Clamp01(pixels[i].g * Mathf.Pow(2, exposure));
                pixels[i].b = Mathf.Clamp01(pixels[i].b * Mathf.Pow(2, exposure));

                pixels[i] *= editorColorTint;

                Color hueShiftColor = pixels[i];
                hueShiftColor.r = Mathf.Clamp01((.299f + .701f * cosHsv + .168f * sinHsv) * pixels[i].r + (.587f - .587f * cosHsv + .330f * sinHsv) * pixels[i].g + (.114f - .114f * cosHsv - .497f * sinHsv) * pixels[i].b);
                hueShiftColor.g = Mathf.Clamp01((.299f - .299f * cosHsv - .328f * sinHsv) * pixels[i].r + (.587f + .413f * cosHsv + .035f * sinHsv) * pixels[i].g + (.114f - .114f * cosHsv + .292f * sinHsv) * pixels[i].b);
                hueShiftColor.b = Mathf.Clamp01((.299f - .3f * cosHsv + 1.25f * sinHsv) * pixels[i].r + (.587f - .588f * cosHsv - 1.05f * sinHsv) * pixels[i].g + (.114f + .886f * cosHsv - .203f * sinHsv) * pixels[i].b);
                pixels[i] = hueShiftColor;

                if(invert) pixels[i] = new Color(1 - pixels[i].r, 1 - pixels[i].g, 1 - pixels[i].b, pixels[i].a);

                if(greyscale || fullWhite || alphaGreyscale)
                {
                    float greyScale = pixels[i].r * 0.59f + pixels[i].g * 0.3f + pixels[i].b * 0.11f;

                    if(fullWhite) pixels[i] = new Color(1, 1, 1, greyScale);
                    else if(greyscale) pixels[i] = new Color(greyScale, greyScale, greyScale, pixels[i].a);

                    if(alphaGreyscale) pixels[i] = new Color(pixels[i].r, pixels[i].g, pixels[i].b, greyScale);
                }

                if(blackBackground)
                {
                    if(pixels[i].a < 0.05f)  pixels[i] = new Color(pixels[i].a, pixels[i].a, pixels[i].a, 1);
                    else pixels[i] = new Color(pixels[i].r, pixels[i].g, pixels[i].b, 1);
                }
            }
        }

        private void TextureEditorFloatParameter(string parameterName, ref float parameter, float rangeMin = -100f, float rangeMax = 100f, float resetValue = 0f)
        {
            EditorGUILayout.BeginHorizontal();
            {
                parameter = EditorGUILayout.Slider(parameterName, parameter, rangeMin, rangeMax, GUILayout.MaxWidth(400));
                GUIContent resetButtonLabel = new GUIContent
                {
                    text = "R",
                    tooltip = "Resets to default value"
                };
                if(GUILayout.Button(resetButtonLabel, GUILayout.Width(20))) parameter = resetValue;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void TextureEditorIntParameter(string parameterName, ref int parameter, int rangeMin = -100, int rangeMax = 100, int resetValue = 0)
        {
            EditorGUILayout.BeginHorizontal();
            {
                parameter = EditorGUILayout.IntSlider(parameterName, parameter, rangeMin, rangeMax, GUILayout.MaxWidth(400));
                GUIContent resetButtonLabel = new GUIContent
                {
                    text = "R",
                    tooltip = "Resets to default value"
                };
                if(GUILayout.Button(resetButtonLabel, GUILayout.Width(20))) parameter = resetValue;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void TextureEditorColorParameter(string parameterName, ref Color parameter, Color resetValue)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUIContent colorLabel = new GUIContent
                {
                    text = parameterName,
                    tooltip = parameterName
                };
                parameter = EditorGUILayout.ColorField(colorLabel, parameter, true, true, true, GUILayout.MaxWidth(400));
                GUIContent resetButtonLabel = new GUIContent
                {
                    text = "R",
                    tooltip = "Resets to default value"
                };
                if(GUILayout.Button(resetButtonLabel, GUILayout.Width(20))) parameter = resetValue;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void RotateEditorTextureLeft()
        {
            Color[] pixels = editorTex.GetPixels();
            Color[] pixelsClean = cleanEditorTex.GetPixels();
            int texWidth = editorTex.width;
            int texHeight = editorTex.height;

            pixels = RotateClockWise(pixels, texWidth, texHeight);
            pixelsClean = RotateClockWise(pixelsClean, texWidth, texHeight);

            editorTex = new Texture2D(texHeight, texWidth); //Width and Height get swapped to account for rotation
            editorTex.SetPixels(pixels);
            editorTex.Apply();
            cleanEditorTex = new Texture2D(texHeight, texWidth); //Width and Height get swapped to account for rotation
            cleanEditorTex.SetPixels(pixelsClean);
            cleanEditorTex.Apply();

            rotationAmount = (rotationAmount + 1) % 4;
        }

        private Color[] RotateClockWise(Color[] pixels, int width, int height)
        {
            Color[] outputPixels = new Color[pixels.Length];
            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    int i1 = GetPixelIndex(x, height - y - 1, width);
                    int i2 = GetPixelIndex(y, x, height);
                    outputPixels[i2] = pixels[i1];
                }
            }

            return outputPixels;
        }

        private void FlipEditorTexture(bool isHorizontal)
        {
            Color[] pixels = editorTex.GetPixels();
            Color[] pixelsClean = cleanEditorTex.GetPixels();
            int texWidth = editorTex.width;
            int texHeight = editorTex.height;

            if(isHorizontal)
            {
                pixels = FlipHorizontal(pixels, texWidth, texHeight);
                pixelsClean = FlipHorizontal(pixelsClean, texWidth, texHeight);
                isFlipHorizontal = !isFlipHorizontal;
            }
            else
            {
                pixels = FlipVertical(pixels, texWidth, texHeight);
                pixelsClean = FlipVertical(pixelsClean, texWidth, texHeight);
                isFlipVertical = !isFlipVertical;
            }

            editorTex = new Texture2D(texWidth, texHeight);
            editorTex.SetPixels(pixels);
            editorTex.Apply();
            cleanEditorTex = new Texture2D(texWidth, texHeight);
            cleanEditorTex.SetPixels(pixelsClean);
            cleanEditorTex.Apply();
        }

        private Color[] FlipHorizontal(Color[] pixels, int width, int height)
        {
            Color[] outputPixels = new Color[pixels.Length];
            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    int i1 = GetPixelIndex(x, y, width);
                    int i2 = GetPixelIndex(width - 1 - x, y, width);
                    outputPixels[i1] = pixels[i2];
                }
            }

            return outputPixels;
        }

        private Color[] FlipVertical(Color[] pixels, int width, int height)
        {
            Color[] outputPixels = new Color[pixels.Length];
            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    int i1 = GetPixelIndex(x, y, width);
                    int i2 = GetPixelIndex(x, height - 1 - y, width);
                    outputPixels[i1] = pixels[i2];
                }
            }

            return outputPixels;
        }

        private int GetPixelIndex(int x, int y, int width)
        {
            return y * width + x;
        }

        private void OnEnable()
        {
            GetBasePath();
#if UNITY_2019_3_OR_NEWER
            RenderPipelineChecker.RefreshData();
#endif
        }

        private static void GetBasePath()
        {
            string[] guids = AssetDatabase.FindAssets("t:folder AllIn1ShaderNodes");
            if(guids.Length > 0)
            {
                basePath = AssetDatabase.GUIDToAssetPath(guids[0]);
            }
            else
            {
                Debug.LogError("AllIn1ShaderNodes folder not found in the project.");
                basePath = "Assets/Plugins/AllIn1ShaderNodes";
            }
        }

        public static void SceneViewNotificationAndLog(string message)
        {
            Debug.Log(message);
            ShowSceneViewNotification(message);
        }

        public static Shader FindShader(string shaderName)
        {
            string[] guids = AssetDatabase.FindAssets($"{shaderName} t:shader");
            foreach(string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
                if(shader != null)
                {
                    string fullShaderName = shader.name;
                    string actualShaderName = fullShaderName.Substring(fullShaderName.LastIndexOf('/') + 1);
                    if(actualShaderName == shaderName) return shader;
                }
            }
            return null;
        }

        public static void ShowSceneViewNotification(string message)
        {
            GUIContent content = new GUIContent(message);
            #if UNITY_2019_1_OR_NEWER
            SceneView.lastActiveSceneView.ShowNotification(content, 1.5f);
            #else
            SceneView.lastActiveSceneView.ShowNotification(content);
            #endif
        }
    }
}
#endif
