using System;
using UnityEngine;

namespace RestlessEngine.Application.Settings
{
    [Serializable]
    public struct InterfaceSettings
    {
        [Header("Interface Settings")]
        public Language language;
        [SerializeField]

        public enum Language
        {
            English,
            French,
            Deutsch,
            Polish,
            Spanish,
        }
    }
}
