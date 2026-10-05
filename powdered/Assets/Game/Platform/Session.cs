using Game.Core;
using Game.Core.Systems;

namespace Game.Platform
{
    public class Session
    {
        private App _app;
        public GameState State { get; }
        
        public Session(App app)
        {
            State = new GameState();
            var commands = new CommandQueue();
            
            app.Register(new CommandSystem(State, commands));
            //TODO: publish commands to a view context,
            //      which can be bound by IGameViews (ui components)
        }
    }
}