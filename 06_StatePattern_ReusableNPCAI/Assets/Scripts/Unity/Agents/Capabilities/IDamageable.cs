using System;
using UnityEngine;

namespace CursedForest.Unity.Agents.Capabilities
{
    public interface IDamageable
    {
        bool IsAlive { get; }

        event Action<Transform> Damaged;
        event Action Died;

        void TakeDamage(float amount, Transform attacker);
    }
}