using System;
using UnityEngine;

namespace Game.GameModes
{
    [CreateAssetMenu(fileName = "GameModeHnS", menuName = "GameModes/Hide and Seek")]
    public class GameModeHnS : GameMode
    {
        public override void LoadGameMode()
        {
            base.LoadGameMode();
        }

        protected override bool GameModeEnded()
        {
            throw new NotImplementedException();
        }
    }
}