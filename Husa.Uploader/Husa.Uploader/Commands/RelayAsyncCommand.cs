namespace Husa.Uploader.Commands
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public class RelayAsyncCommand : ICommand
    {
        private readonly Func<object, Task> execute;
        private readonly Predicate<object> canExecute;

        public RelayAsyncCommand(Func<object, Task> execute, Predicate<object> canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => this.canExecute == null || this.canExecute(parameter);

        public async void Execute(object parameter)
        {
            await this.execute(parameter);
        }
    }
}
