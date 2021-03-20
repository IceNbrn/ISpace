using System;
using System.Collections;
using Mirror;
using UI;
using UnityEngine;
using Weapons;
using Random = UnityEngine.Random;

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

        private int _kills, _deaths;
        private float _killRatio;
        private string _killedBy;
        private float _currentHealth;

        public float GetHealth => health;

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
        
        [SerializeField] private Collider[] colliders;

        // ---------------- UI ---------------- 
        [Header("UI")] 
        [SerializeField] private GameObject playerUI;
        [SerializeField] private GameObject deathScreenUI;
        private DeathUIManager _deathUIManager;
        
        // ---------------- Visuals ---------------- 
        [Header("Visuals")] 
        [SerializeField] private GameObject bodyModel;
        [SerializeField] private GameObject weaponModel;

        private void Start()
        {
            _deathUIManager = deathScreenUI.GetComponent<DeathUIManager>();
        }

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
        
        public void SetRespawnPoint(Vector3 position) => _respawnPoint = position;
        
        [ClientRpc]
        public void RpcTakeDamage(float damage, string fromPlayer)
        {
            Debug.Log($"Taking Damage: {damage} from {fromPlayer}");
            _currentHealth -= damage;

            if (_currentHealth <= 0.0f)
            {
                if (canRespawn && !_respawning)
                {
                    // Player is dead, so time to respawn
                    _deaths++;
                    _killedBy = fromPlayer;
                    StartCoroutine(RespawnCoroutine());
                    
                    //TODO: Send a rpc to the killer saying that he killed me
                }
                return;
            }
            return;
        }

        private IEnumerator RespawnCoroutine()
        {
            _respawning = true;
            
            SetPlayerStatus(EPlayerStatus.DEAD);
            _deathUIManager.SetTextKilledBy(_killedBy, "TEST");
            
            yield return new WaitForSeconds(timeToRespawn);
            
            _deathUIManager.SetKilledTextEmpty();
            _killedBy = String.Empty;
            
            ChangePlayerPosition();
            SetPlayerStatus(EPlayerStatus.ALIVE);

            _currentHealth = health;
            _respawning = false;
        }

        private void SetPlayerStatus(EPlayerStatus status)
        {
            switch (status)
            {
                case EPlayerStatus.ALIVE:
                    // Colliders
                    SetCollidersActive(true);
                    
                    // Models
                    bodyModel.SetActive(true);
                    weaponModel.SetActive(true);
                    
                    // UI
                    deathScreenUI.SetActive(false);
                    playerUI.SetActive(true);
                    break;
                case EPlayerStatus.DEAD:
                    // Colliders
                    SetCollidersActive(false);
                    
                    // Models
                    bodyModel.SetActive(false);
                    weaponModel.SetActive(false);
                    
                    // UI
                    playerUI.SetActive(false);
                    deathScreenUI.SetActive(true);
                    break;
                case EPlayerStatus.SPECTATING:
                    // Colliders
                    SetCollidersActive(false);
                    
                    // Models
                    bodyModel.SetActive(false);
                    weaponModel.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        private void SetCollidersActive(bool value)
        {
            for (int i = 0; i < colliders.Length; ++i)
            {
                colliders[i].enabled = value;
            }
        }

        private void ChangePlayerPosition()
        {
            int maxPositions = NetworkManager.startPositions.Count;
            int index = Random.Range(0, maxPositions);
            Vector3 position = NetworkManager.startPositions[index].position;
            transform.position = position;
        }
    }
}
