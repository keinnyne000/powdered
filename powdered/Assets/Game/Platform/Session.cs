using Game.Core;
using Game.Core.Systems;
using Game.Presentation;
using Presentation.Game.Presentation;

namespace Game.Platform
{
    public class Session : ISessionControl
    {
        private App _app;
        public GameState State { get; }
        public GameConfig GameConfig { get; }
        public ViewContext ViewContext { get; }
        
        /// <summary>
        /// Session constructor; creates a new game state and command queue, registers systems
        /// (!) Currently, calling this a second time does not dispose of the previous systems
        /// (?) May want to wrap App in a smaller class such as "GameLoop" since we don't need all of App
        /// </summary>
        /// <param name="app">The current app, used for registering systems</param>
        /// <param name="config">The game config asset we should use for the session</param>
        public Session(App app, GameConfig config)
        {
            State = new GameState();
            var commands = new CommandQueue();
            ViewContext = new ViewContext(commands, State, this);
            
            GameConfig = config;
            
            app.Register(new CommandSystem(State, commands));
            // Future systems, like the player spawn system can pass config to get a reference
        }

        public void RequestRestart()
        {
            throw new System.NotImplementedException();
        }

        public void Attach(ViewRegistry registry)
        {
            foreach (var view in registry.views)
                view.Bind(ViewContext);
        }
    }
}