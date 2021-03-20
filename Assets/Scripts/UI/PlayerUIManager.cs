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
        private TextMeshProUGUI textInfo, textFps;
        
        [SerializeField] private float hudRefreshRate = 1f;
        
        private float _timer;
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        void OnGUI()
        {
            NetworkManager networkManager = NetworkManager.singleton;
            
            if (networkManager != null && !networkManager.isNetworkActive)
                return;
            
            string infoResult = String.Empty;

            int ping = (int)NetworkTime.rtt * 1000;
            

            if (showPing)
                infoResult += $"[Ping]       : {ping} ms\n";

            
            
            int playesOnline = GameManager.PlayersOnline;
            
            infoResult += $"[Players] : {playesOnline}/{networkManager.maxConnections}";
            
            textInfo.SetText(infoResult);
        }

        private void Update()
        {
            if (Time.unscaledTime > _timer)
            {
                int fps = (int)(1f / Time.unscaledDeltaTime);
                textFps.SetText($"[FPS]       : {fps}\n");
                _timer = Time.unscaledTime + hudRefreshRate;
            }
        }
    }
}
