using System;
using System.Collections;
using System.Linq;
using Mirror;
using Player;
using UI;
using UnityEngine;

namespace Game.Managers
{
    public class GameModeManager : NetworkBehaviour
    {
        [SerializeField] private GameMode gameMode;
        [SerializeField] protected float matchLoadTime;

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
        
        private void GameMatchEnd()
        {
            if(isServer)
                StartCoroutine(StartGameMode());
        }

        [Server]
        private IEnumerator StartGameMode()
        {
            GameRound[] gameRounds = gameMode.GetRounds();
            for (int i = 0; i < gameRounds.Length; i++)
            {
                RpcRespawnPlayers();
                GameRound indexRound = gameRounds[i];
                Debug.Log($"GameRound Started: {indexRound.Index}");
                
                yield return new WaitForSeconds(indexRound.Time);
                //OnGameRoundEnd.Invoke(indexRound);
                
                RpcRoundEnded();
                yield return new WaitForSeconds(gameMode.RoundEndTime);
            }
            yield return new WaitForSeconds(matchLoadTime);
            GameMatchEnd();
            yield return null;
        }

        [ClientRpc]
        private void RpcRespawnPlayers()
        {
            SpacePlayer[] players = GameManager.GetPlayers().Values.ToArray();
            for (int i = 0; i < players.Length; ++i)
            {
                SpacePlayer spacePlayer = players[i];
                if (spacePlayer != null)
                    StartCoroutine(spacePlayer.RespawnCoroutine(false));
                else
                    Debug.Log("[GameModeManager]: Trying to spawn a player that is not in-game");
            }
        }

        [ClientRpc]
        private void RpcRoundEnded()
        {
            RoundUIManager.Instance.SetWinnerText(gameMode.GetWinner().GetPlayers(), gameMode.GetWinner().GetScore());
            Debug.Log($"RPC GameRound Ended | Winner: {gameMode.GetWinner().ToString()}");
        }
        
        
    }
}