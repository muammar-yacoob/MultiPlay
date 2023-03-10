using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MultiPlay.Demo
{
    public class Spinner : MonoBehaviour
    {
        private List<Image> icons;

        public void Spin()
        {
            if (icons == null || icons.Count == 0)
            {
                icons = FindObjectsOfType<Image>().ToList();
            }

            icons.ForEach(i => i.transform.Rotate(Vector3.forward, -45, Space.Self));
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
    }
}