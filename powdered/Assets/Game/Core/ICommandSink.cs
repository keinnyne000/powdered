namespace Game.Core
{
    public interface ICommandSink
    {
        /// <summary>
        /// Queues an ICommand to be processed next frame
        /// </summary>
        /// <param name="command"></param>
        public void Enqueue(ICommand command);
    }
}