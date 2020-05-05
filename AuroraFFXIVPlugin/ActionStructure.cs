using Sharlayan.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Aurora;
using Aurora.Devices;
using AuroraFFXIVPlugin.FFXIV.GSI;

namespace AuroraFFXIVPlugin
{
    public struct ActionStructure
    {
        public readonly int CoolDownPercent;
        public readonly bool InRange;
        public readonly bool IsAvailable;
        public readonly bool IsProcOrCombo;
        public readonly int RemainingCost;
        public bool IsCtrl;
        public bool IsShift;
        public bool IsAlt;
        public DeviceKeys Key;

        public ActionStructure(ActionItem f) : this()
        {
            CoolDownPercent = f.CoolDownPercent;
            InRange = f.InRange;
            IsAvailable = f.IsAvailable;
            IsProcOrCombo = f.IsProcOrCombo;
            RemainingCost = f.RemainingCost;
            getKeys(string.IsNullOrEmpty(f.KeyBinds) ? " " : f.KeyBinds);
        }

        private void getKeys(string keyBinds)
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
                    Key = getActualDeviceKey(string.IsNullOrWhiteSpace(s[s.Length - 1]) ? "=" : s[s.Length - 1]);
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

        public double GetCoolDownPrecent(GameState_FFXIV ffxiv)
        {
            return CoolDownPercent == 0 ? (ffxiv.Player.CastingPercentage == 0 ? 1 : ffxiv.Player.CastingPercentage) : (CoolDownPercent / 100D);
        }

        public override bool Equals(object obj)
        {
            var action = (obj is ActionStructure structure ? structure : default);
            return CoolDownPercent == action.CoolDownPercent && 
                   InRange == action.InRange && 
                   IsAvailable == action.IsAvailable && 
                   IsProcOrCombo == action.IsProcOrCombo && 
                   RemainingCost == action.RemainingCost && 
                   IsCtrl == action.IsCtrl && 
                   IsShift == action.IsShift && 
                   IsAlt == action.IsAlt && 
                   Key == action.Key;
        }

        public bool Equals(ActionStructure other)
        {
            return CoolDownPercent == other.CoolDownPercent && InRange == other.InRange && IsAvailable == other.IsAvailable && IsProcOrCombo == other.IsProcOrCombo && RemainingCost == other.RemainingCost && IsCtrl == other.IsCtrl && IsShift == other.IsShift && IsAlt == other.IsAlt && Key == other.Key;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = CoolDownPercent;
                hashCode = (hashCode * 397) ^ InRange.GetHashCode();
                hashCode = (hashCode * 397) ^ IsAvailable.GetHashCode();
                hashCode = (hashCode * 397) ^ IsProcOrCombo.GetHashCode();
                hashCode = (hashCode * 397) ^ RemainingCost;
                hashCode = (hashCode * 397) ^ IsCtrl.GetHashCode();
                hashCode = (hashCode * 397) ^ IsShift.GetHashCode();
                hashCode = (hashCode * 397) ^ IsAlt.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) Key;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{CoolDownPercent},{InRange},{IsAvailable},{IsProcOrCombo},{RemainingCost},{IsCtrl},{IsShift},{IsAlt},{Key}";
        }
    }
}
