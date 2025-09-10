using System;
using System.Collections.Generic;

namespace RestlessEngine.Diagnostics
{
    public class SystemsMonitor : SingletonSystem<SystemsMonitor>
    {

        public List<SystemMonitorContext> _contexts = new List<SystemMonitorContext>();

        protected override bool Init()
        {
            GetMonitors();
            SystemsRegistry.onSystemsRegistryChanged.AddListener(GetMonitors);

            return true;
        }

        private void GetMonitors()
        {
            _contexts.Clear();
            IReadOnlyList<SystemCore> systems = SystemsRegistry.GetRegisteredSystems();
            foreach (var system in systems)
            {
                _contexts.Add(new SystemMonitorContext(system));
            }
        }

        private void OnGUI()
        {
            foreach (var context in _contexts)
            {
                context.Refresh();
            }
        }
    }

    [Serializable]
    public class SystemMonitorContext
    {
        public SystemCore System;
        public string SystemName;
        public bool IsInitialized;
        public SystemState State;

        public bool Validated;
        public string message;

        public SystemMonitorContext(SystemCore system)
        {
            System = system;
            SystemName = system.GetType().Name;
            IsInitialized = system.IsInitialized;
            State = system.SystemState;
        }

        public void Refresh()
        {
            IsInitialized = System.IsInitialized;
            State = System.SystemState;
        }
    }

}
