using System.Collections;
using System.Collections.Generic;
using Mirror;
using Player;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton { get; private set; }
    public static int PlayersOnline { get; private set; }

    private const string PLAYER_PREFIX = "Player ";
    private static Dictionary<string, SpacePlayer> _players = new Dictionary<string, SpacePlayer>();

    // Start is called before the first frame update
    void Awake()
    {
        if (!InitializeSingleton()) 
            return;
        
        Application.targetFrameRate = -1;
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
        PlayersOnline = FindPlayersByTag("Player");
    }
    
    public int FindPlayersByTag(string tag)
    {
        int players = GameObject.FindGameObjectsWithTag(tag).Length;
        return players;
    }

    public static void AddPlayer(string netId, SpacePlayer player)
    {
        string _playerId = PLAYER_PREFIX + netId;
        _players.Add(_playerId, player);
        player.transform.name = _playerId;
    }

    public static void RemovePlayer(string playerId)
    {
        if(_players.ContainsKey(playerId))
            _players.Remove(playerId);
    }

    public static SpacePlayer GetPlayer(string playerId) => _players[playerId];

}
