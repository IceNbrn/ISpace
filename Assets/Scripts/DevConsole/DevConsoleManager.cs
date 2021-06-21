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

        private GameManager _gameManager;
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
        public static ConsoleCommand<float> CMD_CROSSHAIR_THICKNESS;
        public static ConsoleCommand<float> CMD_CROSSHAIR_SIZE;
        public static ConsoleCommand<float> CMD_CROSSHAIR_GAP;
        public static ConsoleCommand<int> CMD_CROSSHAIR_TYPE;
        public static ConsoleCommand<string> CMD_CROSSHAIR_COLOR;
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

        private void Start()
        {
            _gameManager = GameManager.Singleton;
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
                _gameManager.SetSensitivity(value);
            });
            
            CMD_ADD_SCORE_ROW = new ConsoleCommand("addScoreRow", "adds score row", "addScoreRow",() =>
            {
                
            });
            
            CMD_CROSSHAIR_THICKNESS = new ConsoleCommand<float>("crosshair_thickness", "Sets the player crosshair thickness", "crosshair_thickness <value>",(value) =>
            {
                CrosshairSettings crosshairSettings = new CrosshairSettings() {Thickness = value};
                _gameManager.UpdateCrosshair(crosshairSettings);
            });
            
            CMD_CROSSHAIR_GAP = new ConsoleCommand<float>("crosshair_gap", "Sets the player crosshair gap", "crosshair_gap <value>",(value) =>
            {
                CrosshairSettings crosshairSettings = new CrosshairSettings() {Gap = value};
                _gameManager.UpdateCrosshair(crosshairSettings);
            });
            
            CMD_CROSSHAIR_SIZE = new ConsoleCommand<float>("crosshair_size", "Sets the player crosshair size", "crosshair_size <value>",(value) =>
            {
                CrosshairSettings crosshairSettings = new CrosshairSettings() {Size = value};
                _gameManager.UpdateCrosshair(crosshairSettings);
            });
            
            CMD_CROSSHAIR_TYPE = new ConsoleCommand<int>("crosshair_type", "Sets the player crosshair type", "crosshair_type <value>",(value) =>
            {
                CrosshairSettings crosshairSettings = new CrosshairSettings() {Type = (ECrosshairType) value};
                _gameManager.UpdateCrosshair(crosshairSettings);
            });
            
            CMD_CROSSHAIR_COLOR = new ConsoleCommand<string>("crosshair_color", "Sets the player crosshair color", "crosshair_color <r/g/b>",(value) =>
            {
                // CMD-> crosshair_color 1.0/1.0/1.0
                string[] values = value.Split('/');
                for (int i = 0; i < 3; i++)
                    values[i] = values[i].Replace('.', ',');
                
                Color color = new Color(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
                
                CrosshairSettings crosshairSettings = new CrosshairSettings() {Color = color};
                _gameManager.UpdateCrosshair(crosshairSettings);
            });
            
            CMD_QUIT = new ConsoleCommand("quit", "Quits game", "quit",() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit(0);
#endif
            });
            
            _commands = new List<object>()
            {
                CMD_HELP,
                CMD_PLAYER_SENSITIVITY,
                CMD_ADD_SCORE_ROW,
                CMD_CROSSHAIR_THICKNESS,
                CMD_CROSSHAIR_SIZE,
                CMD_CROSSHAIR_GAP,
                CMD_CROSSHAIR_TYPE,
                CMD_CROSSHAIR_COLOR,
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
                    case ConsoleCommand<string> consoleCommand:
                    {
                        string value = args[1];
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