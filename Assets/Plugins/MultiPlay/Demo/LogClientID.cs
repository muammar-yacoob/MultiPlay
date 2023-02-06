#if UNITY_EDITOR_WIN
using UnityEngine;

public class LogClientID : MonoBehaviour
{
    private void Start()
    {
        int clientIndex = 0;//MultiPlay.Utils.GetCurrentClientIndex();
        if (clientIndex == 0) Debug.Log("MultiPlay is running on: Main Project/Server");
        else Debug.Log($"MultiPlay is running on Client: {clientIndex}");
    }
}
#endif