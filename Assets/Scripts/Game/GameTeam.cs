using System.Collections.Generic;
using Mirror;

namespace Game
{
    public class GameTeam
    {
        public List<NetworkIdentity> Players { get; private set; }
        public uint MaxSize { get; private set; }
    }
}