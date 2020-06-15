using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using Aurora;
using Aurora.Devices;
using Aurora.EffectsEngine;
using Aurora.Profiles;
using Aurora.Settings.Layers;
using Aurora.Utils;
using AuroraFFXIVPlugin.FFXIV.GSI;
using Newtonsoft.Json;

namespace AuroraFFXIVPlugin.FFXIV.Layers
{
    public class FFXIVActionLayerHandlerProperties : LayerHandlerProperties<FFXIVActionLayerHandlerProperties>
    {
        public Color? _Combo { get; set; }

        [JsonIgnore]
        public Color Combo
        {
            get { return Logic._Combo ?? _Combo ?? Color.Empty; }
        }

        public Color? _OutOfRange { get; set; }

        [JsonIgnore]
        public Color OutOfRange
        {
            get { return Logic._OutOfRange ?? _OutOfRange ?? Color.Empty; }
        }

        public Color? _NotAvailable { get; set; }

        [JsonIgnore]
        public Color NotAvailable
        {
            get { return Logic._NotAvailable ?? _NotAvailable ?? Color.Empty; }
        }

        public FFXIVActionLayerHandlerProperties() : base() { }

        public FFXIVActionLayerHandlerProperties(bool assign_default = false) : base(assign_default) { }

        public override void Default()
        {
            base.Default();
            _PrimaryColor = Color.LawnGreen;
            _Combo = Color.White;
            _OutOfRange = Color.DarkRed;
            _NotAvailable = Color.Black;
        }
    }

    [LayerHandlerMeta(Name = "Action Layer")]
    public class FFXIVActionLayerHandler : LayerHandler<FFXIVActionLayerHandlerProperties>
    {
        private EffectLayer prev = new EffectLayer();
        private List<ActionStructure> prevActions = new List<ActionStructure>();
        private List<Func<ActionStructure, bool>> prevDevice = new List<Func<ActionStructure, bool>>();
        private double prevCastPercent;
        
        protected override UserControl CreateControl()
        {
            return new Control_FFXIVActionLayerHandler(this);
        }

        private static List<DeviceKeys> modifList = new List<DeviceKeys> { DeviceKeys.LEFT_ALT, DeviceKeys.LEFT_CONTROL, DeviceKeys.LEFT_SHIFT, DeviceKeys.RIGHT_ALT, DeviceKeys.RIGHT_CONTROL, DeviceKeys.RIGHT_SHIFT };

        public override EffectLayer Render(IGameState gamestate)
        {
            var layer = new EffectLayer("FFXIV - Action Layer");
            layer.Fill(Color.Transparent);
            if (gamestate is GameState_FFXIV ffxiv)
            {
                lock (ffxiv.Actions)
                {
                    var modif = getModifs();
                    if (ffxiv.Actions.Any() && (Math.Abs(ffxiv.Player.CastingPercentage - prevCastPercent) > 0 || !prevActions.Any() || !ffxiv.Actions.All(t => prevActions.Any(f => f.Equals(t))) || !prevDevice.Any() || !modif.All(t => prevDevice.Contains(t))))
                    {
                        layer.Set(ffxiv.Actions.Where(t => t.Key != DeviceKeys.NONE).Select(t => t.Key).ToArray(), Properties.NotAvailable);
                        foreach (var ffxivAction in ffxiv.Actions.Where(t => modif.All(f => f(t)) && t.Key != DeviceKeys.NONE))
                        {
                            layer.Set(ffxivAction.Key, ColorUtils.MultiplyColorByScalar(Properties.PrimaryColor, ffxivAction.GetCoolDownPrecent(ffxiv)));
                            if (ffxivAction.IsProcOrCombo)
                                layer.Set(ffxivAction.Key, ColorUtils.MultiplyColorByScalar(Properties.Combo, ffxivAction.GetCoolDownPrecent(ffxiv)));
                            if (!ffxivAction.InRange)
                                layer.Set(ffxivAction.Key, ColorUtils.MultiplyColorByScalar(Properties.OutOfRange, ffxivAction.GetCoolDownPrecent(ffxiv)));
                            if (!ffxivAction.IsAvailable)
                                layer.Set(ffxivAction.Key, Properties.NotAvailable);
                        }
                        setModifKeys(layer, ffxiv.Actions.Where(t => t.IsCtrl).ToArray(), new [] { DeviceKeys.LEFT_CONTROL, DeviceKeys.RIGHT_CONTROL }, ffxiv);
                        setModifKeys(layer, ffxiv.Actions.Where(t => t.IsShift).ToArray(), new [] { DeviceKeys.LEFT_SHIFT, DeviceKeys.RIGHT_SHIFT }, ffxiv);
                        setModifKeys(layer, ffxiv.Actions.Where(t => t.IsAlt).ToArray(), new [] { DeviceKeys.LEFT_ALT, DeviceKeys.RIGHT_ALT }, ffxiv);
                        prevActions = ffxiv.Actions.ToList();
                        prevDevice = modif;
                        prevCastPercent = ffxiv.Player.CastingPercentage;
                    }
                    else
                        layer = prev;
                }
            }
            else
                layer = prev;
            prev = layer;
            return layer;
        }

        private List<Func<ActionStructure, bool>> getModifs()
        {
            var recordedKeys = Global.InputEvents;
            var modif = new List<Func<ActionStructure, bool>>();
            if (recordedKeys.Alt)
            {
                modif.Add(t => t.IsAlt);
            }
            else
            {
                modif.Add(t => !t.IsAlt);
            }
            if (recordedKeys.Control)
            {
                modif.Add(t => t.IsCtrl);
            }
            else
            {
                modif.Add(t => !t.IsCtrl);
            }
            if (recordedKeys.Shift)
            {
                modif.Add(t => t.IsShift);
            }
            else
            {
                modif.Add(t => !t.IsShift);
            }
            return modif;
        }

        private void setModifKeys(EffectLayer layer, ActionStructure[] actions, DeviceKeys[] deviceKeys, GameState_FFXIV ffxiv)
        {
            if (!actions.Any()) return;
            var scalar = actions.Select(t => t.GetCoolDownPrecent(ffxiv)).Max();
            layer.Set(deviceKeys, ColorUtils.MultiplyColorByScalar(Properties.PrimaryColor, scalar));
            if (actions.Any(t => t.IsProcOrCombo)) layer.Set(deviceKeys, ColorUtils.MultiplyColorByScalar(Properties.Combo, scalar));
            if (!actions.Any(t => t.InRange)) layer.Set(deviceKeys, ColorUtils.MultiplyColorByScalar(Properties.OutOfRange, scalar));
            if (!actions.Any(t => t.IsAvailable)) layer.Set(deviceKeys, Properties.NotAvailable);
        }

        public override void SetApplication(Application profile)
        {
            (Control as Control_FFXIVActionLayerHandler).SetProfile(profile);
            base.SetApplication(profile);
        }
    }
}