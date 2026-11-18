using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Core.Systems
{
    /// <summary>
    /// PROTOTYPE: press ESC to quit. Stops play mode in the editor, closes the app in a build.
    /// </summary>
    public class QuitSystem : ISimulationSystem
    {
        public void Tick(float dt)
        {
            var keyboard = Keyboard.current;
            if (keyboard == null || !keyboard.escapeKey.wasPressedThisFrame) return;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}