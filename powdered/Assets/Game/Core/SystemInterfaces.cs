namespace Game.Core
{
    public interface ISimulationSystem
    {
        void Tick(float deltaTime);
    }

    public interface IPresentationSystem
    {
        void Present(float deltaTime);
    }
}