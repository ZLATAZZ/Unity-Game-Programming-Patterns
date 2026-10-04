namespace PrototypeForge.Core.Prototypes
{
    public interface IPrototype<out T>
    {
        PrototypeId Id { get; }
        T Clone();
    }
}