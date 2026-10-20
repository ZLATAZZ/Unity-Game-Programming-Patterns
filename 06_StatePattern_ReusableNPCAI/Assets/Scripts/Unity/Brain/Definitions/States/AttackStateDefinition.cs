using System;
using CursedForest.Core.FSM;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Behaviors;
using CursedForest.Unity.Brain.Runtime;
using UnityEngine;

namespace CursedForest.Unity.Brain.Definitions.States
{
    [CreateAssetMenu(fileName = "AttackState", menuName = "Cursed Forest/AI/States/Attack")]
    public sealed class AttackStateDefinition : StateDefinition
    {
        public override IState<AgentContext> CreateState(AgentRuntimeDependencies dependencies)
        {
            if (dependencies == null)
            {
                throw new ArgumentNullException(nameof(dependencies));
            }

            if (dependencies.Combat == null)
            {
                throw new InvalidOperationException("Attack state requires IAgentCombat on its agent.");
            }

            return new AttackState(Id, dependencies.Combat);
        }
    }
}
