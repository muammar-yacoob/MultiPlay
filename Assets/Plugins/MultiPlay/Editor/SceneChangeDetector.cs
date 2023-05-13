using System;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace MultiPlay
{
    [InitializeOnLoad]
    public class SceneChangeDetector
    {
        private static FileSystemWatcher watcher;
        private static string lastSceneChanged = null;
        private static DateTime lastEventTime = DateTime.MinValue;

        public static event Action<string> SceneChanged;

        static SceneChangeDetector()
        {
            string projectPath = Directory.GetParent(Application.dataPath)?.FullName;

            watcher = new FileSystemWatcher(projectPath, "*.unity");
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            watcher.Changed += OnFileChanged;
            watcher.Created += OnFileChanged;
            watcher.Deleted += OnFileChanged;
            watcher.Renamed += OnFileRenamed;
        }

        private static void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (lastSceneChanged != e.Name || (DateTime.Now - lastEventTime).TotalSeconds > 1)
            {
                lastSceneChanged = e.Name;
                lastEventTime = DateTime.Now;

                EditorApplication.update += Update;
            }
        }

        private static void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            if (lastSceneChanged != e.Name || (DateTime.Now - lastEventTime).TotalSeconds > 1)
            {
                lastSceneChanged = e.Name;
                lastEventTime = DateTime.Now;

                EditorApplication.update += Update;
            }
        }

        private static void Update()
        {
            SceneChanged?.Invoke(lastSceneChanged);
            EditorApplication.update -= Update;
        }
    }
}