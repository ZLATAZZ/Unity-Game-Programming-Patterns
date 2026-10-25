namespace Patterns.DoubleBuffer.Core
{
    public readonly struct SlapReaction
    {
        public ActorId Source { get; }
        public ActorId Target { get; }

        public SlapReaction(ActorId source, ActorId target)
        {
            Source = source;
            Target = target;
        }
    }
}
