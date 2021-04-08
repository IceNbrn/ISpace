using System;
using TMPro;
using UnityEngine;

namespace UI.ScoreBoard
{
    public class ScoreRow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtPlayerName;
        [SerializeField] private TextMeshProUGUI txtPlayerKills;
        [SerializeField] private TextMeshProUGUI txtPlayerAssists;
        [SerializeField] private TextMeshProUGUI txtPlayerDeaths;
        [SerializeField] private TextMeshProUGUI txtPlayerPoints;

        private string _playerName;
        private int _playerKills;
        private int _playerAssists;
        private int _playerDeaths;
        private int _playerPoints;

        public void SetRowData(string name, int kills, int assists, int deaths, int points)
        {
            _playerName = name;
            _playerKills = kills;
            _playerAssists = assists;
            _playerDeaths = deaths;
            _playerPoints = points;
        }

        private void OnGUI()
        {
            txtPlayerName.SetText(_playerName);
            txtPlayerKills.SetText(_playerKills.ToString());
            txtPlayerAssists.SetText(_playerAssists.ToString());
            txtPlayerDeaths.SetText(_playerDeaths.ToString());
            txtPlayerPoints.SetText(_playerPoints.ToString());
        }
    }
}