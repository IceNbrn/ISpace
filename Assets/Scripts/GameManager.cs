using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton { get; private set; }
    public static int PlayersOnline { get; private set; }

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

    private void UpdatePlayersCount()
    {
        PlayersOnline++;
    }
    
    public int FindPlayersByTag()
    {
        int players = GameObject.FindGameObjectsWithTag("Player").Length;
        return players;
    }
    
}
