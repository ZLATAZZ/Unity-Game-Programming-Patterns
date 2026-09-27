namespace BloodMoon.Pooling
{
    public interface IPoolable
    {
        void OnRent();
        void OnReturn();
    }
}