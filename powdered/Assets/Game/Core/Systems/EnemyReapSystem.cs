namespace Game.Core.Systems
{
    /// <summary>
    /// Responsible for cleaning up dead enemies
    /// </summary>
    public class EnemyReapSystem : ISimulationSystem
    {
        readonly GameState _state;
        public EnemyReapSystem(GameState state)
        {
            _state = state;
        }
        
        public void Tick(float deltaTime)
        {
        }
    }
}