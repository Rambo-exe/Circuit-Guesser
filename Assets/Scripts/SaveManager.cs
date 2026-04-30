using UnityEngine;
using System.IO;

public static class SaveManager
{
    private static readonly string savePath = Application.persistentDataPath + "/save.json";

    public static SaveData data = new SaveData();

    public static void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            data = new SaveData();
            Save();
        }
    }

    public static void Save()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }
}
