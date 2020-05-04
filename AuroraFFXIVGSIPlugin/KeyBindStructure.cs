using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aurora.Devices;

namespace AuroraFFXIVGSIPlugin
{
    public struct KeyBindStructure
    {
        public string Command;
        public DeviceKeys Key;
        public byte KeyRaw;
        public DeviceKeys[] KeyMod;
        public KeyBindStructure[] KeyBinds;

        public KeyBindStructure(DeviceKeys key, byte keyRaw, FFXIVModifierKey keymod)
        {
            Command = "";
            Key = key;
            KeyRaw = keyRaw;
            switch (keymod)
            {
                case FFXIVModifierKey.SHIFT:
                    KeyMod = new [] { DeviceKeys.LEFT_SHIFT, DeviceKeys.RIGHT_SHIFT };
                    break;
                case FFXIVModifierKey.CTRL:
                    KeyMod = new [] { DeviceKeys.LEFT_CONTROL, DeviceKeys.RIGHT_CONTROL };
                    break;
                case FFXIVModifierKey.ALT:
                    KeyMod = new [] { DeviceKeys.LEFT_ALT, DeviceKeys.RIGHT_ALT };
                    break;
                default:
                    KeyMod = new [] { DeviceKeys.NONE };
                    break;
            }
            KeyBinds = new KeyBindStructure[0];
        }

        public KeyBindStructure(string command, KeyBindStructure[] keyBinds)
        {
            var key = keyBinds[0];
            Command = command;
            Key = key.Key;
            KeyRaw = key.KeyRaw;
            KeyMod = key.KeyMod;
            KeyBinds = keyBinds;
        }

        public override bool Equals(object obj)
        {
            var key = (obj is KeyBindStructure structure ? structure : default);
            var @this = this;
            return key.Key == @this.Key &&
                   key.KeyMod.All(t => @this.KeyMod.Contains(t)) &&
                   @this.Command == key.Command &&
                   @this.KeyRaw == key.KeyRaw &&
                   key.KeyBinds.All(t => @this.KeyBinds.Contains(t));
        }
    }
}