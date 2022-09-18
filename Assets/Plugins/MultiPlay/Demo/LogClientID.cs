using UnityEngine;
//using MultiPlay.Utils;

public class LogClientID : MonoBehaviour
{
    private void Start()
    {
        int clientIndex = 0;//GetCurrentClientIndex();
        if (clientIndex == 0) Debug.Log("MultiPlay is running on: Main Project/Server");
        else Debug.Log($"MultiPlay is running on Client: {clientIndex}");
    }
}