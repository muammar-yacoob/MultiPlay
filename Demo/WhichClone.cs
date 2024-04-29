using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiPlay.Demo
{
    public class WhichClone : MonoBehaviour
    {
        void Start()
        {
#if !UNITY_EDITOR
            return;
#endif

            int cloneIndex = MultiPlay.Utils.GetCurrentCloneIndex();

            if (cloneIndex == 0) Debug.Log("MultiPlay is running on: Main Project/Server");
            else Debug.Log($"MultiPlay is running on Client: {cloneIndex}");
        }
    }
}