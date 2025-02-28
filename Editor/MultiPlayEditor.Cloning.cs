using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Debug = UnityEngine.Debug;

namespace MultiPlay
{
    // Partial class handling clone creation and operations
    internal partial class MultiPlayEditor
    {
        #region Cloning Operations
        private void HandleCloneButtonClick(string destinationPath, int i)
        {
            if (!Directory.Exists(destinationPath))
            {
                if (!Settings.LinkLibrary)
                {
                    string sizeInMB = libSize.ToSize(ByteExtensions.SizeUnits.MB);
                    var msg = $"WARNING!\nYou're about to create a clone with {sizeInMB}.\nAre you sure you want to proceed?";
                    var result = EditorUtility.DisplayDialog("Cloning with a library copy", msg, "Proceed", "Cancel");
                    if (!result)
                    {
                        Debug.Log("Operation canceled by user.");
                        return;
                    }
                }

                Debug.Log($"creating clone {i} in {destinationPath.Replace("\\\\", "\\")}");

                Settings.SaveSettings();
                Settings.LoadSettings(this);

                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                isCreatingReferences = false;

                CreateLink(destinationPath, "Assets");
                CreateLink(destinationPath, "ProjectSettings");
                CreateLink(destinationPath, "Packages");

                if (Settings.LinkLibrary)
                    CreateLink(destinationPath, "Library"); //kills auto sync.
            }

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            hasChanged = false;
            LaunchClone(destinationPath);
            CleanUpMenuItem.RemoveFromHub();
        }

        private void CreateLink(string destPath, string subDirectory)
        {
            if (!Directory.Exists(destPath))
                Directory.CreateDirectory(destPath);

            string cmd, args;
            try
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsEditor:
                        cmd = "cmd";
                        args = $"/c mklink /j \"{destPath}\\{subDirectory}\" \"{sourcePath}\\{subDirectory}\"";
                        break;

                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.LinuxEditor:
                        cmd = "/bin/bash";
                        args = $"ln -s \"{sourcePath}/{subDirectory}\" \"{destPath}/{subDirectory}\"";
                        break;

                    default:
                        throw new NotImplementedException("Platform not supported!");
                }

                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = cmd;
                process.StartInfo.Arguments = args;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Debug.LogWarning($"Could not link {subDirectory}, trying again...\n{output}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Links failed. Please contact your system administrator\n{e.Message}");
            }
        }

        private void LaunchClone(string destPath)
        {
            try
            {
                string editorPath = GetAppPath(Application.platform);
                string editorArgs = $"-DisableDirectoryMonitor ‑ignorecompilererrors -disable-assembly-updater -silent-crashes";
                string projectPath = $" -projectPath \"{destPath}\"";

                var thread = new System.Threading.Thread(() => ExcuteCmd($"\"{editorPath}\"", editorArgs + projectPath));
                thread.Start();
                
                if (isClone) ClearConsole();
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"Unable to read temporary files due to insufficient User Privileges. Please contact your system administrator. \nDetails: {e.Message}");
            }
        }

        private string GetAppPath(RuntimePlatform currentPlatform)
        {
            switch (currentPlatform)
            {
                case RuntimePlatform.WindowsEditor:
                    return EditorApplication.applicationPath;
                case RuntimePlatform.OSXEditor:
                    return EditorApplication.applicationPath + "/Contents/MacOS/Unity";
                case RuntimePlatform.LinuxEditor:
                    return EditorApplication.applicationPath;
                default:
                    throw new NotImplementedException("Platform not supported!");
            }
        }

        public static void ExcuteCmd(string prog, string args)
        {
            if (prog == null) return;
            
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                bool isCleaningUp = args.StartsWith("/c rd");
                startInfo.WindowStyle = isCleaningUp ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Maximized;

                startInfo.FileName = prog;
                startInfo.Arguments = args;

                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                process.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                CleanUpMenuItem.RemoveFromHub();
            }
        }
        #endregion
    }
} 