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
            int ping = (int) (NetworkTime.rtt / 2) * 1000;
            
            if (showPing)
                infoResult += $"[Ping]       : {ping} ms\n";

            int playesOnline = GameManager.PlayersOnline;
            infoResult += $"[Players] : {playesOnline}/{networkManager.maxConnections}";
            
            textFps.SetText($"[FPS] {_avgFps}");
            textInfo.SetText(infoResult);
        }

        private float _avgFps;
        
        private void Update()
        {
            
            float current = 0;
            current = (int)(1f / Time.unscaledDeltaTime);
            _avgFps = (int)current;
            
        }
    }
}
