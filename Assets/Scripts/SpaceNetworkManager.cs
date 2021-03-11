using System;
using Mirror;
using UnityEngine;

public class SpaceNetworkManager : MonoBehaviour
{
    private NetworkManager _networkManager;
    private void Start()
    {
        _networkManager = GetComponent<NetworkManager>();
        
        
    }

    public void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("Client connected");
    }
}