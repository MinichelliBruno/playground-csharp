namespace GenericsLab;

public interface IPool<out T> where T : IResettable
{
    T Rent();
}