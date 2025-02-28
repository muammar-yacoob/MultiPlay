using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace MultiPlay
{
    // Partial class handling UI drawing and interactions
    internal partial class MultiPlayEditor
    {
        #region Drawing Methods
        private void DrawHeader()
        {
            if (headerTexture == null || skin == null)
                InitializeTextures();

            headerRect = new Rect(((Screen.width - headerTexture.width * headerTexScale) / ppp) - (20), pad,
                headerTexture.width * headerTexScale, headerTexture.height * headerTexScale);
            GUI.DrawTexture(headerRect, headerTexture);
            pad /= ppp;

            GUILayout.BeginArea(fullRect);
            if (isClone)
            {
                headerStyle = IsLibraryLinked() ? linkStyle : nonLinkStyle;
            }
            GUILayout.Label(isClone ? cloneHeaderText : headerText, headerStyle);
            GUILayout.EndArea();
        }

        private void DrawBody()
        {
            bodyRect = new Rect(pad, headerRect.height + pad, Screen.width - pad * 2, Screen.height - headerRect.height - pad * 2);
            GUILayout.BeginArea(bodyRect);
            
            if (EditorApplication.isPlaying)
            {
                DrawPlayModeUI();
                return;
            }

            if (isClone)
            {
                DrawCloneUI();
            }
            else
            {
                DrawMainUI();
            }
            
            GUILayout.EndArea();
        }

        private void DrawPlayModeUI()
        {
            EditorGUILayout.HelpBox($"{cloneName}: Control panel is disabled while playing.", MessageType.Info);
            ShowNotification(new GUIContent($"Running on {cloneName}..."), 1);
            
            if (GUILayout.Button("More cool tools...", skin.GetStyle("PanStoreLink")))
            {
                Application.OpenURL($"https://assetstore.unity.com/publishers/" + myPubID);
                Application.OpenURL("https://panettonegames.com/");
            }
        }

        private void DrawCloneUI()
        {
            GUILayout.BeginVertical(GUILayout.Height((Screen.height - pad) / ppp), GUILayout.Width((Screen.width - pad * 2) / ppp));
            
            if (GUILayout.Button("Sync"))
            {
                hasChanged = false;
                lastSyncTime = DateTime.Now;
                ShowNotification(new GUIContent("Syncing..."));
                ReloadScene(SceneManager.GetActiveScene().path);
            }

            string autoSyncCaption = !IsLibraryLinked() ? "Auto Sync" : "Auto Sync unavailable in Link Library Mode";
            GUI.enabled = !Utils.IsLibraryLinked();
            autoSync = GUILayout.Toggle(!IsLibraryLinked() && autoSync, autoSyncCaption);
            GUI.enabled = true;

            if (hasChanged)
                EditorGUILayout.HelpBox("Changes from original build were detected. Make sure to Sync before running", MessageType.Warning);
            else
                EditorGUILayout.HelpBox($"You're Good to Go!\nLast Changed:\t{lastWriteTime}\nLast Synced:\t{lastSyncTime}", MessageType.Info);

            GUILayout.EndVertical();
        }

        private void DrawMainUI()
        {
            if (isCreatingReferences)
            {
                isCreatingReferences = false;
                ShowNotification(new GUIContent("Creating clone..."));
                return;
            }

            DrawCloneList();
            
            if (Settings.productLicence == Settings.Licence.Full)
            {
                DrawSettings();
            }
        }

        private void DrawCloneList()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), 
                GUILayout.Height(105 / ppp), GUILayout.Width((Screen.width - pad * 2) / ppp));
            
            for (int i = 1; i < Settings.MaxClones + 1; i++)
            {
                string destinationPath = $"{Settings.ClonesPath}/{cloneCaption}_[{i}]_Clone".Replace(@"/", @"\");
                var libPath = Path.Combine(destinationPath, "Library");
                var linkExists = Directory.Exists(libPath);
                
                var createLinkCaption = Settings.LinkLibrary ? "- Ω" : string.Empty;
                var openLinkCaption = linkExists && IsSymbolic(libPath) ? "- Ω" : string.Empty;

                string btnCaption = Directory.Exists(destinationPath) 
                    ? $"Launch clone {cloneCaption} [{i}] {openLinkCaption}" 
                    : $"Create clone {cloneCaption} [{i}] {createLinkCaption}";
                
                GUI.enabled = !Directory.Exists(destinationPath + "\\Temp");

                GUILayout.BeginHorizontal();
                
                if (Directory.Exists(destinationPath))
                {
                    GUI.contentColor = IsSymbolic(libPath) ? Color.yellow : Color.cyan;
                }

                if (GUILayout.Button(btnCaption, GUILayout.Height(buttonHeight)))
                {
                    HandleCloneButtonClick(destinationPath, i);
                }

                if (Directory.Exists(destinationPath))
                {
                    GUI.contentColor = Color.red;
                    if (GUILayout.Button("x", GUILayout.Height(buttonHeight), GUILayout.Width(35 / ppp)))
                    {
                        Debug.Log($"Deleting [{new DirectoryInfo(destinationPath).Name}]");
                        CleanUpMenuItem.ClearClone(destinationPath);
                    }
                }

                GUILayout.EndHorizontal();
                GUI.contentColor = defaultFontColor;
            }
            
            EditorGUILayout.EndScrollView();
        }

        private void DrawSettings()
        {
            EditorGUILayout.Space(5 / ppp);
            GUILayout.BeginVertical(GUILayout.Height(Screen.height - pad * 2), GUILayout.Width(Screen.width - pad * 2));
            
            Settings.LinkLibrary = GUILayout.Toggle(Settings.LinkLibrary, "Link Library");

            showSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showSettings, "Settings");
            if (showSettings)
            {
                Settings.MaxClones = EditorGUILayout.IntField(
                    new GUIContent("Max clones:", $"Maximum number of allowed clones is {Settings.MaxClonesLimit}"), 
                    Mathf.Clamp(Settings.MaxClones, 1, Settings.MaxClonesLimit));
                
                Settings.ClonesPath = EditorGUILayout.TextField(
                    new GUIContent("Clones Path:", "Default Path of project clones"), 
                    Settings.ClonesPath);
                
                if (GUILayout.Button("Browse", GUILayout.Height(buttonHeight), GUILayout.Width((Screen.width - pad * 2) / ppp)))
                {
                    BrowseForClonesPath();
                }

                string libraryTip = Settings.LinkLibrary 
                    ? "including Library link. i.e. faster but may break some 3rd party packages (recommended for most small projects)" 
                    : "excluding Library link. i.e. project configuration and packages will be stored separately at an extra disk cost. This option is safer for larger projects";
                
                var msgType = Settings.LinkLibrary ? MessageType.Warning : MessageType.Info;

                EditorGUILayout.HelpBox($"New clones will be created in [{new DirectoryInfo(Settings.ClonesPath).Name}] {libraryTip}.", msgType);
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.EndVertical();
        }

        private void BrowseForClonesPath()
        {
            string path = EditorUtility.OpenFolderPanel("Select Clones Folder", Settings.ClonesPath, "");
            if (path.Length != 0)
            {
                Settings.ClonesPath = path.Replace('/', '\\');
                Settings.SaveSettings();
                Repaint();
            }
        }

        private void InitializeTextures()
        {
            try
            {
                headerTexture = (Settings.productLicence == Settings.Licence.Full)
                    ? Resources.Load<Texture2D>("icons/MP_EditorHeader")
                    : Resources.Load<Texture2D>("icons/DP_EditorHeader");
                skin = Resources.Load<GUISkin>("guiStyles/Default");
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}");
            }
        }

        private static void RescaleUI()
        {
            ppp = EditorGUIUtility.pixelsPerPoint;
            buttonHeight /= ppp;
            headerTexScale /= ppp;
            windowMinWidth /= ppp;
            windowMinHeight /= ppp;
            windowMaxWidthExpanded /= ppp;
        }
        #endregion
    }
} 