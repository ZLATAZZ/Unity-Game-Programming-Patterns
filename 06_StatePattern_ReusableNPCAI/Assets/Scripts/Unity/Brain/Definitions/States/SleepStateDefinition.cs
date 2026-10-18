using System;
using CursedForest.Core.FSM;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Behaviors;
using CursedForest.Unity.Brain.Runtime;
using UnityEngine;

namespace CursedForest.Unity.Brain.Definitions.States
{
    [CreateAssetMenu(fileName = "SleepState", menuName = "Cursed Forest/AI/States/Sleep")]
    public sealed class SleepStateDefinition : StateDefinition
    {
        [SerializeField, Min(0.01f)] private float _recoveryPerSecond = 10f;

        public override IState<AgentContext> CreateState(AgentRuntimeDependencies dependencies)
        {
            if (dependencies == null)
            {
                throw new ArgumentNullException(nameof(dependencies));
            }

            if (dependencies.Energy == null)
            {
                throw new InvalidOperationException("Sleep state requires IAgentEnergy on its agent.");
            }

            return new SleepState(Id, dependencies.Energy, _recoveryPerSecond);
        }
    }
}
