namespace Patterns.DoubleBuffer.Core
{
    public sealed class SlapActor
    {
        public ActorId Id { get; }
        public ActorId TargetId { get; }

        public bool CurrentSlapped { get; private set; }
        public bool NextSlapped { get; private set; }

        public SlapActor(ActorId id, ActorId targetId)
        {
            Id = id;
            TargetId = targetId;
        }

        public void SeedCurrentSlap()
        {
            CurrentSlapped = true;
        }

        public bool ConsumeImmediateSlap()
        {
            bool wasSlapped = CurrentSlapped;
            CurrentSlapped = false;
            return wasSlapped;
        }

        public void ReceiveImmediateSlap()
        {
            CurrentSlapped = true;
        }

        public void QueueSlap()
        {
            NextSlapped = true;
        }

        public void PublishQueuedSlap()
        {
            CurrentSlapped = NextSlapped;
            NextSlapped = false;
        }
    }
}
