using UnityEngine;

namespace CursedForest.Unity.Agents.Capabilities
{
    public interface IAgentPerception
    {
        Transform CurrentTarget { get; }
    }
}