using UnityEngine;

namespace UI
{
    public class MainUIManager : MonoBehaviour
    {
        [SerializeField] 
        private GameObject mainMenuPanel, serversPanel;
        
        // Start is called before the first frame update
        void Start()
        {
            MainMenuPanel();
        }

        public void PlayBtn()
        {
            ServersPanel();
        }

        public void OptionsBtn()
        {
            
        }

        public void QuitBtn()
        {
            // TODO: Make sure wants to quit
            Application.Quit();
        }
        
        private void MainMenuPanel()
        {
            serversPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }

        private void ServersPanel()
        {
            mainMenuPanel.SetActive(false);
            serversPanel.SetActive(true);
        }
    }
}
