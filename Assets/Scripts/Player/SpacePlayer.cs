using System;
using System.Collections;
using System.Collections.Generic;
using Game.Managers;
using Mirror;
using UI;
using UI.ScoreBoard;
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
        [SerializeField] private bool canRespawn;
        [SerializeField] private float timeToRespawn = 3.0f;
        [SerializeField] private float timeToTeleport = 0.2f;
        [SerializeField] private float spawnProtectionTime = 3.0f;

        private Vector3 _respawnPoint;
        private bool _respawning;
        private bool _spawnProtected;
        
        // ---------------- Systems ---------------- 
        [Header("Systems")]
        [SerializeField] private Weapon weapon;

        [Tooltip("Position of the camera to be set")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Collider[] colliders;

        // ---------------- UI ---------------- 
        [Header("UI")] 
        [SerializeField] private GameObject playerCanvas;
        [SerializeField] private GameObject playerUI;
        [SerializeField] private GameObject deathScreenUI;
        [SerializeField] private GameObject scoreboardUI;
        [SerializeField] private GameObject roundUI;
        private DeathUIManager _deathUIManager;
        
        // ---------------- Visuals ---------------- 
        [Header("Visuals")] 
        [SerializeField] private GameObject bodyModel;
        [SerializeField] private GameObject weaponModel;
        
        // ---------------- Renderers ---------------- 
        [Header("Renderers")] 
        [SerializeField] private MeshRenderer[] meshRenderers;
        
        // ---------------- Events ---------------- 
        public static Action<EPlayerStatus> OnPlayerStatusUpdated;
        public static Action<DeathInfo> OnPlayerKills;
        public static Action<DeathInfo> OnPlayerDies;
        public ref Transform CameraTransform => ref cameraTransform;
        
        private NetworkIdentity _networkIdentity;

        private void Start()
        {
            _deathUIManager = deathScreenUI.GetComponent<DeathUIManager>();
            _networkIdentity = GetComponent<NetworkIdentity>();

            playerStats.ResetCurrentHealth();

            bool isLocalPlayer = _networkIdentity.isLocalPlayer;
            // Make the player body only visible to other players
            SetMeshRendersActive(!isLocalPlayer);
            SetUIActive(isLocalPlayer);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            uint netId = GetComponent<NetworkIdentity>().netId;
            SpacePlayer player = GetComponent<SpacePlayer>();

            GameManager.AddPlayer(netId, player);
            
            if (!isLocalPlayer) return;
            weapon.OnWeaponFire += OnPlayerWeaponFire;
            CmdUpdateScoreRow(true, netId);
            UpdateScoreBoard();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            
            uint netId = GetComponent<NetworkIdentity>().netId;
            SpacePlayer player = GetComponent<SpacePlayer>();

            GameManager.RemovePlayer(netId);
            if (!isLocalPlayer) return;
            
            // TODO: remove the scorerow when the player leaves
        }
        
        // Checks if the server has more than 1 player, if so, adds the respective score rows
        private void UpdateScoreBoard()
        {
            foreach (SpacePlayer player in GameManager.GetPlayers().Values)
            {
                uint playerNetId = player.netId;
                if (playerNetId == netId) 
                    continue;
                ScoreRowData rowData = new ScoreRowData($"Player {playerNetId}");
                ScoreBoardManager.Singleton.AddRow(playerNetId, rowData);
            }
        }

        public void SetRespawnPoint(Vector3 position) => _respawnPoint = position;

        [ClientRpc]
        private void RpcRespawn(string fromPlayer, string weaponName)
        {
            DeathInfo deathInfo = new DeathInfo(_networkIdentity.name, fromPlayer, weaponName);
            
            // Death
            OnPlayerDies?.Invoke(deathInfo);
            playerStats.AddDeath(fromPlayer, weaponName);
            
            // Kill
            SpacePlayer playerKiller = GameManager.GetPlayer(fromPlayer);
            PlayerStats playerKillerStats = playerKiller.PlayerStats;
            playerKillerStats.AddKill();
            
            // Update score board UI
            ScoreBoardManager.Singleton.SetRowStats(playerKiller.netId, playerKillerStats.GetStats());
            ScoreBoardManager.Singleton.SetRowStats(netId, playerStats.GetStats());
            
            StartCoroutine(RespawnCoroutine(true));
        }
        
        [Server]
        public bool TakeDamage(float damage, string fromPlayer, string weaponName)
        {
            if (_spawnProtected)
            {
                Debug.Log($"(CMD) PlayerProtected");
                return false;
            }
                
            Debug.Log($"(CMD) Taking Damage: {damage} from {fromPlayer}");
            playerStats.CurrentHealth -= damage;

            bool isPlayerDead = playerStats.CurrentHealth <= 0.0f && canRespawn && !_respawning;
            
            if (isPlayerDead)
            {
                Debug.Log("Calling RPCRespawn");
                RpcRespawn(fromPlayer, weaponName);
            }

            return isPlayerDead;
        }

        public IEnumerator RespawnCoroutine(bool wasKilled)
        {
            _respawning = true;

            if (wasKilled)
            {
                SetPlayerStatus(EPlayerStatus.DEAD);
                _deathUIManager.SetTextKilledBy(playerStats.KilledByPlayer, playerStats.KilledByWeapon);

                if (!GameModeManager.RespawnEnabled)
                {
                    SetPlayerStatus(EPlayerStatus.SPECTATING);
                    
                    yield return new WaitForSeconds(timeToRespawn);
                    _deathUIManager.SetKilledTextEmpty();
                    _respawning = false;
                    yield break;
                }

                yield return new WaitForSeconds(timeToRespawn);
                
                _deathUIManager.SetKilledTextEmpty();
            }
            
            ChangePlayerPosition();
            
            // We wait so we give time to change the player position
            // With that the others players don't see a player teleporting
            yield return new WaitForSeconds(timeToTeleport);
            SetPlayerStatus(EPlayerStatus.ALIVE);

            playerStats.ResetCurrentHealth();
            _respawning = false;
        }
        
        private IEnumerator SpawnProtectionCoroutine()
        {
            _spawnProtected = true;
            yield return new WaitForSeconds(spawnProtectionTime);
            _spawnProtected = false;
        }
        
        private void OnPlayerWeaponFire()
        {
            StopCoroutine(SpawnProtectionCoroutine());
            _spawnProtected = false;
        }

        private void SetPlayerStatus(EPlayerStatus status)
        {
            Debug.Log($"PlayerStatusUpdated: {status.ToString()} NetId: {netId} ObjectName: {gameObject.name}");
            if(isLocalPlayer) 
                OnPlayerStatusUpdated?.Invoke(status);
            
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
                    
                    // Systems
                    StartCoroutine(SpawnProtectionCoroutine());
                    weapon.SetWeaponActive(true);
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
                    
                    // Systems
                    weapon.SetWeaponActive(false);
                    break;
                case EPlayerStatus.SPECTATING:
                    // Colliders
                    SetCollidersActive(false);
                    
                    // Models
                    bodyModel.SetActive(false);
                    weaponModel.SetActive(false);
                    
                    // TODO: Activate spectator UI
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

        private void SetMeshRendersActive(bool value)
        {
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                meshRenderer.shadowCastingMode = value ? ShadowCastingMode.On : ShadowCastingMode.ShadowsOnly;
            }
        }

        private void SetUIActive(bool active)
        {
            playerCanvas.SetActive(active);
            playerUI.SetActive(active);
            roundUI.SetActive(active);
            //scoreboardUI.SetActive(value);
        }

        private void SetPlayerStats(PlayerStats oldStats, PlayerStats newStats)
        {
            oldStats = newStats;
        }

        [Command]
        private void CmdUpdateScoreRow(bool add, uint playerNetId)
        {
            RpcUpdateScoreRow(add, playerNetId);
        }
        
        [ClientRpc]
        private void RpcUpdateScoreRow(bool add, uint playerNetId)
        {
            if (add)
            {
                ScoreRowData rowData = new ScoreRowData($"Player {playerNetId}");
                ScoreBoardManager.Singleton.AddRow(playerNetId, rowData);
            }
            else
            {
                ScoreBoardManager.Singleton.RemoveRow(playerNetId);
            }
        }
    }
}
