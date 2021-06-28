using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Player;

namespace Game
{
    public class GameTeam
    {
        public Dictionary<NetworkIdentity, Stats> PlayersScores { get; private set; }
        public int MaxSize { get; private set; }
        
        public GameTeam () {}

        public GameTeam(Dictionary<NetworkIdentity, Stats> playersScores)
        {
            PlayersScores = playersScores;
            MaxSize = playersScores.Count;
        }

        public override string ToString()
        {
            string ouput = String.Empty;
            foreach (var playerRecord in PlayersScores)
            {
                NetworkIdentity networkIdentity = playerRecord.Key;
                ouput += $"{networkIdentity.name} -> {playerRecord.Value.Kills}";
            }

            return ouput;
        }

        public string GetPlayers()
        {
            string ouput = String.Empty;
            if (PlayersScores.Count > 1)
            {
                foreach (var playerRecord in PlayersScores)
                {
                    NetworkIdentity networkIdentity = playerRecord.Key;
                    ouput += $"{networkIdentity.name} - ";
                }
            }
            else
            {
                ouput = PlayersScores.Keys.ElementAt(0).name;
            }
            return ouput;
        }

        public int GetScore()
        {
            return PlayersScores.Count == 1 ? PlayersScores.Values.ElementAt(0).Kills : 0;
        }
    }
}