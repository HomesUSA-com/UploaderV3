namespace Husa.Uploader.Crosscutting.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
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

        protected ClickOnceDeploymentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
