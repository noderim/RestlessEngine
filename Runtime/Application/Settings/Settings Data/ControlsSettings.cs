using System;

namespace RestlessEngine.Application.Settings
{
    [Serializable]
    public struct ControlsSettings
    {
        public CurrentDevicePreset currentDevicePreset;


        public enum CurrentDevicePreset
        {
            KeyboardMouse,
            Gamepad,
            Touch
        }
    }
}
