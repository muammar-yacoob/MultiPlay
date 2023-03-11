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
        private List<MeshRenderer> meshes;
        //[SerializeField] private List<Color> colors;
        private Camera cam;
        public void Spin()
        {
            if (meshes == null || meshes.Count == 0)
            {
                meshes = FindObjectsOfType<MeshRenderer>().ToList();
            }

            meshes.ForEach(i => i.transform.Rotate(Vector3.up, Random.Range(0,359), Space.Self));
            //meshes.ForEach(i => i.material.color =colors[Random.Range(0, colors.Count)]);
            
            cam ??= Camera.main;
            
            meshes.ForEach(i =>
            {
                Vector3 randomPos = new Vector3(Random.Range(cam.ViewportToWorldPoint(Vector3.zero).x, cam.ViewportToWorldPoint(Vector3.one).x),
                    Random.Range(cam.ViewportToWorldPoint(Vector3.zero).y, cam.ViewportToWorldPoint(Vector3.one).y),1);
                i.transform.position = randomPos;
            });
            
            
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
    }
}