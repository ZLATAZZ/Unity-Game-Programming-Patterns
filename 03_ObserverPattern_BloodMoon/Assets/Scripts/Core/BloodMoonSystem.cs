using System;
using System.Collections.Generic;

namespace BloodMoon.Core
{
    public sealed class BloodMoonSystem
    {
        private readonly List<IBloodMoonObserver> _observers = new();
        private readonly List<IBloodMoonObserver> _pendingSubscriptions = new();
        private readonly List<IBloodMoonObserver> _pendingUnsubscriptions = new();

        private int _notificationDepth;

        public bool IsActive { get; private set; }

        public void Subscribe(IBloodMoonObserver observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            if (_notificationDepth > 0)
            {
                QueueSubscription(observer);
                return;
            }

            if (_observers.Contains(observer))
            {
                return;
            }

            _observers.Add(observer);
            observer.Synchronize(IsActive);
        }

        public void Unsubscribe(IBloodMoonObserver observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            if (_notificationDepth > 0)
            {
                QueueUnsubscription(observer);
                return;
            }

            _observers.Remove(observer);
        }

        public bool TryStart()
        {
            if (IsActive)
            {
                return false;
            }

            IsActive = true;

            NotifyStarted();

            return true;
        }

        public bool TryEnd()
        {
            if (!IsActive)
            {
                return false;
            }

            IsActive = false;

            NotifyEnded();

            return true;
        }

        private void NotifyStarted()
        {
            BeginNotification();

            try
            {
                for (int i = 0; i < _observers.Count; i++)
                {
                    _observers[i].OnBloodMoonStarted();
                }
            }
            finally
            {
                EndNotification();
            }
        }

        private void NotifyEnded()
        {
            BeginNotification();

            try
            {
                for (int i = 0; i < _observers.Count; i++)
                {
                    _observers[i].OnBloodMoonEnded();
                }
            }
            finally
            {
                EndNotification();
            }
        }

        private void BeginNotification()
        {
            _notificationDepth++;
        }

        private void EndNotification()
        {
            _notificationDepth--;

            if (_notificationDepth == 0)
            {
                ApplyPendingChanges();
            }
        }

        private void QueueSubscription(IBloodMoonObserver observer)
        {
            if (_pendingUnsubscriptions.Remove(observer))
            {
                return;
            }

            if (_observers.Contains(observer) || _pendingSubscriptions.Contains(observer))
            {
                return;
            }

            _pendingSubscriptions.Add(observer);
        }

        private void QueueUnsubscription(IBloodMoonObserver observer)
        {
            if (_pendingSubscriptions.Remove(observer))
            {
                return;
            }

            if (!_observers.Contains(observer) || _pendingUnsubscriptions.Contains(observer))
            {
                return;
            }

            _pendingUnsubscriptions.Add(observer);
        }

        private void ApplyPendingChanges()
        {
            for (int i = 0; i < _pendingUnsubscriptions.Count; i++)
            {
                _observers.Remove(_pendingUnsubscriptions[i]);
            }

            _pendingUnsubscriptions.Clear();

            for (int i = 0; i < _pendingSubscriptions.Count; i++)
            {
                IBloodMoonObserver observer = _pendingSubscriptions[i];

                if (!_observers.Contains(observer))
                {
                    _observers.Add(observer);
                    observer.Synchronize(IsActive);
                }
            }

            _pendingSubscriptions.Clear();
        }
    }
}