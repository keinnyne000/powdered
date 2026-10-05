namespace Game.Core.Systems
{
    public class CommandSystem : ISimulationSystem
    {
        readonly GameState _state;

        public CommandSystem(GameState state)
        {
            _state = state;
        }
        
        public void Tick(float deltaTime)
        {
            throw new System.NotImplementedException();
        }
    }
}