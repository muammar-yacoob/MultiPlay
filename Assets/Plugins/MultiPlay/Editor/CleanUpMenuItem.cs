using System;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using UnityEditor;
using UnityEngine;

namespace MultiPlay
{
    internal class CleanUpMenuItem
    {
        public static void CleanUpClones()
        {
            int clonesFound = MultiPlayEditor.DoLinksExist();

            string msg = $"Clearing cached references to {clonesFound} clones, are you sure you want to proceed?";


            if (clonesFound == 0)
                msg = $"No clones were found in {MultiPlayEditor.clonesPath}, Try clear references anyway?";

            if (MultiPlayEditor.DoLinksLive())
            {
                Debug.LogWarning(
                    "WARNING: Live clones were detected! You Should close them before clearing references; Otherwise, Unity may crash!");
                msg = "WARNING!! Make sure ALL cloneS are CLOSED before proceeding!!";
            }

            if (EditorUtility.DisplayDialog("Clearing References", msg, "Proceed", "Cancel"))
            {
                try
                {
                    Debug.Log("Cleaning cache...");
                    EditorUtility.DisplayProgressBar("Processing..", "Shows a progress", 0.9f);
                    PurgeAllClones();
                    EditorUtility.ClearProgressBar();
                    Debug.Log("MultiPlay: References cleared successfully");
                    RemoveFromHub();
                    // ReSharper disable once Unity.NoNullPropagation
                    MultiPlayEditor.window?.Repaint();
                    EditorUtility.DisplayDialog("Success", "All Clear!", "OK");
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        public static void RemoveFromHub()
        {
            try
            {
                string keyName = @"Software\Unity Technologies\Unity Editor 5.x";
                using RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true);
                if (key == null)
                {
                    //Debug.Log("Editor version not found");
                }
                else
                {
                    string[] values = key.GetValueNames();
                    foreach (string k in values)
                    {
                        //if (k.Contains("RecentlyUsedProjectPaths-0"))
                        if (k.Contains("RecentlyUsedProjectPaths-"))
                        {


                            if (key.GetValueKind(k) == RegistryValueKind.Binary)
                            {
                                var value = (byte[])key.GetValue(k);
                                var valueAsString = Encoding.ASCII.GetString(value);

                                if (valueAsString.EndsWith("clone"))
                                {
                                    //Debug.Log($"key deleted: {k} with value {valueAsString}");
                                    key.DeleteValue(k);
                                    //kFound = k;
                                    //break;
                                }

                            }
                        }
                    }
                    // Debug.Log($"{kFound} deleted");
                }
            }
            catch (Exception e) { Debug.LogError($"Unable to clear system cache due to insufficient User Privileges. Please contact your system administrator. \nDetails: {e.Message}"); }
        }

        public static void ClearClone(string destPath)
        {
            if (!Directory.Exists(destPath))
                return;

            try
            {
                string args = $"/c rd /s /q \"{destPath}\"";
                var thread = new Thread(delegate() { MultiPlayEditor.ExcuteCMD("cmd", args); });
                thread.Start();
            }
            catch (Exception e) { Debug.LogError($"Error resetting clones\n{e.Message}"); }
        }

        private static void PurgeAllClones()
        {
            try
            {
                var tmpPath = new DirectoryInfo( //Path.GetTempPath());
                    $"{ MultiPlayEditor.clonesPath }");

                foreach (var d in tmpPath.EnumerateDirectories("*clone*"))
                {
                    //Debug.Log(d.FullName);
                    ClearClone(d.FullName);

                    //if all failed, run this from windows command prompt
                    //for / d % x in (% tmp %\*clone) do rd / s / q " % x"

                }
            }
            catch (Exception e) { Debug.LogError($"Error resetting clones\n{e.Message}"); }
        }
    }
}