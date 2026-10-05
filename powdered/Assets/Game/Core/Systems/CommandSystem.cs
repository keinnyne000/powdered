namespace Game.Core.Systems
{
    public class CommandSystem : ISimulationSystem
    {
        readonly GameState _state;
        readonly CommandQueue _commands;
        public CommandSystem(GameState state, CommandQueue commands)
        {
            _state = state;
            _commands = commands;
        }
        
        public void Tick(float deltaTime)
        {
            while(_commands.TryDequeue(out var command))
                Apply(command);
        }

        public void Apply(ICommand command)
        {
            // TODO: Handle commands, probably using some kind of Command Router
        }    
    }
    
}