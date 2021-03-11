using TMPro;
using UnityEngine;

namespace UI
{
    public class MainUIManager : MonoBehaviour
    {
        [SerializeField] 
        private GameObject mainMenuPanel, serversPanel;

        [SerializeField] 
        private GameObject serversListPanel, directIpPanel;
        
        // Start is called before the first frame update
        void Start()
        {
            MainMenuPanel();
        }

        public void PlayBtn(GameObject toolTip = null)
        {
            DisableToolTip(toolTip);
            ServersPanel();
        }

        public void OptionsBtn(GameObject toolTip = null)
        {
            DisableToolTip(toolTip);
        }

        public void BackBtn(GameObject toolTip = null)
        {
            DisableToolTip(toolTip);
            MainMenuPanel();
        }

        public void ServerListBtn(GameObject toolTip = null)
        {
            DisableToolTip(toolTip);
            ServerListPanel();
        }
        
        public void DirectIpBtn(GameObject toolTip = null)
        {
            DisableToolTip(toolTip);
            DirectPanel();
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

        private void DirectPanel()
        {
            serversListPanel.SetActive(false);
            directIpPanel.SetActive(true);
        }
        
        private void ServerListPanel()
        {
            directIpPanel.SetActive(false);
            serversListPanel.SetActive(true);
        }

        private void DisableToolTip(GameObject toolTip)
        {
            if(toolTip != null && toolTip.activeSelf)
                toolTip.SetActive(false);
        }
    }
}
