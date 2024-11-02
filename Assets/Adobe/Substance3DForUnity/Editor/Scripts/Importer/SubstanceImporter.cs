using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;
using Adobe.Substance;
using static UnityEngine.Networking.UnityWebRequest;


#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

namespace Adobe.SubstanceEditor.Importer
{
    internal class SubstanceAssetModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions rao)
        {
            if (string.IsNullOrEmpty(assetPath))
                return AssetDeleteResult.DidNotDelete;

            if (AssetDatabase.IsValidFolder(assetPath))
                return CanDeleteFolder(assetPath, rao) ? AssetDeleteResult.DidNotDelete : AssetDeleteResult.FailedDelete;

            var substanceInstance = AssetDatabase.LoadAssetAtPath<SubstanceGraphSO>(assetPath);

            if (substanceInstance != null)
            {
                if (substanceInstance.FlagedForDelete || !File.Exists(substanceInstance.AssetPath))
                {
                    return AssetDeleteResult.DidNotDelete;
                }
                else
                {
                    Debug.LogWarning($"The target file cannot be manually deleted because it is associated with {substanceInstance.AssetPath}. In order to delete it, first the .sbsar file must be deleted.");
                    return AssetDeleteResult.FailedDelete;
                }
            }

            if (Path.GetExtension(assetPath.ToLower()) != ".sbsar")
                return AssetDeleteResult.DidNotDelete;

            SubstanceImporter importer = AssetImporter.GetAtPath(assetPath) as SubstanceImporter;

            if (importer != null)
            {
                foreach (var materialInstance in importer._fileAsset.GetGraphs())
                {
                    if (materialInstance == null)
                        continue;

                    SubstanceEditorEngine.instance.ReleaseInstance(materialInstance);
                    materialInstance.FlagedForDelete = true;
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(materialInstance));
                }
            }

            return AssetDeleteResult.DidNotDelete;
        }

        public static AssetMoveResult OnWillMoveAsset(string from, string to)
        {
            if (string.IsNullOrEmpty(from))
                return AssetMoveResult.DidNotMove;

            AssetDatabase.Refresh();

            if (Path.GetExtension(from.ToLower()) == ".asset")
            {
                var substanceInstance = AssetDatabase.LoadAssetAtPath<SubstanceGraphSO>(from);

                if (substanceInstance != null)
                {
                    substanceInstance.Move(to);
                }
                else
                {
                    SubstanceEditorEngine.instance.PushMoveOperation(from, to);
                }

                return AssetMoveResult.DidNotMove;
            }

            if (Path.GetExtension(from.ToLower()) != ".sbsar")
                return AssetMoveResult.DidNotMove;

            var oldFileName = Path.GetFileName(from);
            var newFileName = Path.GetFileName(to);

            if (!oldFileName.Equals(newFileName))
            {
                HandleFileNameChange(AssetDatabase.LoadAssetAtPath<SubstanceFileSO>(from), oldFileName, newFileName);
            }

            var importer = AssetImporter.GetAtPath(from) as SubstanceImporter;

            if (importer != null)
            {
                var so = new SerializedObject(importer);
                var prop = so.FindProperty("_assetPath");

                if (prop != null && !string.IsNullOrEmpty(prop.stringValue))
                {
                    prop.stringValue = to;
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
            }

            AssetDatabase.Refresh(ImportAssetOptions.Default);

            var fileObject = AssetDatabase.LoadAssetAtPath<SubstanceFileSO>(from);

            foreach (var materialInstance in fileObject.GetGraphs())
                materialInstance.AssetPath = to;

            return AssetMoveResult.DidNotMove;
        }

        /// <summary>
        /// Checks if the target folder has sbsar file generated assets that can be deleted or not.
        /// </summary>
        /// <param name="assetPath">Path to the target folder.</param>
        /// <param name="rao">Remove asset options.</param>
        /// <returns>True if the folder can be deleted.</returns>
        private static bool CanDeleteFolder(string assetPath, RemoveAssetOptions rao)
        {
            var assetsGUIDs = AssetDatabase.FindAssets($"t:{nameof(SubstanceGraphSO)}", new[] { assetPath });

            foreach (var guid in assetsGUIDs)
            {
                var targetPath = AssetDatabase.GUIDToAssetPath(guid);

                var substanceInstance = AssetDatabase.LoadAssetAtPath<SubstanceGraphSO>(targetPath);

                if (substanceInstance != null)
                {
                    if (!substanceInstance.FlagedForDelete && File.Exists(substanceInstance.AssetPath))
                    {
                        Debug.LogWarning($"The target folder cannot be deleted manually because it has assets associated with {substanceInstance.AssetPath}. In order to delete it, first the .sbsar file must be deleted.");
                        return false;
                    }
                }
            }

            return true;
        }
    
        private static void HandleFileNameChange(SubstanceFileSO sbsar, string oldFileName, string newFileName)
        {
            oldFileName = oldFileName.Replace(".sbsar", "");
            newFileName = newFileName.Replace(".sbsar", "");

            var graphs = sbsar.GetGraphs();

            foreach (var graph in graphs)
            {
                var oldAssetsDirectory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(graph));
                var newAssetsDirectory = oldAssetsDirectory.Replace(oldFileName, newFileName);             
                var associatedAssets = Directory.GetFiles(oldAssetsDirectory);

                foreach (var asset in associatedAssets)
                {
                    string oldAssetPath = asset;

                    if (asset.Contains(oldFileName) && !asset.EndsWith(".meta"))
                    {
                        string newAssetName = Path.GetFileName(oldAssetPath).Replace(oldFileName, newFileName);
                        var result = AssetDatabase.RenameAsset(oldAssetPath, newAssetName);

                        if (!string.IsNullOrEmpty(result))
                        {
                            Debug.LogError(result);
                            continue;
                        }
                    }
                }

                if (!Directory.Exists(newAssetsDirectory))
                {
                    newAssetsDirectory = Path.GetFileName(Path.GetRelativePath(oldAssetsDirectory, newAssetsDirectory));
                    var renameResult = AssetDatabase.RenameAsset(oldAssetsDirectory, newAssetsDirectory);

                    if (!string.IsNullOrEmpty(renameResult))
                    {
                        Debug.LogError(renameResult);
                        continue;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Importer for Substance Material Assets using the .sbsar extension .
    /// </summary>
    [ScriptedImporter(Adobe.Substance.Version.ImporterVersion, "sbsar")]
    public sealed class SubstanceImporter : ScriptedImporter
    {
        [SerializeField]
        public SubstanceFileSO _fileAsset;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var rawData = ScriptableObject.CreateInstance<SubstanceFileRawData>();

            try
            {
                rawData.FileContent = File.ReadAllBytes(ctx.assetPath);
            }
            catch (IOException)
            {
                Debug.LogWarning($"Unable to load file {ctx.assetPath} because the file is locked by another process.");
                return;
            }
            
            rawData.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            ctx.AddObjectToAsset("Substance Data", rawData);

            _fileAsset = ScriptableObject.CreateInstance<SubstanceFileSO>();

            if (CheckIfFileIsReimportedByUnity(ctx))
            {
                SubstanceEditorEngine.instance.UpdateGraphsToNewFile(ctx.assetPath, rawData.FileContent);
            }
            else
            {
                CreateSubstanceFile(ctx, rawData);
            }

            ctx.AddObjectToAsset("Substance File", _fileAsset);
            ctx.SetMainObject(_fileAsset);
        }

        public SubstanceGraphSO GetDefaultGraph()
        {
            var graphs = _fileAsset.GetGraphs();

            if (graphs == null || graphs.Count == 0)
                return null;

            return graphs[0];
        }

        private void CreateSubstanceFile(AssetImportContext ctx, SubstanceFileRawData rawData)
        {
            var graphCount = Engine.GetFileGraphCount(rawData.FileContent);

            var initInfos = new List<EditorTools.SubstanceGraphSOCreateInfo>();

            for (int i = 0; i < graphCount; i++)
            {
                initInfos.Add(new EditorTools.SubstanceGraphSOCreateInfo($"graph_{i}", i));
            }

            EditorTools.CreateSubstanceInstanceAsync(ctx.assetPath, rawData, initInfos);
        }

        private bool CheckIfFileIsReimportedByUnity(AssetImportContext ctx)
        {
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(SubstanceGraphSO)));

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                SubstanceGraphSO graph = AssetDatabase.LoadAssetAtPath<SubstanceGraphSO>(assetPath);

                if (graph != null)
                {
                    if (graph.AssetPath.Equals(ctx.assetPath))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}