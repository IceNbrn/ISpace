using System;
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
    }
}