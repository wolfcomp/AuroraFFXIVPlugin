using Sharlayan.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AuroraFFXIVGSIPluginTest
{
    public struct ActionStructure
    {
        public int CoolDownPercent;
        public bool InRange;
        public bool IsAvailable;
        public bool IsProcOrCombo;
        public string KeyBinds;
        public int RemainingCost;

        public ActionStructure(ActionItem f) : this()
        {
            CoolDownPercent = f.CoolDownPercent;
            InRange = f.InRange;
            IsAvailable = f.IsAvailable;
            IsProcOrCombo = f.IsProcOrCombo;
            KeyBinds = string.IsNullOrEmpty(f.KeyBinds) ? " " : f.KeyBinds;
            RemainingCost = f.RemainingCost;
        }

        public byte[] ToBytes()
        {
            var b = new List<byte>();
            b.AddRange(BitConverter.GetBytes(CoolDownPercent));
            b.AddRange(BitConverter.GetBytes(InRange));
            b.AddRange(BitConverter.GetBytes(IsAvailable));
            b.AddRange(BitConverter.GetBytes(IsProcOrCombo));
            b.AddRange(BitConverter.GetBytes(RemainingCost));
            var f = Encoding.UTF8.GetBytes(KeyBinds);
            b.AddRange(BitConverter.GetBytes(f.Length));
            b.AddRange(f);
            return b.ToArray();
        }
    }
}
