/////////////////////////////////// 
/////////////////////////////////// 
//////////////////  All rights reserved By Muammar.Yacoob@Gmail.com
//////////////////  MultiPlay/Dual Play Unity Editor Extension 2020
//////////////////  Version 1.3.1
///#if Unity_Editor
/// - try catch blocks are added to identify nature of the error in the console log
//  - links args are /j but in case of faiur, try /d or else a simple xcopy for the failed folder
/////////////////////////////////// 
/////////////////////////////////// 
/////////////////////////////////// 

using Microsoft.Win32;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace PanettoneGames
{


    public class MultiPlay : UnityEditor.EditorWindow
    {
        #region privateMembers
        private string sourcePath;
        private static string destinationPath_1;
        private static string destinationPath_2;
        private static string destinationPath_3;
        private string destinationPath_default;
        private string btnCaption_1;
        private string btnCaption_2;
        private string btnCaption_3;
        private string btnCaption_default;
        private Texture2D bgTexture;
        private Texture2D headerTexture;
        private Texture2D bodyTexture;

        private Color bgColor;
        private Color headerColor;
        private Color bodyColor;

        private Rect fullRect;
        private Rect headerRect;
        private Rect bodyRect;

        private float headerTexScale = 0.20f;
        private GUISkin skin;
        private float pad = 5f;

        private bool isCreatingReferences;
        private bool hasChanged;
        private string sceneStatus;
        private static bool isClient;
        private float syncTimer;
        private int syncDelay = 1;
        private float nextSync;
        private string sceneFilePath;
        private DateTime lastSyncTime;
        private DateTime lastWriteTime;
        private bool autoSync;

        private Int32 clientIndex;
        private static Licence productLicence;
        private string headerText;
        private GUIStyle headerStyle;
        private string clientHeaderText;

        private static string myPubID = "46749";
        private float timer;

        #endregion

        #region menus

        //[MenuItem("Tools/DualPlay/Client Manager &C", false, 10)] //<<<<<<<<<<<<<<<<<<<   1
        [MenuItem("Tools/MultiPlay/Client Manager &C", false, 10)] //<<<<<<<<<<<<<<<<<<<

        public static void OpenWindow()
        {
            try
            {
                productLicence = Licence.Full; //<<<<<<<<<<<<<<<<<<<   2
                string windowTitle = (productLicence == Licence.Full) ? "MultiPlay" : "DualPlay";
                MultiPlay window;
                if (isClient)
                {
                    window = GetWindow<MultiPlay>(windowTitle, typeof(SceneView));
                    window.minSize = new Vector2(180, 100);
                }
                else
                {
                    window = GetWindow<MultiPlay>(windowTitle, typeof(SceneView));
                    window.minSize = new Vector2(180, 165);
                    window.maxSize = new Vector2(180, 165);
                }

                window.Show();
            }
            catch (Exception) { }
        }


        //[MenuItem("Tools/DualPlay/Clean Up", false, 11)]//<<<<<<<<<<<<<<<<<<<    3
        [MenuItem("Tools/MultiPlay/Clean Up", false, 11)]//<<<<<<<<<<<<<<<<<<<    
        static void Menu_Cleanup()
        {
            string msg = "Clearing cached references to all clients, are you sure you want to proceed?";


            if (!DoLinksExist())
                msg = "No Clients found, Do you want to try clear references anyway?";

            if (DoLinksLive())
            {
                Debug.Log("WARNING: Live Clients were detected! You Should close them before clearing references; Otherwise, Unity may crash!");
                msg = "WARNING!! Make sure ALL CLIENTS are CLOSED before proceeding!!";
            }

            if (EditorUtility.DisplayDialog("Clearing References", msg, "Proceed", "Cancel"))
            {
                try
                {
                    Debug.Log("Cleaning cache...");
                    PurgeAllClients();
                    Debug.Log("MultiPlay: References cleared successfully");
                    RemoveFromHub();
                    EditorUtility.DisplayDialog("Success", "All Clear!", "OK");
                }
                catch (Exception e) { Debug.LogError(e.Message); }
            }



        }


        //[MenuItem("Tools/DualPlay/Clean Up", true, 11)]//<<<<<<<<<<<<<<<<<<<    4
        [MenuItem("Tools/MultiPlay/Clean Up", true, 11)]//<<<<<<<<<<<<<<<<<<<    

        static bool Validate_Menu_Cleanup()
        {
            int cnt = Application.dataPath.Split('/').Length;
            string appFolderName = Application.dataPath.Split('/')[cnt - 2];
            isClient = appFolderName.EndsWith("___Client");

            return !Application.isPlaying && !isClient;
        }




        //[MenuItem("Tools/DualPlay/Help", false, 30)] //<<<<<<<<<<<<<<<<<<<      5  
        [MenuItem("Tools/MultiPlay/Help", false, 30)] //<<<<<<<<<<<<<<<<<<<        
        public static void MenuHelp()
        {
            Application.OpenURL(@"https://panettonegames.com/");

            string helpFilePath = Application.dataPath + @"/Plugins/PanettoneGames/MultiPlay/MultiPlay Read Me.pdf";
            Debug.Log($"Help file is in: {helpFilePath}");
            Application.OpenURL(helpFilePath);
            Application.OpenURL($"https://assetstore.unity.com/publishers/" + myPubID);
        }



        #endregion

        private void OnEnable()
        {
            try
            {
                productLicence = Licence.Full; //<<<<<<<<<<<<<<<<<<< 6 Last

                sceneFilePath = Application.dataPath + "/../" + SceneManager.GetActiveScene().path;
                sceneFilePath = sceneFilePath.Replace(@"/", @"\");

                isCreatingReferences = false;

                // headerColor = new Color(0, 0, 0);


                sourcePath = $"{ Application.dataPath }/..";
                sourcePath = sourcePath.Replace(@"/", @"\");

                //destinationPath = $"{ Application.dataPath }/../../{Application.productName}Client";
                //Debug.Log(destinationPath);


                //MultiPlay Clients
                destinationPath_1 = $"{ Application.persistentDataPath }/../../{Application.productName}_[1]___Client".Replace(@"/", @"\");
                destinationPath_2 = $"{ Application.persistentDataPath }/../../{Application.productName}_[2]___Client".Replace(@"/", @"\");
                destinationPath_3 = $"{ Application.persistentDataPath }/../../{Application.productName}_[3]___Client".Replace(@"/", @"\");
                destinationPath_default = $"{ Application.persistentDataPath }/../../{Application.productName}___Client".Replace(@"/", @"\");

                btnCaption_1 = Directory.Exists(destinationPath_1) ? $"Launch Client [1]" : $"Create Client [1]";
                btnCaption_2 = Directory.Exists(destinationPath_2) ? $"Launch Client [2]" : $"Create Client [2]";
                btnCaption_3 = Directory.Exists(destinationPath_3) ? $"Launch Client [3]" : $"Create Client [3]";
                btnCaption_default = Directory.Exists(destinationPath_default) ? $"Launch Client" : $"Create Client";

                headerText = (productLicence == Licence.Full) ? "MultiPlay" : "DualPlay";
                headerStyle = (productLicence == Licence.Full) ? skin.GetStyle("PanHeaderFull") : skin.GetStyle("PanHeaderDefault");

                //bgColor = (productLicence == Licence.Full) ? new Color(1f, 1f, 1f) : new Color(0.2f, 0.2f, 0.2f);
                //bodyColor = (productLicence == Licence.Full) ? new Color(1f, 0.6f, 0.3f) : new Color(0.5f, 0.75f, 1);



                int cnt = Application.dataPath.Split('/').Length;
                string appFolderName = Application.dataPath.Split('/')[cnt - 2];
                isClient = appFolderName.EndsWith("___Client");
                if (isClient)
                {
                    bool indexHasParsed = Int32.TryParse(appFolderName.Substring(appFolderName.IndexOf('[') + 1, 1), out clientIndex);
                }

                clientHeaderText = (productLicence == Licence.Full) ? $"Client [{clientIndex}]" : $"Client";

                //reset status
                hasChanged = false;
                lastSyncTime = DateTime.Now;
                syncDelay = clientIndex * 1000;
                //Debug.Log("sync Delay at " + syncDelay);
                nextSync = Time.realtimeSinceStartup + syncDelay;

                InitializeTextures();
                RemoveFromHub();
                //Debug.Log($"lastWrite: {lastWriteTime}, lastSync: {lastSyncTime}");
                EditorApplication.update += OnEditorUpdate;
            }
            catch (Exception e) { Debug.LogError($"{e.Message}"); }

        }
        protected virtual void OnDisable() => EditorApplication.update -= OnEditorUpdate;

        private void Awake()
        {
            InitializeTextures();

        }

        private void OnEditorUpdate()
        {
            if (isClient) CheckIfSceneChanged();
        }

        private void CheckIfSceneChanged()
        {
            try
            {
                lastWriteTime = File.GetLastWriteTime(sceneFilePath);
                if (lastWriteTime > lastSyncTime)
                {
                    if (autoSync)
                    {
                        if (clientIndex > 1) System.Threading.Thread.Sleep(clientIndex * 600); //<<<<<<<<<<<<<<<<<<<<<<<<<<<< induce some delay here
                        ReloadScene();
                    }
                    else hasChanged = true;
                    //Debug.LogWarning($"Changes made on {lastWriteTime}. Make sure to Sync before running the game");
                    Repaint();
                }
            }
            catch (Exception e) { Debug.LogError($"{e.Message}"); }
        }

        /*
     private void Update() //unnecessary for updates but i'm trying to use it to induce delay between clients scene upload
     {
         if (isClient)
         {
             timer += Time.deltaTime;
             if (timer >= clientIndex)
             {
                 timer = 0;
                 Debug.Log("Time to refresh");
             }
         }
     }
     private bool TimeToCheck() => ((Time.realtimeSinceStartup > nextSync));
     */

        private void InitializeTextures()
        {
            try
            {
                headerTexture = (productLicence == Licence.Full) ? Resources.Load<Texture2D>("icons/MP_EditorHeader") : Resources.Load<Texture2D>("icons/DP_EditorHeader");
                skin = Resources.Load<GUISkin>("guiStyles/Default");

                bgTexture = new Texture2D(1, 1);
                bgTexture.SetPixel(0, 0, bgColor);
                bgTexture.Apply();

                bodyTexture = new Texture2D(1, 1);
                bodyTexture.SetPixel(0, 0, bodyColor);
                bodyTexture.Apply();
            }
            catch (Exception e) { Debug.LogError($"{e.Message}"); }
        }
        private void OnGUI()
        {
            DrawLayout();
        }


        private void DrawLayout()
        {
            #region formatting

            btnCaption_1 = Directory.Exists(destinationPath_1) ? $"Launch Client [1]" : $"Create Client [1]";
            btnCaption_2 = Directory.Exists(destinationPath_2) ? $"Launch Client [2]" : $"Create Client [2]";
            btnCaption_3 = Directory.Exists(destinationPath_3) ? $"Launch Client [3]" : $"Create Client [3]";
            btnCaption_default = Directory.Exists(destinationPath_default) ? $"Launch Client" : $"Create Client";

            //bg
            if (bgTexture == null || headerTexture == null || skin == null)
                InitializeTextures();

            fullRect = new Rect(0, 0, Screen.width, Screen.height);
            GUI.DrawTexture(fullRect, bgTexture);

            //Header
            headerRect = new Rect(Screen.width - headerTexture.width * headerTexScale, 0, headerTexture.width * headerTexScale, headerTexture.height * headerTexScale);
            GUI.DrawTexture(headerRect, headerTexture);

            //Body
            bodyRect = new Rect(pad, headerRect.height + pad, Screen.width - pad * 2, Screen.height - headerRect.height - pad * 2);
            GUI.DrawTexture(bodyRect, bodyTexture);

            GUILayout.BeginArea(fullRect);



            if (isClient)

                GUILayout.Label(clientHeaderText, headerStyle);
            else
                GUILayout.Label(headerText, headerStyle);
            GUILayout.EndArea();


            GUILayout.BeginArea(bodyRect);
            GUILayout.Space(10);

            #endregion
            GUILayout.BeginVertical();


            if (isClient)
            {
                if (GUILayout.Button("Sync"))
                {
                    hasChanged = false;
                    lastSyncTime = DateTime.Now;
                    ShowNotification(new GUIContent("Syncing..."));
                    ReloadScene();
                }
            }

            else //Original Copy
            {
                if (isCreatingReferences)
                {
                    isCreatingReferences = false;
                    ShowNotification(new GUIContent("Creating Client..."));
                }
                else //Create References Or Launch

                {
                    if (productLicence == Licence.Full)
                    {
                        #region Full Version
                        //Client [1]
                        GUI.enabled = !Directory.Exists(destinationPath_1 + "\\Temp");


                        if (GUILayout.Button(btnCaption_1, GUILayout.Height(25)))
                        {
                            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                            if (!Directory.Exists(destinationPath_1))
                            {
                                isCreatingReferences = false;
                                CreateLink(destinationPath_1, "Assets");
                                CreateLink(destinationPath_1, "ProjectSettings");
                                CreateLink(destinationPath_1, "Packages");
                                //CreateLink(destinationPath_1, "Library"); //kills auto sync.
                            }


                            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                            hasChanged = false;
                            LaunchClient(destinationPath_1);
                            RemoveFromHub();
                            //Close();
                        }
                        GUI.enabled = true;


                        //Client [2]
                        GUI.enabled = !Directory.Exists(destinationPath_2 + "\\Temp");
                        if (GUILayout.Button(btnCaption_2, GUILayout.Height(25)))
                        {
                            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                            if (!Directory.Exists(destinationPath_2))
                            {
                                isCreatingReferences = false;
                                CreateLink(destinationPath_2, "Assets");
                                CreateLink(destinationPath_2, "ProjectSettings");
                                CreateLink(destinationPath_2, "Packages");
                                //CreateLink(destinationPath_2, "Library"); //kills auto sync.
                            }


                            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                            hasChanged = false;
                            LaunchClient(destinationPath_2);
                            RemoveFromHub();
                            //Close();
                        }
                        GUI.enabled = true;

                        //Client [3]
                        GUI.enabled = !Directory.Exists(destinationPath_3 + "\\Temp");
                        if (GUILayout.Button(btnCaption_3, GUILayout.Height(25)))
                        {
                            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                            if (!Directory.Exists(destinationPath_3))
                            {
                                isCreatingReferences = false;
                                CreateLink(destinationPath_3, "Assets");
                                CreateLink(destinationPath_3, "ProjectSettings");
                                CreateLink(destinationPath_3, "Packages");
                                //CopyStartupConfig(destinationPath_3);
                                //CreateLink(destinationPath_3, "Library"); //kills auto sync.
                            }


                            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                            hasChanged = false;
                            LaunchClient(destinationPath_3);
                            RemoveFromHub();
                            //Close();
                        }
                        GUI.enabled = true;

                        #endregion
                    }
                    else
                    {
                        #region Default Version
                        //Client Default
                        GUI.enabled = !Directory.Exists(destinationPath_default + "\\Temp");
                        if (GUILayout.Button(btnCaption_default, GUILayout.Height(25)))
                        {
                            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                            if (!Directory.Exists(destinationPath_default))
                            {
                                isCreatingReferences = false;
                                CreateLink(destinationPath_default, "Assets");
                                CreateLink(destinationPath_default, "ProjectSettings");
                                CreateLink(destinationPath_default, "Packages");
                                //CreateLink(destinationPath_default, "Library"); //kills auto sync.
                            }


                            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                            hasChanged = false;
                            LaunchClient(destinationPath_default);
                            RemoveFromHub();
                            //Close();
                        }
                        GUI.enabled = true;
                        #endregion
                    }
                }


            }



            GUILayout.EndVertical();
            if (isClient) autoSync = GUILayout.Toggle(autoSync, "Auto Sync");

            //if (GUILayout.Button("x")) RemoveFromHub();

            if (isClient)
            {
                if (hasChanged) EditorGUILayout.HelpBox("Changes from original build were detected. Make sure to Sync before running", MessageType.Warning);
                else EditorGUILayout.HelpBox($"You're Good to Go!\nLast Changed:\t{lastWriteTime}\nLast Synced:\t{lastSyncTime}", MessageType.Info);
            }



            if (productLicence == Licence.Default)
            {
                if (GUILayout.Button("More cool tools...", skin.GetStyle("PanStoreLink")))
                {
                    Application.OpenURL($"https://assetstore.unity.com/publishers/" + myPubID);
                    Application.OpenURL($"https://panettonegames.com/");
                }
            }

            GUILayout.EndArea();
        }

        private static bool DoLinksExist()
        {

            if (productLicence == Licence.Full)
            {
                return
                    Directory.Exists(destinationPath_1) ||
                    Directory.Exists(destinationPath_2) ||
                    Directory.Exists(destinationPath_3);

            }
            else //Dual Play
            {
                return
                    Directory.Exists(destinationPath_1);
            }
        }

        private static bool DoLinksLive()
        {
            if (productLicence == Licence.Full)
            {
                return
                    Directory.Exists(destinationPath_1 + "\\Temp") ||
                    Directory.Exists(destinationPath_2 + "\\Temp") ||
                    Directory.Exists(destinationPath_3 + "\\Temp");
            }
            else  //Dual Play
            {
                return
                    Directory.Exists(destinationPath_1 + "\\Temp");

            }
        }
        private static void ReloadScene()
        {
            try
            {

                EditorSceneManager.OpenScene(SceneManager.GetActiveScene().path, OpenSceneMode.Single);
            }
            catch (Exception ex)
            { EditorUtility.DisplayDialog("Error Loading Scene", ex.Message, "Sync Manually"); }
        }

        private static void RemoveFromHub()
        {
            try
            {
                string kFound = string.Empty;
                string keyName = @"Software\Unity Technologies\Unity Editor 5.x";
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true))
                {
                    if (key == null)
                    {
                        //Debug.Log("Editor version not found");
                    }
                    else
                    {
                        string[] kvals = key.GetValueNames();
                        foreach (string k in kvals)
                        {
                            //if (k.Contains("RecentlyUsedProjectPaths-0"))
                            if (k.Contains("RecentlyUsedProjectPaths-"))
                            {


                                if (key.GetValueKind(k) == RegistryValueKind.Binary)
                                {
                                    var value = (byte[])key.GetValue(k);
                                    var valueAsString = Encoding.ASCII.GetString(value);

                                    if (valueAsString.EndsWith("Client"))
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
            }
            catch (Exception e) { Debug.LogError($"Unable to clear system cache due to unsufficient User Priviliges. Please contact your system administrator. \nDetails: {e.Message}"); }

        }

        private void CopyStartupConfig(string destPath)
        {
            try
            {
                string startupFile = "LastSceneManagerSetup.txt";
                string args = $"/c copy /y {sourcePath}\\Library\\{startupFile} {startupFile}\\Library\\{startupFile}";
                var thread = new Thread(delegate () { ExcuteCMD("cmd", args); });
                thread.Start();
            }
            catch (Exception exx)
            {
                Debug.LogError($"Links failed. You do not have sufficient previliges to write to windows temporary files. Please contact your system administrator\n{exx.Message}");

            }
        }
        private void CreateLink(string destPath, string subDirectory)
        {

            if (!Directory.Exists(destPath))
                Directory.CreateDirectory(destPath);

            string args = String.Empty;
            try
            {
                args = $"/c mklink /j \"{destPath}\\{subDirectory}\" \"{sourcePath}\\{subDirectory}\"";
                var thread = new Thread(delegate () { ExcuteCMD("cmd", args); });
                thread.Start();
            }
            catch (Exception e)
            {

                Debug.LogWarning($"Could not link {subDirectory}, trying again...\n{e.Message}");
                try
                {
                    args = $"/c mklink /d {destPath}\\{subDirectory} {sourcePath}\\{subDirectory}";
                    var thread = new Thread(delegate () { ExcuteCMD("cmd", args); });
                    thread.Start();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Attempt 2 failed.. attempting one last time\n{ex.Message}");

                    try
                    {
                        args = $"/c xcopy /s /y {sourcePath}\\{subDirectory} {destPath}\\{subDirectory}";
                        var thread = new Thread(delegate () { ExcuteCMD("cmd", args); });
                        thread.Start();
                    }
                    catch (Exception exx)
                    {
                        Debug.LogError($"Links failed. You do not have sufficient previliges to write to windows temporary files. Please contact your system administrator\n{exx.Message}");

                    }
                    //this.Close();
                }

            }
        }

        private void LaunchClient(string destPath)
        {
            try
            {
                string currentUnityVersion = Application.unityVersion;
                string editorPath = EditorApplication.applicationPath;
                //string editorArgs = $" -disable-assembly-updater -silent-crashes -noUpm"; //-nographics
                string editorArgs = String.Empty; //$" -disable-assembly-updater -silent-crashes";
                string projectPath = $" -projectPath \"{destPath}\"";

                var thread = new Thread(delegate () { ExcuteCMD($"\"{editorPath}\"", editorArgs + projectPath); });
                //Debug.Log();

                thread.Start();
                //RemoveFromHub();
            }
            catch (Exception e) { Debug.LogError($"Unable to read temporary files due to unsufficient User Priviliges. Please contact your system administrator. \nDetails: {e.Message}"); }

        }

        private static void ClearClient(string destPath)
        {

            if (!Directory.Exists(destPath))
                return;

            Thread thread = null;
            try
            {

                string args = $"/c rd /s /q \"{destPath}\"";
                thread = new Thread(delegate () { ExcuteCMD("cmd", args); });
                thread?.Start();
            }
            catch (Exception e) { Debug.LogError($"Error resetting clients\n{e.Message}"); }
        }

        private static void PurgeAllClients()
        {
            try
            {
                var tmpPath = new DirectoryInfo( //Path.GetTempPath());
                $"{ Application.persistentDataPath }/../../");

                foreach (var d in tmpPath.EnumerateDirectories("*Client*"))
                {
                    //Debug.Log(d.FullName);
                    ClearClient(d.FullName);

                    //if all failed, run this from windows command prompt
                    //for / d % x in (% tmp %\*Client) do rd / s / q " % x"

                }
            }
            catch (Exception e) { Debug.LogError($"Error resetting clients\n{e.Message}"); }
        }



        static void ExcuteCMD(string prog, string args)
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Maximized;
                startInfo.FileName = prog;
                startInfo.Arguments = args;
                string tmp = prog + args;
                //Debug.LogError(tmp);
                process.StartInfo = startInfo;
                process.Start();

                process.WaitForExit();

                process.Close();
            }
            catch (Exception e) { Debug.LogError($"Error excuting command\n{prog} {args}\n{e.Message}"); }
            finally { RemoveFromHub(); }

        }

        private IEnumerator Sleep(float timer)
        { yield return new WaitForSeconds(timer); }

        private enum Licence { Default, Full }

    }


}

