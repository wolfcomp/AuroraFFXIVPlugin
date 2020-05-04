using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aurora;
using Aurora.Devices;

namespace AuroraFFXIVGSIPlugin.FFXIV.GSI
{
    public class FFXIVActionNode : List<FFXIVAction>
    {
        public override bool Equals(object obj)
        {
            if (obj is FFXIVActionNode actionNode)
            {
                return actionNode.All(t => this.Any(f => f.GetHashCode() == t.GetHashCode()));
            }
            return false;
        }
    }

    public struct FFXIVAction
    {
        public int CoolDownPercent;
        public bool InRange;
        public bool IsAvailable;
        public bool IsProcOrCombo;
        private string keyBinds;
        public DeviceKeys Key;
        public int RemainingCost;
        public bool IsCtrl;
        public bool IsShift;
        public bool IsAlt;

        public FFXIVAction(byte[] bytes, int offset, out int newOffset) : this()
        {
            CoolDownPercent = BitConverter.ToInt32(bytes, offset);
            InRange = BitConverter.ToBoolean(bytes, offset += 4);
            IsAvailable = BitConverter.ToBoolean(bytes, offset += 1);
            IsProcOrCombo = BitConverter.ToBoolean(bytes, offset += 1);
            RemainingCost = BitConverter.ToInt32(bytes, offset += 1);
            var count = BitConverter.ToInt32(bytes, offset += 4);
            keyBinds = Encoding.UTF8.GetString(bytes, offset += 4, count);
            getKeys();
            newOffset = offset + count;
        }

        private void getKeys()
        {
            if (!string.IsNullOrWhiteSpace(keyBinds))
            {
                if (keyBinds.Contains("+") && keyBinds.Length > 1)
                {
                    var s = keyBinds.Split('+');
                    for (var i = 0; i < s.Length - 1; i++)
                    {
                        switch (s[i])
                        {
                            case "Ctrl":
                                IsCtrl = true;
                                break;
                            case "Alt":
                                IsAlt = true;
                                break;
                            case "Shift":
                                IsShift = true;
                                break;
                        }
                    }
                    Key = getActualDeviceKey(s[s.Length - 1].Contains("+") ? "=" : s[s.Length - 1]);
                }
                else
                    Key = getActualDeviceKey(keyBinds.Contains("+") ? "=" : keyBinds);
            }
            else
            {
                Key = DeviceKeys.NONE;
            }
        }

        private DeviceKeys getActualDeviceKey(string s)
        {
            var key = getDeviceKey(s);
            return Global.kbLayout.LayoutKeyConversion.ContainsKey(key) ? Global.kbLayout.LayoutKeyConversion[key] : key;
        }

        private DeviceKeys getDeviceKey(string s)
        {
            foreach (var field in typeof(DeviceKeys).GetFields())
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute && attribute.Description.ToLowerInvariant().Equals(s.ToLowerInvariant()))
                    return (DeviceKeys) field.GetValue(null);
            return Enum.TryParse<DeviceKeys>("Undefined", true, out var u) ? u : DeviceKeys.NONE;
        }
    }
}
