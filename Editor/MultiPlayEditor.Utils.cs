using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Debug = UnityEngine.Debug;

namespace MultiPlay
{
    // Partial class for utility methods
    internal partial class MultiPlayEditor
    {
        #region Event Handlers
        private void HandleOnPlayModeChanged(PlayModeStateChange obj)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode && isClone && !autoSync)
            {
                ReloadScene(SceneManager.GetActiveScene().path);
                hasChanged = false;
            }
        }

        private static void OnSceneChanged(string sceneName)
        {
            if (!isClone) return;
            
            try
            {
                lastWriteTime = File.GetLastWriteTime(sceneFilePath);
                if (autoSync)
                {
                    ReloadScene(sceneName);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}");
            }
        }
        #endregion

        #region Utility Methods
        private static async Task<long> GetDirSize(string searchDirectory)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@searchDirectory);
            long dirSize = await Task.Run(() =>
                dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length));
            return dirSize;
        }

        private bool IsSymbolic(string path)
        {
            FileInfo pathInfo = new FileInfo(path);
            return pathInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
        }

        public static int DoLinksExist()
        {
            int cnt = 0;
            for (int i = 1; i < Settings.MaxClones + 1; i++)
            {
                string destinationPath = $"{Settings.ClonesPath}/{cloneCaption}_[{i}]_Clone".Replace(@"/", @"\");
                if (Directory.Exists(destinationPath)) cnt++;
            }
            return cnt;
        }

        public static bool DoLinksLive()
        {
            bool result = false;
            for (int i = 1; i < Settings.MaxClones + 1; i++)
            {
                string destinationPath = $"{Settings.ClonesPath}/{cloneCaption}_[{i}]_Clone".Replace(@"/", @"\");
                if (i == 1) result = Directory.Exists(destinationPath + "\\Temp");
                result = result || Directory.Exists(destinationPath + "\\Temp");
            }
            return result;
        }

        private static void ReloadScene(string scenePath)
        {
            if(Application.isPlaying) return;
            
            string activeScene = SceneManager.GetActiveScene().path;
            if (string.IsNullOrEmpty(activeScene))
            {
                Debug.LogWarning("No Active Scene to reload.");
                return;
            }
            
            try
            { 
                _mainThreadContext?.Post(state =>
                {
                    EditorSceneManager.OpenScene(scenePath);
                    Canvas.ForceUpdateCanvases();
                }, null);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error reloading Scene. {e.Message}");
            }
        }

        private static void ClearConsole()
        {
            var logEntries = Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            if (logEntries != null)
            {
                var clearMethod = logEntries.GetMethod("Clear",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                clearMethod?.Invoke(null, null);
            }
        }

        private int GetCurrentCloneIndex()
        {
            try
            {
                string projectPath = Application.dataPath.Replace("/Assets", "");
                string directoryName = new DirectoryInfo(projectPath).Name;
                
                if (!directoryName.Contains("_[") || !directoryName.Contains("]_Clone"))
                    return 0;
                
                string indexStr = directoryName.Split('[', ']')[1];
                if (int.TryParse(indexStr, out int index))
                    return index;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error determining clone index: {e.Message}");
            }
            
            return 0;
        }

        private bool IsLibraryLinked()
        {
            try
            {
                string libraryPath = $"{Application.dataPath}/../Library";
                if (!Directory.Exists(libraryPath))
                    return false;
                
                FileInfo pathInfo = new FileInfo(libraryPath);
                return pathInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error checking if library is linked: {e.Message}");
                return false;
            }
        }
        #endregion
    }
} 