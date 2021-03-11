using System;
using Mirror;
using TMPro;
using UnityEngine;

namespace UI
{
    public class PlayerUIManager : MonoBehaviour
    {
        [SerializeField] 
        private bool showPing = true;

        [SerializeField] 
        private TextMeshProUGUI textInfo;
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void OnGUI()
        {
            NetworkManager networkManager = NetworkManager.singleton;
            
            if (!networkManager.isNetworkActive)
                return;
            
            string infoResult = String.Empty;

            int ping = (int)NetworkTime.rtt * 1000;
            

            if (showPing)
                infoResult += $"[Ping]       : {ping} ms\n";

            int playesOnline = GameManager.Singleton.FindPlayersByTag();
            
            infoResult += $"[Players] : {playesOnline}/{networkManager.maxConnections}";
            
            textInfo.SetText(infoResult);
        }
    }
}
