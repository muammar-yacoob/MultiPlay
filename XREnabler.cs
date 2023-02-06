#if UNITY_EDITOR
//In Unity: [Edit]->[Project Settings] -> [XR Plug-in Management] -> Select the desktop tab -> Uncheck "Initialize XR on Startup"
namespace Born.Aviation
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.XR.Management;
 
    public class XREnabler : MonoBehaviour
    {
        void Awake()
        {
            if (MultiPlay.Utils.IsClient)
            {
                StartCoroutine(StopXR());
            }
            else
            {
                StartCoroutine(StartXR());
            }
        }
 
        public IEnumerator StopXR()
        {
            Debug.Log("Stopping XR...");
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            Debug.Log("XR Stopped.");
            yield return null;
        }
 
 
        public IEnumerator StartXR()
        {
            Debug.Log("Initializing XR...");
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
 
            if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                Debug.LogError("Initializing XR Failed. Check Editor or Player log for details.");
            }
            else
            {
                Debug.Log("Starting XR...");
                XRGeneralSettings.Instance.Manager.StartSubsystems();
            }
        }
    }
}
#endif
