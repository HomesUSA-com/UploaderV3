namespace Husa.Uploader.Crosscutting.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Husa.Uploader.Crosscutting.Interfaces;

    public class SleepService : ISleepService
    {
        public void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        public void Sleep(TimeSpan duration)
        {
            Thread.Sleep(duration);
        }

        public async Task SleepAsync(int milliseconds, CancellationToken cancellationToken)
        {
            await Task.Delay(milliseconds, cancellationToken);
        }
    }
}
