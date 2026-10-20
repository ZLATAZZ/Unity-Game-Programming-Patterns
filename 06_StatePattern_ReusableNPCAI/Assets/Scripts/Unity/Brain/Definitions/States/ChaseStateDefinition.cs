using System;
using CursedForest.Core.FSM;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Behaviors;
using CursedForest.Unity.Brain.Runtime;
using UnityEngine;

namespace CursedForest.Unity.Brain.Definitions.States
{
    [CreateAssetMenu(fileName = "ChaseState", menuName = "Cursed Forest/AI/States/Chase")]
    public sealed class ChaseStateDefinition : StateDefinition
    {
        [SerializeField, Min(0.01f)] private float _repathInterval = 0.2f;
        [SerializeField, Min(0f)] private float _minimumTargetMovement = 0.25f;

        public override IState<AgentContext> CreateState(AgentRuntimeDependencies dependencies)
        {
            if (dependencies == null)
            {
                throw new ArgumentNullException(nameof(dependencies));
            }

            return new ChaseState(Id, _repathInterval, _minimumTargetMovement);
        }
    }
}
