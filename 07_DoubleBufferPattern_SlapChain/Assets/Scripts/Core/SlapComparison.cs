using System;
using System.Collections.Generic;

namespace Patterns.DoubleBuffer.Core
{
    public sealed class SlapComparison
    {
        private readonly SlapStage[] _stages;
        private readonly IReadOnlyList<SlapStage> _readOnlyStages;

        public IReadOnlyList<SlapStage> Stages => _readOnlyStages;
        public int CompletedTicks => _stages[0].CompletedTicks;
        public bool IsAwaitingPublication => _stages[0].IsAwaitingPublication;

        public SlapComparison()
        {
            _stages = new[]
            {
                new SlapStage(SimulationMode.Immediate, UpdateOrder.Forward),
                new SlapStage(SimulationMode.DoubleBuffered, UpdateOrder.Forward),
                new SlapStage(SimulationMode.Immediate, UpdateOrder.Reverse),
                new SlapStage(SimulationMode.DoubleBuffered, UpdateOrder.Reverse)
            };

            _readOnlyStages = Array.AsReadOnly(_stages);
        }

        public void Evaluate()
        {
            if (IsAwaitingPublication)
            {
                throw new InvalidOperationException("Already awaiting publication.");
            }

            foreach (SlapStage stage in _stages)
            {
                stage.Evaluate();
            }
        }

        public void Publish()
        {
            if (!IsAwaitingPublication)
            {
                throw new InvalidOperationException("Evaluate the next tick first.");
            }

            foreach (SlapStage stage in _stages)
            {
                stage.Publish();
            }
        }

        public void CompleteTick()
        {
            if (!IsAwaitingPublication)
            {
                Evaluate();
            }

            Publish();
        }
    }
}
