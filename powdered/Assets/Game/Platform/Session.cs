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
            
            app.Register(new CommandSystem(State));
        }
    }
}