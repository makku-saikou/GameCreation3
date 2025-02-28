namespace Hmxs.Toolkit
{
    public interface IPool<T> where T : class
    {
        T Get();
        void Release(T element);
        void Dispose();
    }
}