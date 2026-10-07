using Game.Core;

namespace Presentation.Game.Presentation
{
    public interface ISessionControl
    {
        void RequestRestart();
    }
    public class ViewContext
    {
        public ICommandSink CommandSink { get; }
        
        /// <summary>
        /// The Session's current GameState. NEVER mutate it OR sub-classes through this reference.
        /// </summary>
        public GameState GameState { get; }
        
        public ISessionControl SessionControl { get; }

        public ViewContext(ICommandSink commandSink, GameState gameState,
            ISessionControl sessionControl)
        {
            CommandSink = commandSink;
            GameState = gameState;
            SessionControl = sessionControl;
        }
    }
}