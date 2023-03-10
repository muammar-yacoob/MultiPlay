    using UnityEngine;
    using UnityEditor;
    using System.IO;

namespace MultiPlay
{
    [InitializeOnLoad]
    public class SceneChangeDetector
    {
        private static FileSystemWatcher watcher;

        static SceneChangeDetector()
        {
            string projectPath = Directory.GetParent(Application.dataPath)?.FullName;

            watcher = new FileSystemWatcher(projectPath);
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            watcher.Changed += OnFileChanged;
            watcher.Created += OnFileChanged;
            watcher.Deleted += OnFileChanged;
            watcher.Renamed += OnFileRenamed;
        }

        private static void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.EndsWith(".unity"))
            {
                Debug.Log("Scene hierarchy changed");
            }
        }

        private static void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            if (e.FullPath.EndsWith(".unity"))
            {
                Debug.Log("Scene hierarchy changed");
            }
        }
    }

}