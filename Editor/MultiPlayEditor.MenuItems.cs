using UnityEngine;
using UnityEditor;
using System;

namespace MultiPlay
{
    // Partial class handling menu items
    internal partial class MultiPlayEditor
    {
        #region Menu Items
        [MenuItem("Tools/" + licenseMenuCaption + "/Clone Manager &C", false, 10)]
        public static void OpenWindow()
        {
            try
            {
                string windowTitle = (Settings.productLicence == Settings.Licence.Full) ? "MultiPlay" : "DualPlay";
                
                window = window ?? GetWindow<MultiPlayEditor>(windowTitle, typeof(SceneView));
                
                window.titleContent = new GUIContent(windowTitle,
                    EditorGUIUtility.ObjectContent(CreateInstance<MultiPlayEditor>(), typeof(MultiPlayEditor)).image);
                
                window.minSize = new Vector2(windowMinWidth, windowMinHeight);
                
                if (!isClone)
                {
                    window.maxSize = new Vector2(windowMaxWidthExpanded, windowMaxWidthExpanded * 1.5f);
                }

                RescaleUI();
                window.Show();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        [MenuItem("Tools/" + licenseMenuCaption + "/Clean Up", false, 11)]
        static void Menu_Cleanup() => CleanUpMenuItem.CleanUpClones();

        [MenuItem("Tools/" + licenseMenuCaption + "/Clean Up", true, 11)]
        static bool Validate_Menu_Cleanup()
        {
            int cnt = Application.dataPath.Split('/').Length;
            string appFolderName = Application.dataPath.Split('/')[cnt - 2];
            return !Application.isPlaying && !isClone;
        }

        [MenuItem("Tools/" + licenseMenuCaption + "/Please \u2b50 Rate :) ", false, 30)]
        public static void MenuRate()
        {
            Application.OpenURL($"https://assetstore.unity.com/packages/tools/utilities/multiplay-170209?aid=1011lds77#reviews");
            Application.OpenURL($"https://github.com/muammar-yacoob/MultiPlay");
        }

        [MenuItem("Tools/" + licenseMenuCaption + "/Help", false, 30)]
        public static void MenuHelp()
        {
            Application.OpenURL("https://panettonegames.com/");
            string helpFilePath = Application.dataPath + @"/Plugins/PanettoneGames/MultiPlay/MultiPlay Read Me.pdf";
            Debug.Log($"Help file is in: {helpFilePath}");
            Application.OpenURL(helpFilePath);
            Application.OpenURL($"https://assetstore.unity.com/publishers/" + myPubID);
        }
        #endregion
    }
} 