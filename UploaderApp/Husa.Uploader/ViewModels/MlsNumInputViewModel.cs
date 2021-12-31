// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Husa.Uploader
{
    public class MlsNumInputViewModel : Caliburn.Micro.Screen
    {
        private string _mlsNum;

        public string MlsNum
        {
            get { return _mlsNum; }
            set
            {
                if (value == _mlsNum) return;
                _mlsNum = value;
                NotifyOfPropertyChange(() => MlsNum);
            }
        }

        public void Cancel()
        {
            this.TryClose(false);
        }

        public void Continue()
        {
            this.TryClose(true);
        }
    }
}