namespace Husa.Uploader.Core.Tests
{
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Crosscutting.Tests;
    using Microsoft.Extensions.Options;

    public class ApplicationServicesFixture
    {
        public ApplicationServicesFixture()
        {
            this.ApplicationOptions = Options.Create(TestProvider.GetApplicationOptions());
        }

        public IOptions<ApplicationOptions> ApplicationOptions { get; set; }
    }
}
