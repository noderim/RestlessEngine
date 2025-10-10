using System.Collections.Generic;
using System.Linq;
using RestlessLib.Attributes;
using UnityEngine;

namespace RestlessEngine.Application
{
    // Static class to manage supported resolutions
    public class SupportedResolutions
    {
        // List of supported resolutions by the game
        public static readonly List<Vector2> GameSupportedResolutions = new List<Vector2>
    {
        new Vector2(1280, 720),
        new Vector2(1600, 900),
        new Vector2(1920, 1080),
        new Vector2(2560, 1080),
        new Vector2(2560, 1440),
        new Vector2(3440, 1440),
    };

        private static List<Resolution> _supportedScreenResolutions;

        public static List<Resolution> SupportedScreenResolutions
        {
            get
            {
                if (_supportedScreenResolutions == null || _supportedScreenResolutions.Count == 0)
                    _supportedScreenResolutions = GetSupportedResolutions();

                return _supportedScreenResolutions;
            }
        }

        public static List<Resolution> GetSupportedResolutions()
        {
            var result = new List<Resolution>();

            foreach (var gameRes in GameSupportedResolutions)
            {
                var match = Screen.resolutions.FirstOrDefault(r =>
                    r.width == gameRes.x && r.height == gameRes.y);

                if (match.width > 0)
                    result.Add(match);
            }

            return result;
        }
    }

    // --- readonly instance mirrors ---
    [System.Serializable]
    public class ReadonlySupportedResolutions
    {
        [SerializeField, ReadOnly]
        private List<Vector2> _GameSupportedResolutions;

        [SerializeField, ReadOnly]
        private List<Vector2> _SupportedScreenResolutions;

        public void ReadResolutions()
        {
            _GameSupportedResolutions = SupportedResolutions.GameSupportedResolutions;
            _SupportedScreenResolutions = SupportedResolutions.SupportedScreenResolutions.Select(r => new Vector2(r.width, r.height)).ToList();
        }
    }
}