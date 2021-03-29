using System;

namespace DevConsole
{
    public class ConsoleCommandBase
    {
        private string _commandId;
        private string _commandDescription;
        private string _commandFormat;
        private string _commandOutput;

        public string CommandId          => _commandId;
        public string CommandDescription => _commandDescription;
        public string CommandFormat      => _commandFormat;
        public string CommandOutput      => _commandOutput;

        public ConsoleCommandBase(string commandId, string commandDescription, string commandFormat,
            string commandOutput)
        {
            _commandId          = commandId;
            _commandDescription = commandDescription;
            _commandFormat      = commandFormat;
            _commandOutput      = commandOutput;
        }
    }

    public class ConsoleCommand : ConsoleCommandBase
    {
        private Action _command;

        public ConsoleCommand(string commandId, string commandDescription, string commandFormat, Action command ,string commandOutput = null) : base(commandId, commandDescription, commandFormat, commandOutput)
        {
            _command = command;
        }

        public void Invoke()
        {
            _command.Invoke();
        }
    }
    
    public class ConsoleCommand<T> : ConsoleCommandBase
    {
        private Action<T> _command;

        public ConsoleCommand(string commandId, string commandDescription, string commandFormat, Action<T> command ,string commandOutput = null) : base(commandId, commandDescription, commandFormat, commandOutput)
        {
            _command = command;
        }

        public void Invoke(T value)
        {
            _command.Invoke(value);
        }
    }
}