using UnityEngine;

namespace CursedForest.Unity.Agents.Capabilities
{
    public interface IAgentMovement
    {
        Vector3 Position { get; }
        bool HasReachedDestination { get; }

        void MoveTo(Vector3 destination);
        void Stop();
    }
}