using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.ScoreBoard
{
    public class ScoreBoardManager : MonoBehaviour
    {
        public static ScoreBoardManager Singleton { get; private set; }
        public bool IsScoreBoardActive { get; private set; }
        
        [SerializeField] 
        private GameObject rowPrefab;
        [SerializeField] 
        private ScrollViewContent scrollViewContent;
        [SerializeField] 
        private GameObject table;

        private PlayerInActions _controls;
        private Dictionary<uint, ScoreRow> _scoreRows;
        
        private void Start()
        {
            if (!InitializeSingleton()) 
                return;

            _scoreRows = new Dictionary<uint, ScoreRow>();
            _controls = PlayerInputs.PlayerControls;
            _controls.Player.Scoreboard.Enable();
            
            _controls.Player.Scoreboard.started += context => ScoreboardOnPerformed(true);
            _controls.Player.Scoreboard.canceled += context => ScoreboardOnPerformed(false);
            
        }

        private bool InitializeSingleton()
        {
            if (Singleton != null && Singleton == this) 
                return true;

            if (Singleton != null)
            {
                Destroy(gameObject);
                return false;
            }
        
            Singleton = this;
            if (Application.isPlaying) 
                DontDestroyOnLoad(gameObject);
        
            return true;
        }

        public void AddRow(uint playerNetId, ScoreRowData rowData)
        {
            GameObject instantiatedObject = scrollViewContent.AddContent(rowPrefab);
            if (instantiatedObject != null)
            {
                ScoreRow rowInstantiated = instantiatedObject.GetComponent<ScoreRow>();
                rowInstantiated.SetData(rowData);

                if (!_scoreRows.ContainsKey(playerNetId))
                    _scoreRows.Add(playerNetId, rowInstantiated);
                else
                    _scoreRows[playerNetId] = rowInstantiated;

            }
        }
        
        public void RemoveRow(uint playerNetId)
        {
            Debug.Log("Removing row");
            if (_scoreRows.ContainsKey(playerNetId))
            {
                Destroy(_scoreRows[playerNetId].gameObject);
                _scoreRows.Remove(playerNetId);
                Debug.Log("Row removed");
            }
        }

        public bool UpdateRowData(uint playerNetId, ScoreRowData rowData)
        {
            if (!_scoreRows.ContainsKey(playerNetId)) 
                return false;
            
            _scoreRows[playerNetId].UpdateData(rowData);
            return true;
        }
        
        public bool SetRowStats(uint playerNetId, Stats stats)
        {
            if (!_scoreRows.ContainsKey(playerNetId)) 
                return false;
            
            _scoreRows[playerNetId].SetStats(stats);
            return true;
        }
        
        private void ScoreboardOnPerformed(bool value)
        {
            IsScoreBoardActive = value;
            table.SetActive(value);
        }

        public KeyValuePair<uint, ScoreRow>? GetPlayerBestScore()
        {
            int kills = 0;
            foreach (KeyValuePair<uint, ScoreRow> scoreRow in _scoreRows.OrderByDescending(key => key.Value.GetStats().PlayerKills))
            {
                Debug.Log($"KEY {scoreRow.Key} VALUE {scoreRow.Value.GetStats().ToString()}");
                return scoreRow;
            }

            return null;
        }
    }
}