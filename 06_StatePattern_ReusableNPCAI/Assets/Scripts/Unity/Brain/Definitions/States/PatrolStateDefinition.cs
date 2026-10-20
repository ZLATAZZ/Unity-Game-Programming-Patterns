using System;
using CursedForest.Core.FSM;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Behaviors;
using CursedForest.Unity.Brain.Runtime;
using UnityEngine;

namespace CursedForest.Unity.Brain.Definitions.States
{
    [CreateAssetMenu(fileName = "PatrolState", menuName = "Cursed Forest/AI/States/Patrol")]
    public sealed class PatrolStateDefinition : StateDefinition
    {
        public override IState<AgentContext> CreateState(AgentRuntimeDependencies dependencies)
        {
            if (dependencies == null)
            {
                throw new ArgumentNullException(nameof(dependencies));
            }

            if (dependencies.PatrolPoints.Count == 0)
            {
                throw new InvalidOperationException("Patrol state requires at least one waypoint on its agent.");
            }

            return new PatrolState(Id, dependencies.PatrolPoints);
        }
    }
}
