
using System;
using System.Collections.Generic;

namespace CursedForest.Core.FSM
{
    public sealed class StateMachine<TContext>
    {
        private readonly Dictionary<StateId, IState<TContext>> _states;
        private readonly Dictionary<StateId, List<StateTransition<TContext>>> _transitions;
        private readonly TContext _context;

        private IState<TContext> _currentState;
        private bool _isStarted;
        private bool _isFaulted;
        private bool _isExecuting;

        public StateId CurrentStateId
        {
            get
            {
                if (!_isStarted)
                {
                    throw new InvalidOperationException("FSM has not been started.");
                }

                return _currentState.Id;
            }
        }

        public StateMachine(TContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
            _states = new Dictionary<StateId, IState<TContext>>();
            _transitions = new Dictionary<StateId, List<StateTransition<TContext>>>();
        }

        public void RegisterState(IState<TContext> state)
        {
            if (_isStarted)
            {
                throw new InvalidOperationException("Cannot modify the FSM after it has started.");
            }

            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (state.Id.IsEmpty)
            {
                throw new ArgumentException("State ID cannot be empty.", nameof(state));
            }

            if (_states.ContainsKey(state.Id))
            {
                throw new ArgumentException($"State with ID '{state.Id}' is already registered.", nameof(state));
            }

            _states.Add(state.Id, state);
        }

        public void AddTransition(StateTransition<TContext> transition)
        {
            if (_isStarted)
            {
                throw new InvalidOperationException("Cannot modify the FSM after it has started.");
            }

            if (transition == null)
            {
                throw new ArgumentNullException(nameof(transition));
            }

            if (!_states.ContainsKey(transition.From))
            {
                throw new ArgumentException($"Source state '{transition.From}' is not registered.", nameof(transition));
            }

            if (!_states.ContainsKey(transition.To))
            {
                throw new ArgumentException($"Target state '{transition.To}' is not registered.", nameof(transition));
            }

            if (!_transitions.TryGetValue(transition.From, out List<StateTransition<TContext>> transitionsList))
            {
                transitionsList = new List<StateTransition<TContext>>();
                _transitions.Add(transition.From, transitionsList);
            }

            for (int i = 0; i < transitionsList.Count; i++)
            {
                if (transitionsList[i].Priority < transition.Priority)
                {
                    transitionsList.Insert(i, transition);
                    return;
                }
            }

            transitionsList.Add(transition);
        }

        public void Start(StateId initialStateId)
        {
            if (_isStarted)
            {
                throw new InvalidOperationException("FSM has already been started.");
            }

            if (_isExecuting)
            {
                throw new InvalidOperationException("FSM is already executing.");
            }

            if (initialStateId.IsEmpty)
            {
                throw new ArgumentException("Initial state ID cannot be empty.", nameof(initialStateId));
            }

            if (!_states.TryGetValue(initialStateId, out IState<TContext> initialState))
            {
                throw new ArgumentException($"Initial state '{initialStateId}' is not registered.", nameof(initialStateId));
            }

            _currentState = initialState;
            _isStarted = true;
            _isExecuting = true;

            try
            {
                _currentState.Enter(_context);
            }
            catch
            {
                _isFaulted = true;
                throw;
            }
            finally
            {
                _isExecuting = false;
            }
        }

        public void Tick(float deltaTime)
        {
            if (!_isStarted)
            {
                throw new InvalidOperationException("FSM has not been started.");
            }

            if (_isFaulted)
            {
                throw new InvalidOperationException("Cannot tick a faulted FSM.");
            }

            if (_isExecuting)
            {
                throw new InvalidOperationException("FSM is already executing.");
            }

            if (float.IsNaN(deltaTime) || float.IsInfinity(deltaTime) || deltaTime < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaTime), "Delta time must be finite and non-negative.");
            }

            _isExecuting = true;

            try
            {
                if (_transitions.TryGetValue(_currentState.Id, out List<StateTransition<TContext>> transitions))
                {
                    foreach (StateTransition<TContext> transition in transitions)
                    {
                        if (transition.IsSatisfied(_context))
                        {
                            ChangeState(transition.To);
                            break;
                        }
                    }
                }

                _currentState.Tick(_context, deltaTime);
            }
            catch
            {
                _isFaulted = true;
                throw;
            }
            finally
            {
                _isExecuting = false;
            }
        }

        private void ChangeState(StateId nextStateId)
        {
            if (!_states.TryGetValue(nextStateId, out IState<TContext> nextState))
            {
                throw new InvalidOperationException($"Target state '{nextStateId}' is not registered.");
            }

            _currentState.Exit(_context);
            _currentState = nextState;
            _currentState.Enter(_context);
        }
    }
}
