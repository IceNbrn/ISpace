using System;
using Mirror;
using UnityEngine;

namespace Player
{
    public class Stats
    {
        public int Kills;
        public int Assists;
        public int Deaths;
        public int Points;
        public float KillRatio;
    }
    public class PlayerStats : NetworkBehaviour
    {
        [SyncVar] public float Health = 100.0f;
        [SyncVar] public int Kills;
        [SyncVar] public int Assists;
        [SyncVar] public int Deaths;
        [SyncVar] public int Points;
        [SyncVar] public float KillRatio;
        [SyncVar] public string KilledByPlayer;
        [SyncVar] public string KilledByWeapon;
        [SyncVar] public float CurrentHealth;

        public void ResetCurrentHealth() => CurrentHealth = Health;

        public void AddKill() => Kills++;
        
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

        public Stats GetStats()
        {
            Stats stats = new Stats { Kills = Kills, Assists = Assists, Deaths = Deaths, Points = Points };
            return stats;
        }
    }
}