using System;
using System.Collections.Generic;
using CursedForest.Unity.Brain.Definitions.States;
using UnityEngine;

namespace CursedForest.Unity.Brain.Definitions
{
    [CreateAssetMenu(fileName = "BrainDefinition", menuName = "Cursed Forest/AI/Brain Definition")]
    public sealed class BrainDefinition : ScriptableObject
    {
        [SerializeField] private StateDefinition _initialState;
        [SerializeField] private StateDefinition[] _states = Array.Empty<StateDefinition>();
        [SerializeField] private TransitionDefinition[] _transitions = Array.Empty<TransitionDefinition>();

        public StateDefinition InitialState => _initialState;
        public IReadOnlyList<StateDefinition> States => _states;
        public IReadOnlyList<TransitionDefinition> Transitions => _transitions;
    }
}
