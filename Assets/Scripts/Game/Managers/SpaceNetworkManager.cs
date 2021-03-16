using System;
using Mirror;
using UnityEngine;

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
        Debug.Log("StopServer");
    }
    
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log($"Player {conn.identity.netId} added!");
    }

}