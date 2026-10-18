using System;
using System.Collections.Generic;
using CursedForest.Core.FSM;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Brain.Definitions;
using CursedForest.Unity.Brain.Definitions.Conditions;
using CursedForest.Unity.Brain.Definitions.States;

namespace CursedForest.Unity.Brain.Runtime
{
    public static class BrainBuilder
    {
        public static BrainRuntime Build(BrainDefinition definition, AgentRuntimeDependencies dependencies)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (dependencies == null)
            {
                throw new ArgumentNullException(nameof(dependencies));
            }

            IReadOnlyList<StateDefinition> states = definition.States;
            IReadOnlyList<TransitionDefinition> transitions = definition.Transitions;

            if (states == null || states.Count == 0)
            {
                throw new InvalidOperationException($"Brain '{definition.name}' has no states.");
            }

            if (definition.InitialState == null)
            {
                throw new InvalidOperationException($"Brain '{definition.name}' has no initial state.");
            }

            var knownAssets = new HashSet<StateDefinition>();
            var knownIds = new HashSet<StateId>();

            foreach (StateDefinition stateDefinition in states)
            {
                if (stateDefinition == null)
                {
                    throw new InvalidOperationException($"Brain '{definition.name}' has an unassigned state.");
                }

                if (!knownAssets.Add(stateDefinition))
                {
                    throw new InvalidOperationException($"Brain '{definition.name}' repeats state asset '{stateDefinition.name}'.");
                }

                StateId id = stateDefinition.Id;

                if (!knownIds.Add(id))
                {
                    throw new InvalidOperationException($"Brain '{definition.name}' has duplicate state ID '{id}'.");
                }
            }

            if (!knownAssets.Contains(definition.InitialState))
            {
                throw new InvalidOperationException($"Initial state is not listed in brain '{definition.name}'.");
            }

            if (transitions == null)
            {
                throw new InvalidOperationException($"Brain '{definition.name}' has a null transition list.");
            }

            foreach (TransitionDefinition transition in transitions)
            {
                if (transition == null || transition.From == null || transition.To == null || transition.Condition == null)
                {
                    throw new InvalidOperationException($"Brain '{definition.name}' has an incomplete transition.");
                }

                if (!knownAssets.Contains(transition.From) || !knownAssets.Contains(transition.To))
                {
                    throw new InvalidOperationException($"Brain '{definition.name}' has a transition to an unregistered state asset.");
                }

                if (transition.Priority < 0)
                {
                    throw new InvalidOperationException($"Brain '{definition.name}' has a negative transition priority.");
                }
            }

            var machine = new StateMachine<AgentContext>(dependencies.Context);

            foreach (StateDefinition stateDefinition in states)
            {
                IState<AgentContext> state = stateDefinition.CreateState(dependencies);

                if (state == null || state.Id != stateDefinition.Id)
                {
                    throw new InvalidOperationException($"State definition '{stateDefinition.name}' created an invalid state.");
                }

                machine.RegisterState(state);
            }

            foreach (TransitionDefinition transition in transitions)
            {
                ICondition<AgentContext> condition = transition.Condition.CreateRuntimeCondition(
                    dependencies,
                    new HashSet<ConditionDefinition>());

                machine.AddTransition(new StateTransition<AgentContext>(
                    transition.From.Id,
                    transition.To.Id,
                    transition.Priority,
                    context => condition.Evaluate(context)));
            }

            return new BrainRuntime(machine, definition.InitialState.Id);
        }
    }
}
