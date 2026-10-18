using System;
using CursedForest.Unity.Brain.Definitions.Conditions;
using CursedForest.Unity.Brain.Definitions.States;
using UnityEngine;

namespace CursedForest.Unity.Brain.Definitions
{
    [Serializable]
    public sealed class TransitionDefinition
    {
        [SerializeField] private StateDefinition _from;
        [SerializeField] private StateDefinition _to;
        [SerializeField, Min(0)] private int _priority;
        [SerializeField] private ConditionDefinition _condition;

        public StateDefinition From => _from;
        public StateDefinition To => _to;
        public int Priority => _priority;
        public ConditionDefinition Condition => _condition;
    }
}
