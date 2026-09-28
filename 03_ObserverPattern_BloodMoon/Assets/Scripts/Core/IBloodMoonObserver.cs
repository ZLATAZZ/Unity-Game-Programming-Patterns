namespace BloodMoon.Core
{
    public interface IBloodMoonObserver
    {
        void OnBloodMoonStarted();
        void OnBloodMoonEnded();
    }
}