using System;
using System.IO;
using UnityEngine;

// saving and loading jsons
public static class JsonManager
{
    static string BasePath => Application.persistentDataPath;

    public static bool Save<T>(string fileName, T data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            string path = Path.Combine(BasePath, fileName);
            File.WriteAllText(path, json);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {e}");
            return false;
        }
    }

    public static T Load<T>(string fileName) where T : class
    {
        try
        {
            string path = Path.Combine(BasePath, fileName);
            if (!File.Exists(path))
            {
                return null;
            }
            string json = File.ReadAllText(path);
            T data = JsonUtility.FromJson<T>(json);
            return data;
        }
        catch
        {
            return null;
        }
    }

    public static bool Delete(string fileName)
    {
        try
        {
            string path = Path.Combine(BasePath, fileName);
            if (File.Exists(path)) { File.Delete(path); return true; }
            return false;
        }
        catch
        {
            return false;
        }
    }

}

