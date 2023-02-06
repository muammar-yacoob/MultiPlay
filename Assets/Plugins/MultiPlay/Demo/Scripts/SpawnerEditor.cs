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

        private void Awake()
        {
            spawner ??= (Spawner)target;
            cam ??= Camera.main;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Shuffle"))
            {
                spawner.Shuffle(Random.insideUnitCircle);
            }
            //Repaint();
        }
    }
}