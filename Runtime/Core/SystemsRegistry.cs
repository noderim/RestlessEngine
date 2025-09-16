using System.Collections.Generic;
using RestlessLib.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RestlessEngine
{
    public static class SystemsRegistry
    {
        [ReadOnly]
        [Expandable]
        [SerializeField]
        [SerializeReference]
        private static List<SystemCore> systems = new();
        public static UnityEvent onSystemsRegistryChanged = new();

        public static void RegisterSystem(SystemCore system)
        {
            if (!systems.Contains(system))
            {
                systems.Add(system);
                SortByInitPriority();
                onSystemsRegistryChanged?.Invoke();
            }
        }

        public static void UnregisterSystem(SystemCore system)
        {
            systems.Remove(system);
            onSystemsRegistryChanged?.Invoke();
        }
        private static void SortByInitPriority()
        {
            systems.Sort((a, b) => a.InitPriority.CompareTo(b.InitPriority));
        }

        public static IReadOnlyList<SystemCore> GetRegisteredSystems()
        {
            return systems.AsReadOnly();
        }
    }

}
