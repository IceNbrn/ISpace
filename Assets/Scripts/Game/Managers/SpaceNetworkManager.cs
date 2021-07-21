using System;
using Mirror;
using UI.ScoreBoard;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceNetworkManager : NetworkManager
{
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("StartServer");
    }
    
    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("StopServer");
    }
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("StartClient");
        GameManager.Singleton.UpdatePlayersCount();
    }
    
    public override void OnStopClient()
    {
        base.OnStopClient();
        //SceneManager.LoadSceneAsync(0);
        Debug.Log("StopClient");
    }
    
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log($"Player {conn.identity.netId} added!");
        
    }

}