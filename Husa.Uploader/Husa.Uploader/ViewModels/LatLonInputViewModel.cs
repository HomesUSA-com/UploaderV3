using Husa.Uploader.Commands;
using Husa.Uploader.Crosscutting.Enums;
using Husa.Uploader.Models;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Husa.Uploader.ViewModels
{
    public class LatLonInputViewModel : ViewModel
    {
        private UiState state;

        private string latitude;

        private string longitude;

        private ICommand continueCommand;
        private ICommand cancelCommand;

        private bool CanContinue => decimal.TryParse(this.Latitude, out _) && decimal.TryParse(this.Longitude, out _);

        private UiState State
        {
            get => this.state;
            set
            {
                this.state = value;
                this.OnPropertyChanged(name: nameof(ShowError));
            }
        }

        public string Latitude
        {
            get => this.latitude;
            set
            {
                if (value == this.latitude)
                {
                    return;
                }

                this.latitude = value;
                OnPropertyChanged();
            }
        }

        public string Longitude
        {
            get => this.longitude;
            set
            {
                if (value == this.longitude)
                {
                    return;
                }

                this.longitude = value;
                OnPropertyChanged();
            }
        }

        public bool ShowError => this.State == UiState.Error;

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

        public void Cancel()
        {
            var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(window => window.DataContext == this);
            if (currentWindow != null)
            {
                currentWindow.DialogResult = false;
            }
        }

        public void Continue()
        {
            var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(window => window.DataContext == this);
            ////if (decimal.TryParse(this.Latitude, out decimal value) && decimal.TryParse(this.Longitude, out value))
            if (currentWindow != null)
            {
                currentWindow.DialogResult = true;
            }
            else
            {
                this.State = UiState.Error;
                OnPropertyChanged(name: nameof(ShowError));
            }
        }

        public LocationInfo GetLocationInfo() => decimal.TryParse(this.latitude, out var lat) && decimal.TryParse(this.longitude, out var lon) ?
            new(lat, lon) :
            new();
    }
}
