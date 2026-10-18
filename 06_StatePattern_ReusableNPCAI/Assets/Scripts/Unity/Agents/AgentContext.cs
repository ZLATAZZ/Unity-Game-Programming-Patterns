
using System;
using CursedForest.Unity.Agents.Capabilities;

namespace CursedForest.Unity.Agents
{
    public sealed class AgentContext
    {
        public IAgentMovement Movement { get; }
        public IAgentPerception Perception { get; }

        public AgentContext(IAgentMovement movement, IAgentPerception perception)
        {
            if (movement == null)
            {
                throw new ArgumentNullException(nameof(movement));
            }

            if (perception == null)
            {
                throw new ArgumentNullException(nameof(perception));
            }

            Movement = movement;
            Perception = perception;
        }
    }
}
