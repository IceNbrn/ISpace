using System;
using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DevConsole
{
    public class DevConsoleManager : MonoBehaviour
    {
        public static DevConsoleManager Singleton { get; private set; }

        private PlayerInActions _controls;
        
        // UI
        [SerializeField]
        private GameObject _canvas;
        private bool _isConsoleActive;
        
        // Commands
        public List<object> _commands;

        public static ConsoleCommand CMD_HELP;
        public static ConsoleCommand<float> CMD_PLAYER_SENSITIVITY;
        
        private void Awake()
        {
            if (!InitializeSingleton()) 
                return;
            
            _controls = PlayerInputs.PlayerControls;
            _controls.UI.DevConsole.Enable();
            _controls.UI.DevConsole.performed += context => ToggleConsole();
            
            InitializeCommands();
        }

        private void OnDisable()
        {
            _controls.UI.DevConsole.Disable();
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

        private void InitializeCommands()
        {
            CMD_HELP = new ConsoleCommand("help", "Shows a list of commands", "help", () =>
            {
                Debug.Log("help test");
            });
            
            CMD_PLAYER_SENSITIVITY = new ConsoleCommand<float>("sensitivity", "Sets the player mouse sensitivity", "sensitivity <value>",(value) =>
            {
                GameManager.Singleton.SetSensitivity(value);
            });
            
            _commands = new List<object>()
            {
                CMD_HELP,
                CMD_PLAYER_SENSITIVITY
            };
        }

        public void HandleInput(TMP_InputField input)
        {
            string inputText = input.text.ToLower();
            Debug.Log($"Input text: {inputText}");
            string[] args = inputText.Split(' ');

            for (int i = 0; i < _commands.Count; ++i)
            {
                object command = _commands[i];
                if (command is ConsoleCommandBase commandBase && inputText.Contains(commandBase.CommandId))
                {
                    if (command is ConsoleCommand)
                    {
                        ((ConsoleCommand)command)?.Invoke();
                    }
                    else if (command is ConsoleCommand<int>)
                    {
                        int value = int.Parse(args[1]);
                        ((ConsoleCommand<int>)command)?.Invoke(value);
                    }
                    else if (command is ConsoleCommand<float>)
                    {
                        string args1 = args[1];
                        args1 = args1.Replace(".", ",");
                        float value = float.Parse(args1);
                        ((ConsoleCommand<float>)command)?.Invoke(value);
                    }
                }
            }
        }

        private void ToggleConsole()
        {
            _isConsoleActive = !_isConsoleActive;
            LockPlayer(_isConsoleActive);
            _canvas.SetActive(_isConsoleActive);
            
        }

        private void LockPlayer(bool value)
        {
            if (value)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                _controls.Player.Disable();
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                _controls.Player.Enable();
            }
                
        }
        
    }
}