namespace Husa.Uploader.Factories
{
    public interface IAbstractFactory<out T>
    {
        T Create();
    }
}
