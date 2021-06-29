using System;
using UnityEngine;

namespace Game.GameModes
{
    [CreateAssetMenu(fileName = "GameModeHnS", menuName = "GameModes/Hide and Seek")]
    public class GameModeHnS : GameMode
    {
        [SerializeField] private int seekerTeamMixSize = 1;
        [SerializeField] private int seekerTeamMaxSize = 4;
        [SerializeField] private int hiderTeamMixSize = 2;
        [SerializeField] private int hiderTeamMaxSize = 12;
        public override void LoadGameMode()
        {
            base.LoadGameMode();
            GameTeam seekerTeam = new GameTeam(seekerTeamMixSize, seekerTeamMaxSize);
            GameTeam hiderTeam = new GameTeam(hiderTeamMixSize, hiderTeamMaxSize);
            _teams = new GameTeam[2];
            
            _teams[0] = seekerTeam;
            _teams[1] = hiderTeam;
        }

        protected override bool GameModeEnded()
        {
            throw new NotImplementedException();
        }

        public override GameTeam GetWinner()
        {
            throw new NotImplementedException();
        }

    }
}