using System;
using CursedForest.Unity.Agents.Capabilities;
using UnityEngine;
using Zenject;

namespace CursedForest.Unity.Agents.Stats
{
     public sealed class MovementEnergyDrain : IInitializable, ITickable
    {
        private readonly IAgentMovement _movement;
        private readonly IAgentEnergy _energy;
        private readonly float _costPerMeter;
        private Vector3 _previousPosition;

        public MovementEnergyDrain(IAgentMovement movement, IAgentEnergy energy, float costPerMeter)
        {
            _movement = movement ?? throw new ArgumentNullException(nameof(movement));
            _energy = energy ?? throw new ArgumentNullException(nameof(energy));

            if (float.IsNaN(costPerMeter) || float.IsInfinity(costPerMeter) || costPerMeter < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(costPerMeter));
            }

            _costPerMeter = costPerMeter;
        }

        public void Initialize()
        {
            _previousPosition = _movement.Position;
        }

        public void Tick()
        {
            Vector3 currentPosition = _movement.Position;
            Vector3 difference = currentPosition - _previousPosition;
            difference.y = 0f;
            _previousPosition = currentPosition;

            float travelledDistance = difference.magnitude;
            _energy.Spend(travelledDistance * _costPerMeter);
        }
    }
}
