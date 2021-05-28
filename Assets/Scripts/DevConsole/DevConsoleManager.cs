using System;
using System.Collections.Generic;
using Player;
using TMPro;
using UI.ScoreBoard;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
        [SerializeField]
        private TMP_InputField input;
        private bool _isConsoleActive;
        
        // Commands
        public List<object> _commands;

        public static ConsoleCommand CMD_HELP;
        public static ConsoleCommand<float> CMD_PLAYER_SENSITIVITY;
        public static ConsoleCommand CMD_ADD_SCORE_ROW;
        public static ConsoleCommand CMD_QUIT;
        
        private void Awake()
        {
            if (!InitializeSingleton()) 
                return;
            
            _controls = PlayerInputs.PlayerControls;
            _controls.UI.DevConsole.Enable();
            _controls.UI.DevConsole.performed += context => ToggleConsole();
            
            InitializeCommands();
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
            
            CMD_ADD_SCORE_ROW = new ConsoleCommand("addScoreRow", "adds score row", "addScoreRow",() =>
            {
                ScoreRowData rowData = new ScoreRowData("Player", 1, 2, 3, 4); 
                ScoreBoardManager.Singleton.AddRow(rowData);
                Debug.Log("addRow");
            });
            
            
            CMD_QUIT = new ConsoleCommand("quit", "Quits game", "quit",() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
            
            _commands = new List<object>()
            {
                CMD_HELP,
                CMD_PLAYER_SENSITIVITY,
                CMD_ADD_SCORE_ROW,
                CMD_QUIT
            };
        }

        public void HandleInput()
        {
            string inputText = input.text.ToLower();
            Debug.Log($"Input text: {inputText}");
            string[] args = inputText.Split(' ');

            for (int i = 0; i < _commands.Count; ++i)
            {
                object command = _commands[i];
                if (RunCommand(ref command, inputText, ref args))
                    break;
            }
            input.Select();
            input.ActivateInputField();
            input.text = String.Empty;
        }

        private bool RunCommand(ref object command, string inputText, ref string[] args)
        {
            Debug.Log($"Run Command {inputText}");
            ConsoleCommandBase commandBase = command as ConsoleCommandBase;
            if (commandBase != null && inputText.Contains(commandBase.CommandId.ToLower()))
            {
                switch (command)
                {
                    case ConsoleCommand consoleCommand:
                    {
                        consoleCommand?.Invoke();
                        return true;
                    }
                    case ConsoleCommand<int> consoleCommand:
                    {
                        int value = int.Parse(args[1]);
                        consoleCommand?.Invoke(value);
                        return true;
                    }
                    case ConsoleCommand<float> consoleCommand:
                    {
                        string args1 = args[1];
                        args1 = args1.Replace(".", ",");
                        float value = float.Parse(args1);
                        consoleCommand?.Invoke(value);
                        return true;
                    }
                }
            }
            return false;
        }

        private void ToggleConsole()
        {
            _isConsoleActive = !_isConsoleActive;
            LockPlayer(_isConsoleActive);
            _canvas.SetActive(_isConsoleActive);

            if (_isConsoleActive)
            {
                input.Select();
                input.ActivateInputField();
            }
            
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
                if (SceneManager.GetActiveScene().name.Contains("Game"))
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                _controls.Player.Enable();
            }
                
        }
        
    }
}