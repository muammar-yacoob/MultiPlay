using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MultiPlay.Demo
{
    public class Spinner : MonoBehaviour
    {
        [SerializeField] private Color[] colors; 
        private Texture2D[] textures;
        private Camera cam;
        private List<Image> icons;
        private int colorIndex;

        public void Spin()
        {
            icons ??= FindObjectsOfType<Image>().ToList();
            icons.ForEach(i => i.transform.Rotate(Vector3.forward, -45, Space.Self));
            
            colorIndex++;
            if (colorIndex >= 3)
                colorIndex = 0;

            icons.FirstOrDefault(i => i.name == "arrow")!.color = colors[colorIndex];
        }
    }
}