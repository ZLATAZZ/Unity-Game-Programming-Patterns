using UnityEngine;

namespace CursedForest.Unity.Agents.Capabilities
{
    public interface IEscapePlanner
    {
        bool TryPlanEscape(Vector3 position, Vector3 threatPosition, float distance, out Vector3 destination);
    }
}