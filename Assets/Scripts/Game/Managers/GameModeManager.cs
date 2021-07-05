using System;
using System.Collections;
using System.Linq;
using Mirror;
using Player;
using UI;
using UI.ScoreBoard;
using UnityEngine;

namespace Game.Managers
{
    public class GameModeManager : NetworkBehaviour
    {
        [SerializeField] private GameMode gameMode;
        [SerializeField] protected float matchLoadTime;

        public static bool RespawnEnabled;
        public static Action<int> OnGameRoundStarted;
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
            if (isServer)
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
                
                for (ushort time = indexRound.Time; time > 0; time--)
                {
                    Debug.Log($"RoundTimeLeft: {time}");
                    RpcShowTimer(time);
                    yield return new WaitForSeconds(1);
                }
                RpcShowTimer(0);
                
                //OnGameRoundEnd.Invoke(indexRound);
                
                RpcRoundEnded();
                yield return new WaitForSeconds(gameMode.RoundEndTime);
            }
            yield return new WaitForSeconds(matchLoadTime);
            GameMatchEnd();
            yield return null;
        }

        [ClientRpc]
        private void RpcShowTimer(ushort time) => RoundUIManager.Singleton?.SetTxtTimer(time);

        [Server]
        private void AssignPlayersToTeams()
        {
            GameTeam[] gameTeams = gameMode.GetTeams();
            int gameTeamsSize = gameTeams.Length;
            if (gameTeamsSize < 2) return;

            int playersCount = NetworkIdentity.spawned.Count;
            int minTotalPlayers = 0;
            
            for (int i = 0; i < gameTeamsSize; i++)
                minTotalPlayers += gameTeams[i].MinSize;

            if (playersCount >= minTotalPlayers)
            {
                if (gameTeamsSize == 2)
                {
                    GameTeam biggerTeam = GetBiggerTeam();
                    GameTeam smallestTeam = GetSmallestTeam();

                    int playerPerTeam = biggerTeam.MaxSize / smallestTeam.MaxSize;
                    
                    for (int i = 0; i < playersCount; i++)
                    {
                        var playersIdentities = NetworkIdentity.spawned.Values;
                        NetworkIdentity player = playersIdentities.ElementAt(0);
                        
                        if (biggerTeam.Size <= playerPerTeam)
                            smallestTeam.AddPlayer(player);
                    }

                }
                
            }
            else
            {
                // Not enough players to play 
            }
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

        private GameTeam GetBiggerTeam()
        {
            var teams = gameMode.GetTeams().OrderByDescending(team => team.MaxSize);
            return teams.ElementAt(0);
        }
        
        private GameTeam GetSmallestTeam()
        {
            var teams = gameMode.GetTeams().OrderBy(team => team.MaxSize);
            return teams.ElementAt(0);
        }
        
    }
}