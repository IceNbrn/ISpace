using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [Serializable]
    public abstract class GameMode : ScriptableObject
    {
        [SerializeField] private GameRound[] rounds;
        [SerializeField] private float roundTime;
        [SerializeField] private ushort maxTeams;
        [SerializeField] private ushort maxPlayersPerTeam;

        public ref GameRound[] GetRounds() => ref rounds; 
        
        private bool _ended;

        public virtual void LoadGameMode()
        {
            for (ushort i = 0; i < rounds.Length; ++i)
            {
                rounds[i] = new GameRound(i, roundTime);
            }
        }

        protected virtual bool GameModeEnded()
        {
            return _ended;
        }

        private void LoadSettings()
        {
            
        }
    }
}