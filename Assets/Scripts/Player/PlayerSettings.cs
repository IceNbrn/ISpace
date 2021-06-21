using System;
using Player;
using UnityEngine;

namespace Player
{
    [Serializable]
    public struct PlayerSettings
    {
        public float Sensitivity;
        public CrosshairSettings CrosshairSettings;

        public Action OnSettingsUpdated;
        public Action OnCrosshairUpdated;

        
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}

public interface ISaveable
{
    void PopulatePlayerSettings(PlayerSettings settings);
    void LoadFromPlayerSettings(PlayerSettings settings);
}