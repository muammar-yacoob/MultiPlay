using UnityEditor;
using UnityEngine;

namespace MultiPlay.Demo
{
    [CustomEditor(typeof(Spinner))]
    internal class SpawnerEditor : Editor
    {
        private Spinner spinner;
        private Camera cam;


        public override void OnInspectorGUI()
        {
            spinner ??= (Spinner)target;
            cam ??= Camera.main;
            
            if (GUILayout.Button("Spin"))
            {
                spinner.Spin();
            }
            DrawDefaultInspector();
            //Repaint();
        }
    }
}