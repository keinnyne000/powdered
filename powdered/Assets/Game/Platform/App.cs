using System.Collections.Generic;
using Game.Core;
using UnityEngine;

namespace Game.Platform
{
    [DisallowMultipleComponent]
    public sealed class App : MonoBehaviour
    {
        private readonly List<ISimulationSystem> _simulation = new List<ISimulationSystem>();
        private readonly List<IPresentationSystem> _presentation = new List<IPresentationSystem>();
        
        private Session _session;
        
        public void Register(ISimulationSystem system) => _simulation.Add(system);

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            // Resources.Load(Config);
        }

        private void Start()
        {
            _session = new Session(this);
        }

        void Update()
        {
            var dt = Time.deltaTime;
            foreach (var system in _simulation)
                system.Tick(dt);
        }

        private void LateUpdate()
        {
            var dt = Time.deltaTime;
            foreach (var system in _presentation)
                system.Present(dt);
        }
    }
}
