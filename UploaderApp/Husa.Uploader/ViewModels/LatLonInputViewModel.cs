
using Caliburn.Micro;
using Husa.Core.UploaderBase;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Husa.Uploader
{
    public class LatLonInputViewModel : Caliburn.Micro.Screen
    {

        private UiState _state;

        private string _latitude;

        private string _longitude;

        private UiState State
        {
            get { return _state; }
            set
            {
                _state = value;
                NotifyOfPropertyChange(() => ShowError);
            }
        }

        public string Latitude
        {
            get { return _latitude; }
            set
            {
                if (value == _latitude) return;
                _latitude = value;
                NotifyOfPropertyChange(() => Latitude);
            }
        }

        public string Longitude
        {
            get { return _longitude; }
            set
            {
                if (value == _longitude) return;
                _longitude = value;
                NotifyOfPropertyChange(() => Longitude);
            }
        }

        public bool ShowError { get { return _state == UiState.Error; } }

        public void Cancel()
        {
            this.TryClose(false);
        }

        public void Continue()
        {
            decimal value;
            if (Decimal.TryParse(Latitude, out value) && Decimal.TryParse(Longitude, out value))
            {
                this.TryClose(true);
            } else
            {
                _state = UiState.Error;
                NotifyOfPropertyChange(() => ShowError);
            }

        }

    }

}
