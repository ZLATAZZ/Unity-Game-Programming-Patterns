using UnityEngine;

namespace CursedForest.Unity.Agents.Capabilities
{
    public interface IAgentCombat
    {
        bool IsInRange(Transform target);
        void TryAttack(Transform target);
    }
}