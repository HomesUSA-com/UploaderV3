using Husa.Uploader.Commands;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Husa.Uploader.ViewModels
{
    public class MlsnumInputViewModel : ViewModel
    {
        private string mlsNum;

        private ICommand continueCommand;
        private ICommand cancelCommand;

        private bool CanContinue => !string.IsNullOrEmpty(this.mlsNum);

        public string MlsNum
        {
            get { return this.mlsNum; }
            set
            {
                if (value == this.mlsNum)
                {
                    return;
                }

                this.mlsNum = value;
                this.OnPropertyChanged();
            }
        }

        public ICommand ContinueCommand
        {
            get
            {
                this.continueCommand ??= new RelayCommand(param => this.Continue(), param => this.CanContinue);
                return this.continueCommand;
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                this.cancelCommand ??= new RelayCommand(param => this.Cancel(), param => true);
                return this.cancelCommand;
            }
        }

        private void Cancel()
        {
            var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(window => window.DataContext == this);
            if (currentWindow != null)
            {
                currentWindow.DialogResult = false;
            }
        }

        private void Continue()
        {
            var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(window => window.DataContext == this);
            if (currentWindow != null)
            {
                currentWindow.DialogResult = true;
            }
        }
    }
}
