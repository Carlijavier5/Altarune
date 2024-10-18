using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
using System.Reflection;

namespace Adobe.Substance.Connector.Unity
{
    [InitializeOnLoad]
    static class SubstanceConnectorEditor
    {
        private static readonly object InitializedLock = new object();

        static bool started = false;
        static SubstanceConnectorEditor() {
            AssemblyReloadEvents.beforeAssemblyReload += TearDown;
            AssemblyReloadEvents.afterAssemblyReload += Setup; 
            Setup();
        }

        private static ConcurrentQueue<ImportFileMessage> _ImportFiles;

        private static Dictionary<string, string> _monitoredFiles = new Dictionary<string, string>();
        private static void Setup()
        {
            lock (InitializedLock) 
            {
                if(!started) {

                    Instance.Initialize();
                    Instance.OnImportFile += OnFileImportMessageReceived;
                    _ImportFiles = new ConcurrentQueue<ImportFileMessage>();
                    EditorApplication.update += Update;
                    EditorApplication.quitting += Quiting;
                    started = true;
                }
            }
        }

        private static void OnFileImportMessageReceived(object sender, ConnectorEventArgs e)
        {
            var message = JsonUtility.FromJson<ImportFileMessage>(e.Message);
            _ImportFiles.Enqueue(message);
        }

        static void TearDown()
        {
            lock (InitializedLock) {
                if(started){
                    Instance.Shutdown();
                    Instance.OnImportFile -= OnFileImportMessageReceived;
                    EditorApplication.update -= Update;
                    EditorApplication.quitting -= Quiting;
                    started = false;
                }
            }
        }
        

        static void Update()
        {
            while (_ImportFiles.TryDequeue(out ImportFileMessage importMessage))
            {              
                HandleImportMessage(importMessage);
            }
        }

        private static void Quiting()
        {
            Instance.Shutdown();
            Instance.SignalApplicationShutdown();
        }
        private static void HandleImportMessage(ImportFileMessage importMessage)
        {
            var importPath = Path.GetFullPath(importMessage.path);
            var importFileName = Path.GetFileName(importPath);

            var destFolder = GetCurrentDestinationFolder();
            var destFullPath = Path.Combine(Path.GetFullPath(destFolder), importFileName);
            var destRelativePath = Path.Combine("Assets/", Path.GetRelativePath(UnityEngine.Application.dataPath, destFullPath));

            if (!File.Exists(importPath))
            {
                Debug.LogWarning($"Unable to import file from {importPath}, file does not exist.");
                return;
            }

            if (!_monitoredFiles.TryGetValue(importMessage.uuid, out string unityGUID))
            {
                ImportNewFile(importPath, destFullPath, importMessage.uuid);
            }
            else
            {
                ImportExistingFile(importPath, destFullPath, unityGUID, importMessage.uuid);
            }
        }

        private static void ImportNewFile(string importPath, string destFullPath, string messageUUID)
        {
            var destRelativePath = Path.Combine("Assets/", Path.GetRelativePath(UnityEngine.Application.dataPath, destFullPath));

            File.Copy(importPath, destFullPath, true);

            AssetDatabase.ImportAsset(destRelativePath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();

            var unityGUID = AssetDatabase.AssetPathToGUID(destRelativePath);
            _monitoredFiles.Add(messageUUID, unityGUID);
        }

        private static void ImportExistingFile(string importPath, string destFullPath, string unityGUID, string messageUUID)
        {
            var destRelativePath = Path.Combine("Assets/", Path.GetRelativePath(Application.dataPath, destFullPath));

            // Update current asset.
            var assetPath = AssetDatabase.GUIDToAssetPath(unityGUID);
            var oldPath = Path.GetFullPath(assetPath);

            if (File.Exists(oldPath))
            {
                File.Copy(importPath, oldPath, true);
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

                if (NormalizePath(assetPath).Equals(NormalizePath(destRelativePath)))
                    return;

                var assetRelativeDest = Path.GetFileName(Path.GetRelativePath(Application.dataPath, destFullPath));

                var result = AssetDatabase.RenameAsset(assetPath, assetRelativeDest);
                AssetDatabase.Refresh();

                if (!string.IsNullOrEmpty(result))
                    Debug.LogError(result);
            }
            else
            {
                _monitoredFiles.Remove(messageUUID);
                ImportNewFile(importPath, destFullPath, messageUUID);
            }
        }
    
        private static string GetCurrentDestinationFolder()
        {
            string path;

            if (TryGetActiveFolderPath(out path))
            {
                return path;
            }

            var obj = Selection.activeObject;

            if (obj == null) 
                return UnityEngine.Application.dataPath;
            else 
                path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

            if (path.Length == 0)
                return UnityEngine.Application.dataPath;

            return Directory.Exists(path) ? path : Path.GetDirectoryName(path);
        }    

        private static string NormalizePath(string path)
        {
            return path.Replace("\\", "/").Replace("\\\\", "/");
        }

        // Define this function somewhere in your editor class to make a shortcut to said hidden function
        private static bool TryGetActiveFolderPath(out string path)
        {
            var _tryGetActiveFolderPath = typeof(ProjectWindowUtil).GetMethod("TryGetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);

            if (_tryGetActiveFolderPath == null) 
            {
                path = null;
                return false;
            }

            object[] args = new object[] { null };
            bool found = (bool)_tryGetActiveFolderPath.Invoke(null, args);
            path = (string)args[0];

            return found;
        }

    }
}
