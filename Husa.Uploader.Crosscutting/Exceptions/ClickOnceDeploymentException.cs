namespace Husa.Uploader.Crosscutting.Exceptions
{
    using System;

    public class ClickOnceDeploymentException : Exception
    {
        public ClickOnceDeploymentException()
        {
        }

        public ClickOnceDeploymentException(string message)
            : base(message)
        {
        }

        public ClickOnceDeploymentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
