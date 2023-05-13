using UnityEditor;
using UnityEngine;

namespace MultiPlay.Demo
{
    [CustomEditor(typeof(Spinner))]
    internal class SpinnerEditor : Editor
    {
        private Spinner spinner;
        private Camera cam;


        public override void OnInspectorGUI()
        {
            if(Application.isPlaying) return;
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