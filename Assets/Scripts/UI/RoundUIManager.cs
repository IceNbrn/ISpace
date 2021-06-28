using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Utils;

namespace UI
{
    public class RoundUIManager : Singleton<RoundUIManager>
    {
        [SerializeField] private float timeOnScreen = 3;
        [SerializeField] private TextMeshProUGUI txtWinner;
        [SerializeField] private TextMeshProUGUI txtScore;
        [SerializeField] private GameObject panel;

        public void SetWinnerText(string text, int scoreA, int scoreB)
        {
            txtWinner.SetText(text);
            txtScore.SetText($"{scoreA} - .{scoreB}");
            DisplayText();
        }

        public void SetWinnerText(string text, int kills)
        {
            txtWinner.SetText(text);
            txtScore.SetText($"{kills} kills");
            DisplayText();
        }

        private void DisplayText()
        {
            StartCoroutine(DisplayTextCoroutine());
        }

        private IEnumerator DisplayTextCoroutine()
        {
            panel.SetActive(true);
            yield return new WaitForSeconds(timeOnScreen);
            panel.SetActive(false);
        }
    }
}