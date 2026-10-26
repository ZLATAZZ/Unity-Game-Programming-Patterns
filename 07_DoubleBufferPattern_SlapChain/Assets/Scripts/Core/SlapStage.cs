using System;
using System.Collections.Generic;

namespace Patterns.DoubleBuffer.Core
{
    public sealed class SlapStage
    {
        private readonly SlapActor[] _actors;
        private readonly ActorId[] _executionOrder;
        private readonly List<SlapReaction> _reactions = new List<SlapReaction>();
        private readonly IReadOnlyList<SlapReaction> _readOnlyReactions;

        public SimulationMode Mode { get; }
        public UpdateOrder Order { get; }
        public int CompletedTicks { get; private set; }
        public bool IsAwaitingPublication { get; private set; }
        public IReadOnlyList<SlapReaction> Reactions => _readOnlyReactions;

        public SlapStage(SimulationMode mode, UpdateOrder order)
        {
            if (!Enum.IsDefined(typeof(SimulationMode), mode))
            {
                throw new ArgumentOutOfRangeException(nameof(mode));
            }

            if (!Enum.IsDefined(typeof(UpdateOrder), order))
            {
                throw new ArgumentOutOfRangeException(nameof(order));
            }

            Mode = mode;
            Order = order;
            _readOnlyReactions = _reactions.AsReadOnly();

            _actors = new[]
            {
                new SlapActor(ActorId.Harry, ActorId.Baldy),
                new SlapActor(ActorId.Baldy, ActorId.Chump),
                new SlapActor(ActorId.Chump, ActorId.Harry)
            };

            _executionOrder = order == UpdateOrder.Forward
                ? new[] { ActorId.Harry, ActorId.Baldy, ActorId.Chump }
                : new[] { ActorId.Chump, ActorId.Baldy, ActorId.Harry };

            _actors[(int)ActorId.Harry].SeedCurrentSlap();
        }

        public SlapActor GetActor(ActorId id)
        {
            int index = (int)id;

            if (index < 0 || index >= _actors.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return _actors[index];
        }

        public void Evaluate()
        {
            if (IsAwaitingPublication)
            {
                throw new InvalidOperationException("Publish the current tick before evaluating again.");
            }

            _reactions.Clear();

            foreach (ActorId id in _executionOrder)
            {
                SlapActor actor = GetActor(id);

                bool shouldReact = Mode == SimulationMode.DoubleBuffered
                    ? actor.CurrentSlapped
                    : actor.ConsumeImmediateSlap();

                if (!shouldReact)
                {
                    continue;
                }

                SlapActor target = GetActor(actor.TargetId);

                if (Mode == SimulationMode.DoubleBuffered)
                {
                    target.QueueSlap();
                }
                else
                {
                    target.ReceiveImmediateSlap();
                }

                _reactions.Add(new SlapReaction(actor.Id, target.Id));
            }

            IsAwaitingPublication = true;
        }

        public void Publish()
        {
            if (!IsAwaitingPublication)
            {
                throw new InvalidOperationException("Evaluate a tick before publishing it.");
            }

            if (Mode == SimulationMode.DoubleBuffered)
            {
                foreach (SlapActor actor in _actors)
                {
                    actor.PublishQueuedSlap();
                }
            }

            IsAwaitingPublication = false;
            CompletedTicks++;
        }
    }
}
