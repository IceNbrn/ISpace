using System.Collections;
using System.Collections.Generic;
using Mirror;
using Player;
using SaveSystem;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton { get; private set; }
    public static ref Dictionary<string, SpacePlayer> GetPlayers() => ref _players;
    public static int GetPlayersOnline() => _players.Count;
    
    private const string PLAYER_PREFIX = "Player ";
    private static Dictionary<string, SpacePlayer> _players = new Dictionary<string, SpacePlayer>();
    
    [SerializeField] 
    public PlayerSettings PlayerSettings;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (!InitializeSingleton()) 
            return;
        
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;

        LoadPlayerSettings();
        
        PlayerSettings.OnSettingsUpdated += OnSettingsUpdated;
    }

    private void LoadPlayerSettings()
    {
        PlayerSettings = SaveDataManager.LoadPlayerJson() ?? PlayerSettings;
    }

    private void OnSettingsUpdated()
    {
        SaveDataManager.SavePlayerJson(PlayerSettings);
    }

    private bool InitializeSingleton()
    {
        if (Singleton != null && Singleton == this) 
            return true;

        if (Singleton != null)
        {
            Destroy(gameObject);
            return false;
        }
        
        Singleton = this;
        if (Application.isPlaying) 
            DontDestroyOnLoad(gameObject);
        
        return true;
    }

    public void UpdatePlayersCount()
    {
        //PlayersOnline = FindPlayersByTag("Player");
    }
    
    public int FindPlayersByTag(string tag)
    {
        int players = GameObject.FindGameObjectsWithTag(tag).Length;
        return players;
    }

    public static void AddPlayer(uint netId, SpacePlayer player)
    {
        string _playerId = PLAYER_PREFIX + netId.ToString();
        _players.Add(_playerId, player);
        player.transform.name = _playerId;
    }

    public static void RemovePlayer(uint playerId)
    {
        string playerIdStr = playerId.ToString();
        if(_players.ContainsKey(playerIdStr))
            _players.Remove(playerIdStr);
    }

    public static SpacePlayer GetPlayer(string playerId) => _players[playerId];

    public void SetSensitivity(float value)
    {
        value *= 0.1f;
        PlayerSettings.Sensitivity = value;
        PlayerMovementController.SetSensitivity(value);
    }

    //public ref PlayerSettings PlayerSettings => ref PlayerSettings;

    public void UpdateCrosshair(CrosshairSettings crosshairSettings)
    {
        PlayerSettings.CrosshairSettings.Override(crosshairSettings);
        PlayerSettings.OnSettingsUpdated.Invoke();
        PlayerSettings.OnCrosshairUpdated.Invoke();
    }
}
