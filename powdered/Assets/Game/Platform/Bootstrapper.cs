using UnityEngine;

namespace Game.Platform
{
    public static class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap() => new GameObject("App").AddComponent<App>();
    } 
}
