using System;
using System.Collections.Generic;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Agents.Capabilities;
using UnityEngine;

namespace CursedForest.Unity.Brain.Runtime
{
    // Construction-time dependencies only. States retain only what they actually need.
    public sealed class AgentRuntimeDependencies
    {
        private readonly Vector3[] _patrolPoints;

        public AgentContext Context { get; }
        public IAgentCombat Combat { get; }
        public IEscapePlanner EscapePlanner { get; }
        public IAgentEnergy Energy { get; }
        public IReadOnlyList<Vector3> PatrolPoints => _patrolPoints;

        public AgentRuntimeDependencies(
            AgentContext context,
            IAgentCombat combat,
            IEscapePlanner escapePlanner,
            IAgentEnergy energy,
            IReadOnlyList<Vector3> patrolPoints)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Combat = combat;
            EscapePlanner = escapePlanner;
            Energy = energy;

            int count = patrolPoints?.Count ?? 0;
            _patrolPoints = new Vector3[count];

            for (int i = 0; i < count; i++)
            {
                _patrolPoints[i] = patrolPoints[i];
            }
        }
    }
}
