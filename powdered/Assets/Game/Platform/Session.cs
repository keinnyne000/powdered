using Game.Core;
using Game.Core.Systems;

namespace Game.Platform
{
    public class Session
    {
        private App _app;
        public GameState State { get; }
        public GameConfig GameConfig { get; }
        
        /// <summary>
        /// Session constructor; creates a new game state and command queue, registers systems
        /// (!) Currently, calling this a second time does not dispose of the previous systems
        /// (?) May want to wrap App in a smaller class such as "GameLoop" since we don't need all of App
        /// </summary>
        /// <param name="app">The current app, used for registering systems</param>
        public Session(App app, GameConfig config)
        {
            State = new GameState();
            var commands = new CommandQueue();
            GameConfig = config;
            
            app.Register(new CommandSystem(State, commands));
            // Future systems, like the player spawn system can pass config to get a reference
            //TODO: publish commands to a view context,
            //      which can be bound by IGameViews (ui components)
        }
    }
}