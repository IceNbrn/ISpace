using System;
using System.Collections;
using Game.Managers;
using TMPro;
using UnityEngine;
using Utils;

namespace UI
{
    public class RoundUIManager : MonoBehaviour
    {
        [SerializeField] private float timeOnScreen = 3;
        [SerializeField] private TextMeshProUGUI txtWinner;
        [SerializeField] private TextMeshProUGUI txtScore;
        [SerializeField] private TextMeshProUGUI txtTimer;
        [SerializeField] private GameObject panel;
        
        public static RoundUIManager Singleton { get; private set; }

        private void Awake()
        {
            if (!InitializeSingleton()) 
                return;

            GameModeManager.OnGameRoundStarted += OnGameRoundStarted;
        }

        public void OnGameRoundStarted(int time)
        {
            StartCoroutine(StartTimerCoroutine(time));
        }

        private IEnumerator StartTimerCoroutine(int roundTime)
        {
            for (int time = roundTime; time >= 0; time--)
            {
                Debug.Log($"RoundTimeLeft: {time}");
                SetTxtTimer(time);
                yield return new WaitForSeconds(1);
            }
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

        public void SetTxtTimer(int seconds)
        {
            if (txtTimer == null)
                return;
            
            float minutes = (float) seconds / 60;
            float secondsMin = (float) (minutes - Math.Truncate(minutes));
            secondsMin *= 60;

            if (seconds < 60)
                minutes = 0;
            txtTimer.SetText($"{Math.Truncate(minutes)}:{secondsMin:00}");
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