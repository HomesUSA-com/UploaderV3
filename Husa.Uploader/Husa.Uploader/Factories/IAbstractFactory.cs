namespace Husa.Uploader.Factories
{
    public interface IAbstractFactory<T>
    {
        T Create();
    }
}