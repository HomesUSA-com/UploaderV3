namespace Husa.Uploader.Crosscutting.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISleepService
    {
        void Sleep(int milliseconds);

        void Sleep(TimeSpan duration);

        Task SleepAsync(int milliseconds, CancellationToken cancellationToken);
    }
}
