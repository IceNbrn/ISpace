using System;

namespace Player
{
    [Serializable]
    public struct PlayerSettings
    {
        public float Sensitivity;
        public CrosshairSettings CrosshairSettings;

        public Action OnCrosshairUpdated;
    }
}