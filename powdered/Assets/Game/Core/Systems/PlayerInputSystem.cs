using UnityEngine;

namespace Game.Core.Systems

{
    public class PlayerInputSystem : ISimulationSystem
    {
        readonly IInputSource _input;
        readonly ICommandSink _commands;

        public PlayerInputSystem(IInputSource input, ICommandSink commands)
        {
            _input = input;
            _commands = commands;
        }

        public void Tick(float deltaTime)
        {
            _commands.Enqueue(new MoveInput(_input.Move));
        }
    }
}
