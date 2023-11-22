namespace Husa.Uploader.Data.Tests
{
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Crosscutting.Tests;
    using Microsoft.Extensions.Options;
    using Moq;

    public class ApplicationServicesFixture
    {
        public ApplicationServicesFixture()
        {
            this.ApplicationOptions = new Mock<IOptions<ApplicationOptions>>();

            this.ApplicationOptions
                .SetupGet(o => o.Value)
                .Returns(TestProvider.GetApplicationOptions());
        }

        public Mock<IOptions<ApplicationOptions>> ApplicationOptions { get; set; }
    }
}
