using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using TMPro;
using UI;
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
        [SerializeField] private GameObject _canvas;
        [SerializeField] private GameObject commandRowPrefab;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private ScrollViewContent scrollViewContent;
        
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
        public static ConsoleCommand CMD_CLEAR;
        public static ConsoleCommand CMD_QUIT;

        [SerializeField] private int maxInputsHistory = 20;
        private Queue<string> _inputsHistory;
        private int _inputIndex;
        
        private void Awake()
        {
            if (!InitializeSingleton()) 
                return;
            
            _controls = PlayerInputs.PlayerControls;
            PlayerInActions.DevConsoleActions devConsoleActions = _controls.DevConsole;
            devConsoleActions.Enable();
            devConsoleActions.Interaction.performed += ToggleConsole;
            devConsoleActions.HistoryUp.performed += HistoryUpOnperformed;
            devConsoleActions.HistoryDown.performed += HistoryDownOnperformed;
            
            
            _inputsHistory = new Queue<string>(maxInputsHistory); 
            
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
                if (value > 0.0f)
                    _gameManager.SetSensitivity(value);
                else
                    ShowCommandOutput(CMD_PLAYER_SENSITIVITY.CommandId, _gameManager.GetCameraSensitivity().ToString(), CMD_PLAYER_SENSITIVITY.CommandDescription);
            });
            
            CMD_ADD_SCORE_ROW = new ConsoleCommand("addScoreRow", "adds score row", "addScoreRow",() =>
            {
                
            });
            
            CMD_CROSSHAIR_THICKNESS = new ConsoleCommand<float>("crosshair_thickness", "Sets the player crosshair thickness", "crosshair_thickness <value>",(value) =>
            {
                if (value > 0.0f)
                {
                    CrosshairSettings crosshairSettings = new CrosshairSettings() {Thickness = value};
                    _gameManager.UpdateCrosshair(crosshairSettings);
                }
                else
                {
                    ShowCommandOutput(CMD_CROSSHAIR_THICKNESS.CommandId, _gameManager.PlayerSettings.CrosshairSettings.Thickness.ToString(), CMD_CROSSHAIR_THICKNESS.CommandDescription);
                }
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
            
            CMD_CLEAR = new ConsoleCommand("clear", "Clears the console", "clear",() =>
            {
                scrollViewContent.DeleteAllContent();
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
                CMD_CLEAR,
                CMD_QUIT
            };
        }

        public void HandleInput()
        {
            string inputText = input.text.ToLower();
            
            // Add the input to the input history
            /*if (_inputsHistory.Count >= maxInputsHistory)
                _inputsHistory.Dequeue();*/
            
            if(!inputText.Equals(string.Empty))
                _inputsHistory.Enqueue(inputText);

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
                    case ConsoleCommand<int> consoleCommand when IsValidCommand(ref args):
                    {
                        int value = int.Parse(args[1]);
                        consoleCommand?.Invoke(value);
                        return true;
                    }
                    case ConsoleCommand<float> consoleCommand:
                    {
                        float value = 0.0f;
                        if (IsValidCommand(ref args))
                        {
                            string args1 = args[1];
                            args1 = args1.Replace(".", ",");
                            value = float.Parse(args1);
                        }
                        
                        consoleCommand?.Invoke(value);
                        return true;
                    }
                    case ConsoleCommand<string> consoleCommand when IsValidCommand(ref args):
                    {
                        string value = args[1];
                        consoleCommand?.Invoke(value);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsValidCommand(ref string[] args)
        {
            bool argsValid = false;
            if (args.Length > 1)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    argsValid = args[i] != null || args[i] != string.Empty;
                    if (!argsValid)
                        return false;
                }
            }
            return argsValid;
        }

        private void ShowCommandOutput(string title, string value, string description)
        {
            GameObject instantiatedCommand = scrollViewContent.AddContent(commandRowPrefab);
            CommandOutput command = instantiatedCommand.GetComponent<CommandOutput>();
            command.SetTexts(title, value, description);
        }

        private void ToggleConsole(InputAction.CallbackContext obj)
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
        
        private void HistoryUpOnperformed(InputAction.CallbackContext obj)
        {
            string inputHistory = _inputsHistory.ElementAt((_inputsHistory.Count - 1) - _inputIndex);
            input.text = inputHistory;
            if (_inputIndex + 1 < _inputsHistory.Count)
                _inputIndex++;
        }
        
        private void HistoryDownOnperformed(InputAction.CallbackContext obj)
        {
            if (_inputIndex - 1 >= 0)
                _inputIndex--;
            input.text = _inputsHistory.ElementAt(_inputIndex);
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