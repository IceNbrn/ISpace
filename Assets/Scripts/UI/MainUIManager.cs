using System.Collections;
using Mirror;
using TMPro;
using UnityEngine;

namespace UI
{
    public class MainUIManager : MonoBehaviour
    {
        [SerializeField]
        private SpaceNetworkManager networkManager;
        
        [SerializeField] 
        private GameObject mainMenuPanel, serversPanel;

        [SerializeField] 
        private GameObject serversListPanel, directIpPanel;

        [SerializeField] private TMP_InputField ipAddressInput;
        
        // Start is called before the first frame update
        void Start()
        {
            MainMenuPanel();
        }

        // Buttons
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

        public void HostBtn(GameObject toolTip = null)
        {
            DisableToolTip(toolTip);
            networkManager.StartHost();
        }

        public void JoinBtn(GameObject toolTip = null)
        {
            DisableToolTip(toolTip);

            string ip = ipAddressInput.text;
            //networkManager.networkAddress = "192.168.1.195";
            networkManager.networkAddress = ip;
            //networkManager.networkAddress = ipAddressInput.text;
            
            networkManager.StartClient();
        }
        

        // Panels
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
