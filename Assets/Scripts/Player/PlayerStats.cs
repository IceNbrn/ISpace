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
        [SyncVar] public string KilledByPlayer;
        [SyncVar] public string KilledByWeapon;
        [SyncVar] public float CurrentHealth;

        public void ResetCurrentHealth() => CurrentHealth = Health;

        public void AddDeath(string killerName, string weaponName)
        {
            Deaths++;
            KilledByPlayer = killerName;
            KilledByWeapon = weaponName;
        }

        public void ResetKilledBy()
        {
            KilledByWeapon = String.Empty;
            KilledByPlayer = String.Empty;
        }
    }
}