namespace Game.Core
{
    /// <summary>
    /// Defines an ISimulationSystem that is to be called by App.cs every frame
    /// </summary>
    public interface ISimulationSystem
    {
        void Tick(float deltaTime);
    }

    /// <summary>
    /// Defines an IPresentationSystem, usually UI, that is to be called by App.cs every frame
    /// </summary>
    public interface IPresentationSystem
    {
        void Present(float deltaTime);
    }
}