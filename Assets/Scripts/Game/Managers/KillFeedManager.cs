using System;
using System.Collections;
using Mirror;
using Player;
using TMPro;
using UnityEngine;

namespace Game.Managers
{
    public class KillFeedManager : NetworkBehaviour
    {
        [SerializeField] private float killFeedTime = 3.0f;
        [SerializeField] private GameObject killFeed;
        [SerializeField] private TextMeshProUGUI killFeedText;

        private string _currentText;
        private bool _isKillFeedActive;
        private EPlayerStatus? _status;
        
        
        private void Awake()
        {
            SpacePlayer.OnPlayerStatusUpdated += OnPlayerStatusUpdated;
        }

        
        private void OnPlayerStatusUpdated(EPlayerStatus obj)
        {
            if (!isLocalPlayer) return;
            
            _status = obj;
            CmdShowKillFeed();
        }

        [Command]
        private void CmdShowKillFeed()
        {
            RpcShowKillFeed();
        }

        [ClientRpc]
        private void RpcShowKillFeed()
        {
            if (_isKillFeedActive)
                _currentText = killFeedText.text;
            else
                StartCoroutine(ShowKillFeedCoroutine());
        }
        
        private IEnumerator ShowKillFeedCoroutine()
        {
            _isKillFeedActive = true;

            killFeed.SetActive(true);
           
            killFeedText.SetText($"{_status?.ToString()} \n {_currentText} \n");
            
            yield return new WaitForSeconds(killFeedTime);
            killFeed.SetActive(false);
            _isKillFeedActive = false;
            _status = null;
        }
    }
}