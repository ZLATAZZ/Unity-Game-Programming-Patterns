using System;
using CursedForest.Core.FSM;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Behaviors;
using CursedForest.Unity.Brain.Runtime;
using UnityEngine;

namespace CursedForest.Unity.Brain.Definitions.States
{
    [CreateAssetMenu(fileName = "FleeState", menuName = "Cursed Forest/AI/States/Flee")]
    public sealed class FleeStateDefinition : StateDefinition
    {
        [SerializeField, Min(0.1f)] private float _escapeDistance = 8f;
        [SerializeField, Min(0.01f)] private float _repathInterval = 0.5f;

        public override IState<AgentContext> CreateState(AgentRuntimeDependencies dependencies)
        {
            if (dependencies == null)
            {
                throw new ArgumentNullException(nameof(dependencies));
            }

            if (dependencies.EscapePlanner == null)
            {
                throw new InvalidOperationException("Flee state requires IEscapePlanner on its agent.");
            }

            return new FleeState(Id, dependencies.EscapePlanner, _escapeDistance, _repathInterval);
        }
    }
}
