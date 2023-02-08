using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace MultiPlay.Demo
{
    public class Spawner : MonoBehaviour
    {
        private Texture2D[] textures;
        private Camera cam;

        private void Awake()
        {
            LoadTextures();
            cam = Camera.main;
        }

        private Texture2D[] LoadTextures() => Resources.LoadAll<Texture2D>("Textures");
        

        public void SpawnTextures()
        {
            textures ??= LoadTextures();
            cam ??= Camera.main;
            Random.InitState(DateTime.Now.Millisecond);
            
            Vector2 pos;
            foreach (var tex in textures)
            {
                var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                var mat = quad.GetComponent<MeshRenderer>().material =  new Material(Shader.Find("Transparent/Cutout/Soft Edge Unlit"));
                mat.SetTexture("_MainTex",tex);
                
                quad.transform.localScale = Vector3.one * 3;

                pos = GetPos(cam.orthographicSize - quad.transform.localScale.x/2);
                quad.transform.localPosition = pos;
                
                Debug.Log(tex.name);
            }
        }

        private Vector2 GetPos(float range)
        {
            Random.InitState(DateTime.Now.Millisecond);
            return Random.insideUnitCircle * range;
        }
    }
}
