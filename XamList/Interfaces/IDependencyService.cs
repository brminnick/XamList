namespace XamList
{
    public interface IDependencyService
    {
        T Get<T>() where T : class;
    }
}
