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
        public int Size { get; private set; }
        public int MinSize { get; private set; }
        public int MaxSize { get; private set; }
        
        public GameTeam () {}

        public GameTeam(int minSize, int maxSize)
        {
            MinSize = minSize;
            MaxSize = maxSize;
        }
        
        public GameTeam(Dictionary<NetworkIdentity, Stats> playersScores, int maxSize)
        {
            PlayersScores = playersScores;
            Size = playersScores.Count;
            MaxSize = maxSize;
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

        public void AddPlayer(NetworkIdentity playerIdentity)
        {
            PlayersScores ??= new Dictionary<NetworkIdentity, Stats>();
            
            PlayersScores.Add(playerIdentity, null);
        }
    }
}