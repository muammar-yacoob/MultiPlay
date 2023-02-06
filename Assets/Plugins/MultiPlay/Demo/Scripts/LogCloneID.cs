using UnityEngine;
using UnityEngine.Diagnostics;
using MultiPlay;

namespace MultiPlay.Demo
{
    public class LogCloneID : MonoBehaviour
    {
#if UNITY_EDITOR_WIN
        private void Start()
        {
            int cloneIndex = 0;//MultiPlay.Demo.
            if (cloneIndex == 0) Debug.Log("MultiPlay is running on: Main Editor");
            else Debug.Log($"MultiPlay is running on Clone: {cloneIndex}");
        }
#endif
    }
}
