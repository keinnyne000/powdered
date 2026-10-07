using System.Collections.Generic;
namespace Game.Core
{
    /// <summary>
    /// Laser-thin wrapper around a queue object
    /// Publish commands here for the command system to process them
    /// </summary>
    public sealed class CommandQueue : ICommandSink
    {
        readonly Queue<ICommand> _pending = new();
        
        public void Enqueue(ICommand command) => _pending.Enqueue(command);
        public bool TryDequeue(out ICommand command) => _pending.TryDequeue(out command);
    }
}