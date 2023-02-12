namespace MultiPlay
{
    internal static class Settings
    {
        //Settings: Hard coded
        public static MultiPlaySettings settingsAsset;
        public static readonly int MaxClonesLimit = 30;
        public const Licence productLicence = Licence.Full;

        //Settings Preferences
        public static int MaxClones;
        public static string clonesPath;
        public static bool linkLibrary;

        public static void SaveSettings()
        {
            settingsAsset.clonesPath = clonesPath;
            settingsAsset.maxNumberOfClones = MaxClones;
            settingsAsset.copyLibrary = linkLibrary;
        }

        public static void LoadSettings(MultiPlayEditor multiPlayEditor)
        {
            if (string.IsNullOrEmpty(clonesPath))
            {
                clonesPath = settingsAsset.clonesPath;
            }
            MaxClones = productLicence == Licence.Default? 1 : settingsAsset.maxNumberOfClones;
            linkLibrary = settingsAsset.copyLibrary;

            if (string.IsNullOrEmpty(clonesPath))
            {
                clonesPath = clonesPath.Replace(@"/", @"\");
            }
        }

        public enum Licence { Default, Full }

    }
}