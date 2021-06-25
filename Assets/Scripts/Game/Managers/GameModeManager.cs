using System;
using System.Collections;
using Mirror;
using UnityEngine;

namespace Game.Managers
{
    public class GameModeManager : NetworkBehaviour
    {
        [SerializeField]
        private GameMode gameMode;

        public static Action<GameRound> OnGameRoundEnd;
        
        private string _gameModeName;

        private void Start()
        {
            _gameModeName = gameMode.name;
            Debug.Log($"GameMode Loaded: {_gameModeName}");
            gameMode.LoadGameMode();

            if(isServer)
                StartCoroutine(StartGameMode());
        }

        [Server]
        private IEnumerator StartGameMode()
        {
            GameRound[] gameRounds = gameMode.GetRounds();
            for (int i = 0; i < gameRounds.Length; i++)
            {
                GameRound indexRound = gameRounds[i];
                Debug.Log($"GameRound Started: {indexRound.Index}");
                yield return new WaitForSeconds(indexRound.Time);
                //OnGameRoundEnd.Invoke(indexRound);
                RpcRoundEnded();
                
                
            }

            yield return null;
        }

        [ClientRpc]
        private void RpcRoundEnded()
        {
            Debug.Log($"GameRound Ended");
        }
    }
}