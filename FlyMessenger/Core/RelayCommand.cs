using System;
using System.Windows.Input;

namespace FlyMessenger.Core
{
    /// <summary>
    /// Relay command.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="execute"> Action to execute. </param>
        /// <param name="canExecute"> Predicate to check if action can be executed. </param>
        /// <exception cref="ArgumentNullException"> Thrown when execute is null. </exception>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute) 
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Can execute command.
        /// </summary>
        /// <param name="parameter"> Command parameter. </param>
        /// <returns> True if command can be executed, false otherwise. </returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute(parameter!);
        }

        // Event handler for command execution.
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        // Execute command.
        public void Execute(object? parameter)
        {
            _execute(parameter!);
        }
    }
}
