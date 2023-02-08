using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MultiPlay.Demo
{
    [CustomEditor(typeof(Spawner))]
    public class SpawnerEditor : Editor
    {
        private Spawner spawner;
        private Camera cam;


        public override void OnInspectorGUI()
        {
            spawner ??= (Spawner)target;
            cam ??= Camera.main;
            
            if (GUILayout.Button("Spin"))
            {
                spawner.Spin();
            }
            DrawDefaultInspector();
            //Repaint();
        }
    }
}