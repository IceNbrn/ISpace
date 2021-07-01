using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [Serializable]
    public abstract class GameMode : ScriptableObject
    {
        
        [SerializeField] protected int totalRounds = 1;
        [SerializeField] protected ushort roundTime = 300;
        [SerializeField] protected float roundEndTime;
        [SerializeField] protected bool roundRespawnEnabled;
        [SerializeField] protected ushort totalTeams;
        [SerializeField] protected ushort maxPlayersPerTeam;
        protected GameRound[] _rounds;
        protected GameTeam[] _teams;
        
        public int TotalRounds => totalRounds;
        public float RoundEndTime => roundEndTime;
        public bool RoundRespawnEnabled => roundRespawnEnabled;
        public ref GameRound[] GetRounds() => ref _rounds; 
        public ref GameTeam[] GetTeams() => ref _teams; 
        
        private bool _ended;

        public virtual void LoadGameMode()
        {
            _rounds = new GameRound[totalRounds];
            for (ushort i = 0; i < _rounds.Length; ++i)
            {
                _rounds[i] = new GameRound(i, roundTime, roundRespawnEnabled);
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