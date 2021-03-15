using System;
using Mirror;
using UnityEngine;

public class SpaceNetworkManager : NetworkBehaviour
{
    public void Start()
    {
        
        //ClientScene.onLocalPlayerChanged += ClientSceneOnonLocalPlayerChanged;
    }

    public override void OnStartClient()
    {
        Debug.Log("StartClient");
        GameManager.Singleton.UpdatePlayersCount();
        base.OnStartClient();
    }

    private void ClientSceneOnonLocalPlayerChanged(NetworkIdentity oldplayer, NetworkIdentity newplayer)
    {
        Debug.Log("LocalPlayerChanged");
        GameManager.Singleton.UpdatePlayersCount();
    }
}