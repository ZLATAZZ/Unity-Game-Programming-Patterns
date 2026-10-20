
using System;
using CursedForest.Core.FSM;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Agents.Capabilities;

namespace CursedForest.Unity.Behaviors
{
    public sealed class SleepState : IState<AgentContext>
    {
        private readonly IAgentEnergy _energy;
        private readonly float _recoveryPerSecond;

        public StateId Id { get; }

        public SleepState(StateId id, IAgentEnergy energy, float recoveryPerSecond)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException("State ID cannot be empty.", nameof(id));
            }

            _energy = energy ?? throw new ArgumentNullException(nameof(energy));

            if (float.IsNaN(recoveryPerSecond) || float.IsInfinity(recoveryPerSecond) || recoveryPerSecond <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(recoveryPerSecond));
            }

            Id = id;
            _recoveryPerSecond = recoveryPerSecond;
        }

        public void Enter(AgentContext context)
        {
            context.Movement.Stop();
        }

        public void Tick(AgentContext context, float deltaTime)
        {
            _energy.Recover(_recoveryPerSecond * deltaTime);
        }

        public void Exit(AgentContext context)
        {
        }
    }
}
