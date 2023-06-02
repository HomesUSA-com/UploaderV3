namespace Husa.Uploader.Desktop.Deployment
{
    using System;

    public class UpdateCheckInfo
    {
        private readonly bool updateAvailable;

        private readonly Version availableVersion;

        private readonly bool isUpdateRequired;

        private readonly Version minimumRequiredVersion;

        private readonly long updateSize;

        internal UpdateCheckInfo(bool updateAvailable, Version availableVersion, bool isUpdateRequired, Version minimumRequiredVersion, long updateSize)
        {
            this.updateAvailable = updateAvailable;
            this.availableVersion = availableVersion;
            this.isUpdateRequired = isUpdateRequired;
            this.minimumRequiredVersion = minimumRequiredVersion;
            this.updateSize = updateSize;
        }

        public bool UpdateAvailable => this.updateAvailable;

        public Version AvailableVersion
        {
            get
            {
                this.RaiseExceptionIfUpdateNotAvailable();
                return this.availableVersion;
            }
        }

        public bool IsUpdateRequired
        {
            get
            {
                this.RaiseExceptionIfUpdateNotAvailable();
                return this.isUpdateRequired;
            }
        }

        public Version MinimumRequiredVersion
        {
            get
            {
                this.RaiseExceptionIfUpdateNotAvailable();
                return this.minimumRequiredVersion;
            }
        }

        public long UpdateSizeBytes
        {
            get
            {
                this.RaiseExceptionIfUpdateNotAvailable();
                return this.updateSize;
            }
        }

        private void RaiseExceptionIfUpdateNotAvailable()
        {
            if (!this.UpdateAvailable)
            {
                throw new InvalidOperationException("Update not available.");
            }
        }
    }
}
