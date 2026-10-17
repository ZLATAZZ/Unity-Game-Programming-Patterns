namespace CursedForest.Core.FSM
{
    public interface IState<TContext>
    {
        StateId Id { get; }

        void Enter(TContext context);
        void Tick(TContext context, float deltaTime);
        void Exit(TContext context);
    }
}