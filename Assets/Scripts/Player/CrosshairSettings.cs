using System;
using TMPro;
using UnityEngine;

namespace Player
{
    [Serializable]
    public enum ECrosshairType
    {
        Cross = 1,
        T = 2,
        X = 3
    }
    [Serializable]
    public struct CrosshairSettings
    {
        public float Gap;
        public float Thickness ;
        public float Size;
        public ECrosshairType Type;
        public Color Color;
        
        public void Override(CrosshairSettings newSettings)
        {
            Gap = newSettings.Gap != 0 ? newSettings.Gap : Gap;
            Thickness = newSettings.Thickness != 0 ? newSettings.Thickness : Thickness;
            Size = newSettings.Size != 0 ? newSettings.Size : Size;
            Type = newSettings.Type != 0 ? newSettings.Type : Type;
            Color = !newSettings.Color.CompareRGB(new Color(0.0f, 0.0f, 0.0f)) ? newSettings.Color : Color;
        }
    }
}