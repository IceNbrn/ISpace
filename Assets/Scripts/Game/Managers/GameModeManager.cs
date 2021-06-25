using System;
using System.Collections;
using System.Linq;
using Mirror;
using Player;
using UnityEngine;

namespace Game.Managers
{
    public class GameModeManager : NetworkBehaviour
    {
        [SerializeField]
        private GameMode gameMode;

        public static bool RespawnEnabled;
        public static Action<GameRound> OnGameRoundEnd;
        
        private string _gameModeName;

        private void Start()
        {
            _gameModeName = gameMode.name;
            Debug.Log($"GameMode Loaded: {_gameModeName}");
            gameMode.LoadGameMode();
            RespawnEnabled = gameMode.RoundRespawnEnabled;

            if(isServer)
                StartCoroutine(StartGameMode());
        }

        [Server]
        private IEnumerator StartGameMode()
        {
            GameRound[] gameRounds = gameMode.GetRounds();
            for (int i = 0; i < gameRounds.Length; i++)
            {
                GameRound indexRound = gameRounds[i];
                Debug.Log($"GameRound Started: {indexRound.Index}");
                
                yield return new WaitForSeconds(indexRound.Time);
                //OnGameRoundEnd.Invoke(indexRound);
                
                RpcRoundEnded();
                RpcRespawnPlayers();
            }

            yield return null;
        }

        [ClientRpc]
        private void RpcRespawnPlayers()
        {
            SpacePlayer[] players = GameManager.GetPlayers().Values.ToArray();
            for (int i = 0; i < players.Length; ++i)
            {
                SpacePlayer spacePlayer = players[i];
                StartCoroutine(spacePlayer.RespawnCoroutine(false));
            }
        }

        [ClientRpc]
        private void RpcRoundEnded()
        {
             
            Debug.Log($"RPC GameRound Ended");
        }
        
        
    }
}