namespace Patterns.Command.Core
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}