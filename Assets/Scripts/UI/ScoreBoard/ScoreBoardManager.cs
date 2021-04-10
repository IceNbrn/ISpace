using System;
using System.Collections.Generic;
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
        private Dictionary<ScoreRowData, Transform> _rows;

        private void Awake()
        {
            if (!InitializeSingleton()) 
                return;

            _rows = new Dictionary<ScoreRowData, Transform>();
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

        public void AddRow(ScoreRowData rowData)
        {
            GameObject instantiatedObject = scrollViewContent.AddContent(rowPrefab);
            if (instantiatedObject != null)
            {
                ScoreRow rowInstantiated = instantiatedObject.GetComponent<ScoreRow>();
                rowInstantiated.SetRowData(rowData);
            }
        }
        
        private void ScoreboardOnPerformed(bool value)
        {
            IsScoreBoardActive = value;
            table.SetActive(value);
        }
    }
}