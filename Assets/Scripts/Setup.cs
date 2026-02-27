using System.IO;
using UnityEngine;
using UnityEditor;
using static System.IO.Path;

public static class Setup
{
    [MenuItem("Tools/Setup/Create Default Folders")]
    public static void CreateDefaultFolders()
    {
        Folders.CreateDefault("_Project", "Animation", "Art", "Materials", "Physical Materials", "Prefabs", "Loaded assets", "ScriptableObjects", "Scripts", "Settings"); 
        AssetDatabase.Refresh();
    }

    static class Folders
    {
        public static void CreateDefault(string root, params string[] folders)
        {
            var fullPath = Path.Combine(Application.dataPath, root);
            foreach (var folder in folders)
            {
                var path = Path.Combine(fullPath, folder);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
        }
    }

    [MenuItem("Tools/Setup/Import My Favourite Assets")]
    public static void ImportMyFavouriteAssets()
    {
        Assets.ImportAsset("DOTween HOTween v2.unitypackage", @"Demigiant\Editor ExtensionsAnimation");
    }

    [MenuItem("Tools/Setup/Import Basics")]
    static void ImportBasics()
    {
    }
    public static class Assets
    {
        public static void ImportAsset(string asset, string subfolder, string folder = @"C:\Users\helst\AppData\Roaming\Unity\Asset Store-5.x") 
        {
            AssetDatabase.ImportPackage(Combine(folder, subfolder, asset), false);
        }
    }
}
