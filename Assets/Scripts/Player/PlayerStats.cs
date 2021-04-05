using System;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class PlayerStats
    {
        public float Health = 100.0f;
        public int Kills, Deaths;
        public float KillRatio;
        public string KilledBy;
        public float CurrentHealth;

        public void ResetCurrentHealth() => CurrentHealth = Health;

        public void AddDeath(string killerName)
        {
            Deaths++;
            KilledBy = killerName;
        }

        public void ResetKilledBy() => KilledBy = String.Empty;
    }
}