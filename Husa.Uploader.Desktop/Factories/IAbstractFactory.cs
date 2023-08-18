namespace Husa.Uploader.Desktop.Factories
{
    public interface IAbstractFactory<out T>
    {
        T Create();
    }
}
