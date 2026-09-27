namespace BloodMoon.Core
{
    public interface IBloodMoonObserver
    {
        void Synchronize(bool isBloodMoonActive);
        void OnBloodMoonStarted();
        void OnBloodMoonEnded();
    }
}