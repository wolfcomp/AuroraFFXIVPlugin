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
using AuroraFFXIVGSIPlugin.FFXIV.GSI;

namespace AuroraFFXIVGSIPlugin
{
    public struct ActionStructure
    {
        public int CoolDownPercent;
        public bool InRange;
        public bool IsAvailable;
        public bool IsProcOrCombo;
        public int RemainingCost;
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
    }
}
