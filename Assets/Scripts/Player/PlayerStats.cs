using System;
using Mirror;
using UnityEngine;

namespace Player
{
    public class PlayerStats : NetworkBehaviour
    {
        [SyncVar] public float Health = 100.0f;
        [SyncVar] public int Kills;
        [SyncVar] public int Deaths;
        [SyncVar] public float KillRatio;
        [SyncVar] public string KilledBy;
        [SyncVar] public float CurrentHealth;

        public void ResetCurrentHealth() => CurrentHealth = Health;

        public void AddDeath(string killerName)
        {
            Deaths++;
            KilledBy = killerName;
        }

        public void ResetKilledBy() => KilledBy = String.Empty;
    }
}