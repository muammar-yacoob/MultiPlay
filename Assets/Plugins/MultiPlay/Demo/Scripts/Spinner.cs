using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MultiPlay.Demo
{
    [ExecuteInEditMode]
    public class Spinner : MonoBehaviour
    {
        [SerializeField] private Color[] colors; 
        private Texture2D[] textures;
        private Camera cam;
        private List<Image> icons;
        private int colorIndex;

        public void Spin()
        {
            if (icons == null || icons.Count == 0)
            {
                icons = FindObjectsOfType<Image>().ToList();
            }

            icons.ForEach(i => i.transform.Rotate(Vector3.forward, -45, Space.Self));
            
            colorIndex++;
            if (colorIndex >= 3)
                colorIndex = 0;

            icons.FirstOrDefault(i => i.name == "arrow")!.color = colors[colorIndex];
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
    }
}