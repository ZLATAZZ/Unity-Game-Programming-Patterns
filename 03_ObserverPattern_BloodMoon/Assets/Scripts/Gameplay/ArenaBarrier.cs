using BloodMoon.Core;
using BloodMoon.Presentation;
using UnityEngine;

namespace BloodMoon.Gameplay
{
    public sealed class ArenaBarrier : MonoBehaviour, IBloodMoonObserver
    {
        [SerializeField] private ArenaBarrierView _view;

        public void Synchronize(bool isBloodMoonActive)
        {
            if (isBloodMoonActive)
            {
                _view.CloseImmediate();
                return;
            }

            _view.OpenImmediate();
        }

        public void OnBloodMoonStarted()
        {
            _view.Close();
        }

        public void OnBloodMoonEnded()
        {
            _view.Open();
        }
    }
}