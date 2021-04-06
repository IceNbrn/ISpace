using System;
using System.Collections;
using Mirror;
using UI;
using UnityEngine;
using UnityEngine.Rendering;
using Weapons;
using Random = UnityEngine.Random;

namespace Player
{
    public enum EPlayerStatus
    {
        ALIVE,
        DEAD,
        SPECTATING
    }

    public struct DeathInfo 
    {
        public string PlayerKilled { get; private set; }
        public string KilledByPlayer { get; private set; }
        public string KilledByWeapon { get; private set; }

        public DeathInfo(string playerKilled, string killedByPlayer, string killedByWeapon)
        {
            PlayerKilled = playerKilled;
            KilledByPlayer = killedByPlayer;
            KilledByWeapon = killedByWeapon;
        }
    }
    
    public class SpacePlayer : NetworkBehaviour
    {
        // ----------------  Stats  ----------------  
        [Header("Stats")]
        [SerializeField] 
        private PlayerStats playerStats;

        public PlayerStats PlayerStats => playerStats;

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

        [Tooltip("Position of the camera to be set")]
        [SerializeField] private Transform cameraTransform;
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
        
        // ---------------- Renderers ---------------- 
        [Header("Renderers")] 
        [SerializeField] private MeshRenderer[] meshRenderers;
        
        public static Action<EPlayerStatus> OnPlayerStatusUpdated;
        public static Action<DeathInfo> OnPlayerDies;
        public ref Transform CameraTransform => ref cameraTransform;
        
        private NetworkIdentity _networkIdentity;

        private void Start()
        {
            _deathUIManager = deathScreenUI.GetComponent<DeathUIManager>();
            _networkIdentity = GetComponent<NetworkIdentity>();

            playerStats.ResetCurrentHealth();
            
            // Make the player body only visible to other players
            SetMeshRendersActive(!_networkIdentity.isLocalPlayer);
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
        private void RpcRespawn(string fromPlayer, string weaponName)
        {
            DeathInfo deathInfo = new DeathInfo(_networkIdentity.name, fromPlayer, weaponName);
            OnPlayerDies?.Invoke(deathInfo);
            playerStats.AddDeath(fromPlayer, weaponName);
            StartCoroutine(RespawnCoroutine());
        }
        
        [Server]
        public void TakeDamage(float damage, string fromPlayer, string weaponName)
        {
            Debug.Log($"(CMD)Taking Damage: {damage} from {fromPlayer}");
            playerStats.CurrentHealth -= damage;

            bool isPlayerDead = playerStats.CurrentHealth <= 0.0f && canRespawn && !_respawning;
            
            if (isPlayerDead)
                RpcRespawn(fromPlayer, weaponName);
        }

        private IEnumerator RespawnCoroutine()
        {
            _respawning = true;
            
            SetPlayerStatus(EPlayerStatus.DEAD);
            _deathUIManager.SetTextKilledBy(playerStats.KilledByPlayer, playerStats.KilledByWeapon);
            
            yield return new WaitForSeconds(timeToRespawn);
            
            _deathUIManager.SetKilledTextEmpty();
            
            ChangePlayerPosition();
            SetPlayerStatus(EPlayerStatus.ALIVE);

            playerStats.ResetCurrentHealth();
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
            OnPlayerStatusUpdated?.Invoke(status);
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

        private void SetMeshRendersActive(bool value)
        {
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                meshRenderer.shadowCastingMode = value ? ShadowCastingMode.On : ShadowCastingMode.ShadowsOnly;
            }
        }

        private void SetPlayerStats(PlayerStats oldStats, PlayerStats newStats)
        {
            oldStats = newStats;
        }
    }
}
