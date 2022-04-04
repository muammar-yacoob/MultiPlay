using UnityEngine;


/// <summary>
/// General Settings that works for most games.
/// </summary>
[CreateAssetMenu(menuName = "MultiPlay/Settings")] //Uncomment to create one object to control the global settings.
//there's already one scriptable object asset provided and you don't actually need to create another one, just find it and change its variables
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