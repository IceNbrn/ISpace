using System;
using System.Collections.Generic;
using Mirror;
using Player;
using UI.ScoreBoard;
using UnityEngine;

namespace Game.GameModes
{
    [CreateAssetMenu(fileName = "GameModeFFA", menuName = "GameModes/Free for all")]
    public class GameModeFFA : GameMode
    {
        public override void LoadGameMode()
        {
            base.LoadGameMode();
        }


        protected override bool GameModeEnded()
        {
            throw new NotImplementedException();
        }

        public override GameTeam GetWinner()
        {
            KeyValuePair<uint, ScoreRow>? bestScore = ScoreBoardManager.Singleton.GetPlayerBestScore();
            if (!bestScore.HasValue) return null;
            
            ScoreRowData scoreRowStats = bestScore.Value.Value.GetStats();
            Stats stats = new Stats(scoreRowStats.PlayerKills, scoreRowStats.PlayerAssists,scoreRowStats.PlayerKills,scoreRowStats.PlayerPoints);

            uint pNetId = bestScore.Value.Key;
            if (!NetworkIdentity.spawned.ContainsKey(pNetId)) 
                return null;
            
            NetworkIdentity bestPlayer = NetworkIdentity.spawned[pNetId];
            Dictionary<NetworkIdentity, Stats> playerScore = new Dictionary<NetworkIdentity, Stats>(){ {bestPlayer, stats } };
            GameTeam gameTeam = new GameTeam(playerScore, 1);
            return gameTeam;

        }
    }
}