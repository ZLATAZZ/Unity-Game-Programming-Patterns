namespace CursedForest.Unity.Agents.Capabilities
{
    public interface IAgentEnergy
    {
        float Current { get; }
        float Maximum { get; }

        void Recover(float amount);
        void Spend(float amount);
    }
}