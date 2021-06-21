using System.Collections.Generic;
using Player;
using UnityEngine;

namespace SaveSystem
{
    public static class SaveDataManager
    {
        private static string FILE_NAME = "playerSettings.json";
        public static void SavePlayerJson(PlayerSettings playerSettings)
        {
            if (FileManager.WriteToFile(FILE_NAME, playerSettings.ToJson()))
            {
                Debug.Log("Save successful");
            }
        }
    
        public static PlayerSettings? LoadPlayerJson()
        {
            if (FileManager.LoadFromFile(FILE_NAME, out var json))
            {
                PlayerSettings playerSettings = JsonUtility.FromJson<PlayerSettings>(json);
                Debug.Log("Load complete");
                return playerSettings;
            }

            return null;
        }
    }
}