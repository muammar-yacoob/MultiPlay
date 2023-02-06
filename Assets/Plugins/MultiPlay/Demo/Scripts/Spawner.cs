using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MultiPlay.Demo
{
    public class Spawner : MonoBehaviour
    {
        private Sprite[] Sprite;
        private void Awake()
        {
            Sprite = (Sprite[])Resources.LoadAll("Sprites", typeof(Sprite));
            
            foreach (var t in Sprite)
            {
                Debug.Log(t.name);
                
                // GameObject go = new GameObject("New Sprite");
                // SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
                // renderer.sprite = t;
            }
        }
        public void Shuffle(Vector3 pos)
        {
            Sprite ??= (Sprite[])Resources.LoadAll("Sprites", typeof(Sprite));
            foreach (var t in Sprite)
                Debug.Log(t.name);
            
            Debug.Log(pos.ToString());
        }
    }
}
