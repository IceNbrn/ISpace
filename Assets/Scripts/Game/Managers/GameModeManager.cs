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

                for (int time = indexRound.Time; time >= 0; time--)
                {
                    Debug.Log($"RoundTimeLeft: {time}");
                    if(RoundUIManager.Singleton != null)
                        RoundUIManager.Singleton.SetTxtTimer(time);
                    yield return new WaitForSeconds(1);
                }
                
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
            GameTeam winner = gameMode.GetWinner();
            if (winner == null)
                return;
            string textWinner = gameMode.TotalRounds == 1 ? "wins the game" : "wins the round";
            RoundUIManager.Singleton.SetWinnerText($"{winner.GetPlayers()} {textWinner}", winner.GetScore());
            Debug.Log($"RPC GameRound Ended | Winner: {winner.ToString()}");
        }
        
        
    }
}