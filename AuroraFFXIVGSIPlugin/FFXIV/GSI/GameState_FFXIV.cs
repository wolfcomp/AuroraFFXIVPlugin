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
        private FFXIVActionNode _Actions { get; } = new FFXIVActionNode();

        public FFXIVActionNode Actions
        {
            get
            {
                if (_ParsedData.ContainsKey("actions"))
                {
                    _Actions.Clear();
                    var actionsList = JArray.Parse(_ParsedData["actions"].ToString()).Select(t => t.ToObject<byte>()).ToArray();
                    var offset = 0;
                    while (actionsList.Length > offset)
                    {
                        _Actions.Add(new FFXIVAction(actionsList, offset, out offset));
                    }
                }
                return _Actions;
            }
        }

        private FFXIVPlayerNode _Player { get; set; } = new FFXIVPlayerNode();

        public FFXIVPlayerNode Player
        {
            get
            {
                if(_ParsedData.ContainsKey("player")) _Player = NodeFor<FFXIVPlayerNode>("player");
                return _Player;
            }
        }

        public FFXIVKeyBindNode _KeyBinds { get; } = new FFXIVKeyBindNode();

        public FFXIVKeyBindNode KeyBinds
        {
            get
            {
                if (_ParsedData.ContainsKey("keybinds"))
                {
                    _KeyBinds.Clear();
                    var actionsList = JArray.Parse(_ParsedData["keybinds"].ToString()).Select(t => t.ToObject<byte>()).ToArray();
                    var offset = 0;
                    while (actionsList.Length > offset)
                    {
                        _KeyBinds.Add(new FFXIVKeyBind(actionsList, offset, out offset));
                    }
                }
                return _KeyBinds;
            }
        }

        public GameState_FFXIV() : base() { }

        public GameState_FFXIV(string json) : base(json) { }

        public override bool Equals(object obj)
        {
            if (obj is GameState_FFXIV ffxiv)
            {
                ffxiv.Actions.Equals(Actions);
            }
            return base.Equals(obj);
        }
    }
}