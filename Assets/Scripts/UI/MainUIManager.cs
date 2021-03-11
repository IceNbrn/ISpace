using TMPro;
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

        public void PlayBtn(GameObject toolTip = null)
        {
            if(toolTip != null)
                DisableToolTip(toolTip);
            ServersPanel();
        }

        public void OptionsBtn(GameObject toolTip)
        {
            DisableToolTip(toolTip);
        }

        public void BackBtn(GameObject toolTip)
        {
            DisableToolTip(toolTip);
            MainMenuPanel();
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

        private void DisableToolTip(GameObject toolTip)
        {
            if(toolTip.activeSelf)
                toolTip.SetActive(false);
        }
    }
}
