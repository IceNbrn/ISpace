using System;
using System.Collections;
using Mirror;
using Player;
using TMPro;
using UI.ScoreBoard;
using UnityEngine;

namespace Game.Managers
{
    public class KillFeedManager : NetworkBehaviour
    {
        [SerializeField] private float killFeedTime = 3.0f;
        [SerializeField] private GameObject killFeed;
        [SerializeField] private TextMeshProUGUI killFeedText;
        [SerializeField] private TextMeshProUGUI killFeedWeaponText;

        private string _currentText;
        private string _currentWeaponText;
        private bool _isKillFeedActive;
        private DeathInfo? _deadInfo;
        
        
        private void Awake()
        {
            //SpacePlayer.OnPlayerKills += OnPlayerKills;
            //if (!isLocalPlayer) return;
            SpacePlayer.OnPlayerDies += OnPlayerDies;
        }
        
        private void OnPlayerKills(DeathInfo deadInfo)
        {
            if (!isLocalPlayer) return;
            
            _deadInfo = deadInfo;
            CmdShowKillFeed();
        }
        
        private void OnPlayerDies(DeathInfo deadInfo)
        {
            if (!isLocalPlayer) return;
            
            _deadInfo = deadInfo;
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
            /*
            if (_isKillFeedActive)
            {
                _currentText = killFeedText.text;
                _currentWeaponText = killFeedWeaponText.text;
            }
            else
            {
                StartCoroutine(ShowKillFeedCoroutine());
            }*/
            if (_isKillFeedActive)
            {
                _currentText = killFeedText.text;
                _currentWeaponText = killFeedWeaponText.text;
            }
            StartCoroutine(ShowKillFeedCoroutine());
        }
        
        private IEnumerator ShowKillFeedCoroutine()
        {
            _isKillFeedActive = true;
            killFeed.SetActive(true);

            if (_deadInfo.HasValue)
            {
                DeathInfo info = _deadInfo.Value;
                killFeedText.SetText($"{info.KilledByPlayer} killed {info.PlayerKilled} \n {_currentText}");
                killFeedWeaponText.SetText($"{info.KilledByWeapon} \n {_currentWeaponText}");
            }
            
            yield return new WaitForSeconds(killFeedTime);
            
            _isKillFeedActive = false;
            _deadInfo = null;
            killFeed.SetActive(false);
        }
    }
}