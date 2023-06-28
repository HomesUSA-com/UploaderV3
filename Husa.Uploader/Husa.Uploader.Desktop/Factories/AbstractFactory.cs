namespace Husa.Uploader.Desktop.Factories
{
    using System;

    public class AbstractFactory<T> : IAbstractFactory<T>
    {
        private readonly Func<T> factory;

        public AbstractFactory(Func<T> factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public T Create()
        {
            return this.factory();
        }
    }
}
