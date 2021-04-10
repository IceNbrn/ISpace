using System;
using TMPro;
using UnityEngine;

namespace UI.ScoreBoard
{
    public readonly struct ScoreRowData
    {
        private readonly string _playerName;
        private readonly int _playerKills;
        private readonly int _playerAssists;
        private readonly int _playerDeaths;
        private readonly int _playerPoints;

        public string PlayerName => _playerName;
        public int PlayerKills => _playerKills;
        public int PlayerAssists => _playerAssists;
        public int PlayerDeaths => _playerDeaths;
        public int PlayerPoints => _playerPoints;

        public ScoreRowData(string playerName, int playerKills, int playerAssists, int playerDeaths, int playerPoints)
        {
            _playerName = playerName;
            _playerKills = playerKills;
            _playerAssists = playerAssists;
            _playerDeaths = playerDeaths;
            _playerPoints = playerPoints;
        }
    }
    public class ScoreRow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtPlayerName;
        [SerializeField] private TextMeshProUGUI txtPlayerKills;
        [SerializeField] private TextMeshProUGUI txtPlayerAssists;
        [SerializeField] private TextMeshProUGUI txtPlayerDeaths;
        [SerializeField] private TextMeshProUGUI txtPlayerPoints;

        private ScoreRowData _data;

        public void SetRowData(ScoreRowData newData) => _data = newData;

        private void OnGUI()
        {
            txtPlayerName.SetText(_data.PlayerName);
            txtPlayerKills.SetText(_data.PlayerKills.ToString());
            txtPlayerAssists.SetText(_data.PlayerAssists.ToString());
            txtPlayerDeaths.SetText(_data.PlayerDeaths.ToString());
            txtPlayerPoints.SetText(_data.PlayerPoints.ToString());
        }
    }
}