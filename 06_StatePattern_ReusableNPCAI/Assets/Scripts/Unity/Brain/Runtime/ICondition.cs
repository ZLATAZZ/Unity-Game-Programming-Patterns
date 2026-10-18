namespace CursedForest.Unity.Brain.Runtime
{
    public interface ICondition<in TContext>
    {
        bool Evaluate(TContext context);
    }
}
