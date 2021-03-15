using System;
using System.Collections;
using Mirror;
using UnityEngine;
using Weapons;

namespace Player
{
    enum EPlayerStatus
    {
        ALIVE,
        DEAD,
        SPECTATING
    }
    
    public class SpacePlayer : NetworkBehaviour
    {
        // ----------------  Stats  ----------------  
        [Header("Stats")]
        [SerializeField]
        private float health = 100.0f;

        private float _currentHealth;

        // ---------------- Respawn ----------------  
        [Header("Respawn")]
        [SerializeField]
        private bool canRespawn;
        
        [SerializeField] 
        private float timeToRespawn = 3.0f;
        
        private Vector3 _respawnPoint;
        private bool _respawning;
        
        // ---------------- Systems ---------------- 
        [Header("Systems")]
        [SerializeField] private Weapon weapon;

        // ---------------- Visuals ---------------- 
        [Header("Visuals")] 
        [SerializeField] private GameObject bodyModel;
        [SerializeField] private GameObject weaponModel;
        
        [ClientRpc]
        public void RpcTakeDamage(float damage, string fromPlayer)
        {
            Debug.Log($"Taking Damage: {damage} from {fromPlayer}");
            _currentHealth -= damage;

            if (_currentHealth <= 0.0f)
            {
                if (canRespawn && !_respawning)
                {
                    StartCoroutine(RespawnCoroutine());
                }
                return;
            }
            return;
        }
        
        public void SetRespawnPoint(Vector3 position) => _respawnPoint = position;
        
        public override void OnStartClient()
        {
            base.OnStartClient();

            string netId = GetComponent<NetworkIdentity>().netId.ToString();
            SpacePlayer player = GetComponent<SpacePlayer>();

            GameManager.AddPlayer(netId, player);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            
            string netId = GetComponent<NetworkIdentity>().netId.ToString();
            SpacePlayer player = GetComponent<SpacePlayer>();

            GameManager.RemovePlayer(netId);
        }

        private IEnumerator RespawnCoroutine()
        {
            Debug.Log("Respawning...");
            _respawning = true;
            
            SetPlayerStatus(EPlayerStatus.DEAD);
            yield return new WaitForSeconds(timeToRespawn);
            SetPlayerStatus(EPlayerStatus.ALIVE);

            _currentHealth = health;
            _respawning = false;
        }

        private void SetPlayerStatus(EPlayerStatus status)
        {
            switch (status)
            {
                case EPlayerStatus.ALIVE:
                    bodyModel.SetActive(true);
                    weaponModel.SetActive(true);
                    break;
                case EPlayerStatus.DEAD:
                    bodyModel.SetActive(false);
                    weaponModel.SetActive(false);
                    break;
                case EPlayerStatus.SPECTATING:
                    bodyModel.SetActive(false);
                    weaponModel.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }
}
