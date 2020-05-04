using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aurora.Profiles;
using Newtonsoft.Json.Linq;

namespace AuroraFFXIVGSIPlugin.FFXIV.GSI
{
    public class GameState_FFXIV : GameState<GameState_FFXIV>
    {
        public FFXIVActionNode Actions { get; } = new FFXIVActionNode();

        public PlayerStruct Player { get; set; } = new PlayerStruct();

        public FFXIVKeyBindNode KeyBinds { get; } = new FFXIVKeyBindNode();
        
        public override bool Equals(object obj)
        {
            if (obj is GameState_FFXIV ffxiv)
            {
                return ffxiv.Actions.Equals(Actions) && ffxiv.KeyBinds.Equals(KeyBinds) && ffxiv.Player.Equals(Player);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Actions != null ? Actions.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Player.GetHashCode();
                hashCode = (hashCode * 397) ^ (KeyBinds != null ? KeyBinds.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    public class FFXIVKeyBindNode : List<KeyBindStructure>
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

    public class FFXIVActionNode : List<ActionStructure>
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
}