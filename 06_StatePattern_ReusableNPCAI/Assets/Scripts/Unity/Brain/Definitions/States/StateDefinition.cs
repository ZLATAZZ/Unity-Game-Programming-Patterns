using CursedForest.Core.FSM;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Brain.Runtime;
using UnityEngine;

namespace CursedForest.Unity.Brain.Definitions.States
{
    public abstract class StateDefinition : ScriptableObject
    {
        [SerializeField] private string _stateId;

        public StateId Id => new StateId(_stateId);

        public abstract IState<AgentContext> CreateState(AgentRuntimeDependencies dependencies);
    }
}
