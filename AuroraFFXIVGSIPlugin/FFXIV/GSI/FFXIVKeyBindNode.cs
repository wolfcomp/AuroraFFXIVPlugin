using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aurora.Devices;

namespace AuroraFFXIVGSIPlugin.FFXIV.GSI
{
    public class FFXIVKeyBindNode : List<FFXIVKeyBind>
    {
        public override bool Equals(object obj)
        {
            if (obj is FFXIVKeyBindNode keyBindNode)
            {
                return keyBindNode.All(t => this.Any(f => f.GetHashCode() == t.GetHashCode()));
            }
            return false;
        }
    }

    public struct FFXIVKeyBind
    {
        public string Command;
        public DeviceKeys Key;
        public byte KeyRaw;
        public DeviceKeys[] KeyMod;

        public FFXIVKeyBind(byte[] bytes, int offset, out int newOffset) : this()
        {
            var count = BitConverter.ToInt32(bytes, offset);
            Command = Encoding.UTF8.GetString(bytes, offset += 4, count);
            Key = (DeviceKeys) bytes[offset += count];
            KeyRaw = bytes[offset += 1];
            switch (bytes[offset += 1])
            {
                case 1:
                    KeyMod = new [] { DeviceKeys.LEFT_SHIFT, DeviceKeys.RIGHT_SHIFT };
                    break;
                case 2:
                    KeyMod = new [] { DeviceKeys.LEFT_CONTROL, DeviceKeys.RIGHT_CONTROL };
                    break;
                case 4:
                    KeyMod = new [] { DeviceKeys.LEFT_ALT, DeviceKeys.RIGHT_ALT };
                    break;
                default:
                    KeyMod = new [] { DeviceKeys.NONE };
                    break;
            }
            newOffset = offset + 1;
        }
    }
}