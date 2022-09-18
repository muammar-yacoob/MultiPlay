using UnityEngine;
namespace MultiPlay.Editor
{
    //[CreateAssetMenu(menuName = "MultiPlay/Settings")] //Uncomment to create one object to control the global settings.
    public class MultiPlaySettings : ScriptableObject
    {
        [Range(1, 30)]
        [Tooltip("Maximum number of clients")]
        public int maxNumberOfClients;

        [Tooltip("Default Project Clones Path")]
        public string clonesPath;

        [Tooltip("Enabeling this will increase the project size but will transfer project data like startup scene")]
        public bool copyLibrary;

        private void OnEnable()
        {
            maxNumberOfClients = 3;
            copyLibrary = true;

            if (string.IsNullOrEmpty(clonesPath))
                clonesPath = Application.persistentDataPath;
        }
    }
}