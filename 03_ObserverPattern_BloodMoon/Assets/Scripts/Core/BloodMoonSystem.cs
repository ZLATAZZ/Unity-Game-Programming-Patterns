using System;

namespace BloodMoon.Core
{
    public sealed class BloodMoonSystem
    {
        public bool IsActive { get; private set; }

        public event Action Started;
        public event Action Ended;

        public bool TryStart()
        {
            if (IsActive)
            {
                return false;
            }

            IsActive = true;

            Started?.Invoke();

            return true;
        }

        public bool TryEnd()
        {
            if (!IsActive)
            {
                return false;
            }

            IsActive = false;

            Ended?.Invoke();

            return true;
        }
    }
}