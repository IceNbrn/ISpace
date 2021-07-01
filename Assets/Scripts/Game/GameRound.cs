using System;

namespace Game
{
    [Serializable]
    public class GameRound
    {
        public ushort Index { get; private set; }
        public ushort Time { get; private set; }
        public bool RespawnEnabled { get; private set; }
        public GameTeam WinnerTeam { get; private set; }
        public GameTeam LoserTeam { get; private set; }

        public GameRound(ushort index, ushort time, bool respawnEnabled)
        {
            Index = index;
            Time = time;
            RespawnEnabled = respawnEnabled;
        }
    }
}