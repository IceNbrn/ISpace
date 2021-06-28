using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [Serializable]
    public abstract class GameMode : ScriptableObject
    {
        [SerializeField] protected GameRound[] rounds;
        [SerializeField] protected int roundTime;
        [SerializeField] protected float roundEndTime;
        [SerializeField] protected bool roundRespawnEnabled;
        [SerializeField] protected ushort maxTeams;
        [SerializeField] protected ushort maxPlayersPerTeam;

        public float RoundEndTime => roundEndTime;
        public bool RoundRespawnEnabled => roundRespawnEnabled;
        public ref GameRound[] GetRounds() => ref rounds; 
        
        private bool _ended;

        public virtual void LoadGameMode()
        {
            for (ushort i = 0; i < rounds.Length; ++i)
            {
                rounds[i] = new GameRound(i, roundTime, roundRespawnEnabled);
            }
        }

        protected virtual bool GameModeEnded()
        {
            return _ended;
        }

        public abstract GameTeam GetWinner();

        private void LoadSettings()
        {
            
        }
    }
}