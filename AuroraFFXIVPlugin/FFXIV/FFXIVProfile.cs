using System.Collections.ObjectModel;
using System.Drawing;
using Aurora.Devices;
using Aurora.Settings;
using Aurora.Settings.Layers;
using Aurora.Settings.Overrides.Logic;
using Aurora.Settings.Overrides.Logic.Builder;
using AuroraFFXIVPlugin.FFXIV.Layers;

namespace AuroraFFXIVPlugin
{
    public class FFXIVProfile : ApplicationProfile
    {
        public FFXIVProfile() : base() { }

        public override void Reset()
        {
            base.Reset();
            Layers = new ObservableCollection<Layer>
            {
                new Layer("Status Layer", new SolidFillLayerHandler
                          {
                              Properties = new SolidFillLayerHandlerProperties
                              {
                                  _PrimaryColor = Color.Transparent
                              }
                          },
                    new OverrideLogicBuilder().SetLookupTable("_PrimaryColor", new OverrideLookupTableBuilder<Color>()
                        .AddEntry(Color.FromArgb(0, 0, 0, 0), new BooleanGSIEnum { StatePath = "Player/Status", EnumValue = OnlineStatus.None })
                        .AddEntry(Color.FromArgb(255, 0, 0), new BooleanGSIEnum { StatePath = "Player/Status", EnumValue = OnlineStatus.Busy })
                        .AddEntry(Color.FromArgb(0, 0, 0), new BooleanGSIEnum { StatePath = "Player/Status", EnumValue = OnlineStatus.AFK })
                        .AddEntry(Color.FromArgb(255, 255, 0), new BooleanGSIEnum { StatePath = "Player/Status", EnumValue = OnlineStatus.LookingtoMeldMateria })
                        .AddEntry(Color.FromArgb(255, 135, 0), new BooleanGSIEnum { StatePath = "Player/Status", EnumValue = OnlineStatus.RolePlaying })
                        .AddEntry(Color.FromArgb(0, 255, 41), new BooleanGSIEnum { StatePath = "Player/Status", EnumValue = OnlineStatus.LookingforParty }))),
                new Layer("Action Layer", new FFXIVActionLayerHandler()),
                new Layer("Health Indicator Layer", new PercentLayerHandler
                {
                    Properties = new PercentLayerHandlerProperties
                    {
                        _PrimaryColor =  Color.FromArgb(0, 255, 0),
                        _SecondaryColor = Color.FromArgb(255, 0, 0),
                        _PercentType = PercentEffectType.Progressive_Gradual,
                        _Sequence = new KeySequence(new []
                        {
                            DeviceKeys.F1, DeviceKeys.F2, DeviceKeys.F3, DeviceKeys.F4
                        }),
                        _BlinkThreshold = 0.0,
                        _BlinkDirection = false,
                        _VariablePath = "Player/HPCurrent",
                        _MaxVariablePath = "Player/HPMax"
                    }
                }),
                new Layer("MP Indicator Layer", new PercentLayerHandler
                {
                    Properties = new PercentLayerHandlerProperties
                    {
                        _PrimaryColor =  Color.FromArgb(0, 255, 255),
                        _SecondaryColor = Color.FromArgb(0, 0, 0, 0),
                        _PercentType = PercentEffectType.Progressive_Gradual,
                        _Sequence = new KeySequence(new []
                        {
                            DeviceKeys.F5, DeviceKeys.F6, DeviceKeys.F7, DeviceKeys.F8
                        }),
                        _BlinkThreshold = 0.0,
                        _BlinkDirection = false,
                        _VariablePath = "Player/MPCurrent",
                        _MaxVariablePath = "10000"
                    }
                }),
                new Layer("GP Indicator Layer", new PercentLayerHandler
                {
                    Properties = new PercentLayerHandlerProperties
                    {
                        _PrimaryColor =  Color.FromArgb(0, 255, 255),
                        _SecondaryColor = Color.FromArgb(0, 0, 0, 0),
                        _PercentType = PercentEffectType.Progressive_Gradual,
                        _Sequence = new KeySequence(new []
                        {
                            DeviceKeys.F5, DeviceKeys.F6, DeviceKeys.F7, DeviceKeys.F8
                        }),
                        _BlinkThreshold = 0.0,
                        _BlinkDirection = false,
                        _VariablePath = "Player/GPCurrent",
                        _MaxVariablePath = "Player/GPMax"
                    }
                }),
                new Layer("CP Indicator Layer", new PercentLayerHandler
                {
                    Properties = new PercentLayerHandlerProperties
                    {
                        _PrimaryColor =  Color.FromArgb(0, 255, 255),
                        _SecondaryColor = Color.FromArgb(0, 0, 0, 0),
                        _PercentType = PercentEffectType.Progressive_Gradual,
                        _Sequence = new KeySequence(new []
                        {
                            DeviceKeys.F5, DeviceKeys.F6, DeviceKeys.F7, DeviceKeys.F8
                        }),
                        _BlinkThreshold = 0.0,
                        _BlinkDirection = false,
                        _VariablePath = "Player/CPCurrent",
                        _MaxVariablePath = "Player/CPMax"
                    }
                }),
                new Layer("Cast Indicator Layer", new PercentLayerHandler
                {
                    Properties = new PercentLayerHandlerProperties
                    {
                        _PrimaryColor =  Color.FromArgb(255, 255, 0),
                        _SecondaryColor = Color.FromArgb(0, 0, 0, 0),
                        _PercentType = PercentEffectType.Progressive_Gradual,
                        _Sequence = new KeySequence(new []
                        {
                            DeviceKeys.F9, DeviceKeys.F10, DeviceKeys.F11, DeviceKeys.F12
                        }),
                        _BlinkThreshold = 0.0,
                        _BlinkDirection = false,
                        _VariablePath = "Player/CastingPercentage",
                        _MaxVariablePath = "1"
                    }
                }),
                new Layer("Level Progress Layer", new PercentLayerHandler
                {
                    Properties = new PercentLayerHandlerProperties
                    {
                        _PrimaryColor =  Color.FromArgb(255, 255, 255),
                        _SecondaryColor = Color.FromArgb(0, 0, 0, 0),
                        _PercentType = PercentEffectType.Progressive_Gradual,
                        _Sequence = new KeySequence(new []
                        {
                            DeviceKeys.ADDITIONALLIGHT1, DeviceKeys.ADDITIONALLIGHT2, DeviceKeys.ADDITIONALLIGHT3, DeviceKeys.ADDITIONALLIGHT4, DeviceKeys.ADDITIONALLIGHT5,
                            DeviceKeys.ADDITIONALLIGHT6, DeviceKeys.ADDITIONALLIGHT7, DeviceKeys.ADDITIONALLIGHT8, DeviceKeys.ADDITIONALLIGHT9, DeviceKeys.ADDITIONALLIGHT10,
                            DeviceKeys.ADDITIONALLIGHT11, DeviceKeys.ADDITIONALLIGHT12, DeviceKeys.ADDITIONALLIGHT13, DeviceKeys.ADDITIONALLIGHT14, DeviceKeys.ADDITIONALLIGHT15,
                            DeviceKeys.ADDITIONALLIGHT16, DeviceKeys.ADDITIONALLIGHT17, DeviceKeys.ADDITIONALLIGHT18, DeviceKeys.ADDITIONALLIGHT19
                        }),
                        _BlinkThreshold = 0.0,
                        _BlinkDirection = false,
                        _VariablePath = "Player/LevelProgression",
                        _MaxVariablePath = "1"
                    }
                }),
                new Layer("In Combat Layer", new SolidColorLayerHandler
                    {
                        Properties = new LayerHandlerProperties
                        {
                            _PrimaryColor = Color.FromArgb(255, 0, 52),
                            _Sequence = new KeySequence(new []
                            {
                                DeviceKeys.R, DeviceKeys.T, DeviceKeys.Y, DeviceKeys.U, DeviceKeys.I, DeviceKeys.O,
                                DeviceKeys.F, DeviceKeys.G, DeviceKeys.H, DeviceKeys.J, DeviceKeys.K, DeviceKeys.L,
                                DeviceKeys.V, DeviceKeys.B, DeviceKeys.N, DeviceKeys.M, DeviceKeys.COMMA, DeviceKeys.PERIOD
                            })
                        }
                    },
                    new OverrideLogicBuilder().SetDynamicBoolean("_Enabled", new BooleanGSIBoolean { VariablePath = "Player/InCombat" })),
                new Layer("Weapon Unshethed Layer", new SolidColorLayerHandler
                    {
                        Properties = new LayerHandlerProperties
                        {
                            _PrimaryColor = Color.FromArgb(105, 105, 105),
                            _Sequence = new KeySequence(new []
                            {
                                DeviceKeys.R, DeviceKeys.T, DeviceKeys.Y, DeviceKeys.U, DeviceKeys.I, DeviceKeys.O,
                                DeviceKeys.F, DeviceKeys.G, DeviceKeys.H, DeviceKeys.J, DeviceKeys.K, DeviceKeys.L,
                                DeviceKeys.V, DeviceKeys.B, DeviceKeys.N, DeviceKeys.M, DeviceKeys.COMMA, DeviceKeys.PERIOD
                            })
                        }
                    },
                    new OverrideLogicBuilder().SetDynamicBoolean("_Enabled", new BooleanGSIBoolean { VariablePath = "Player/WeaponUnsheathed" })),
                new Layer("Key Bind Layer", new FFXIVKeyBindLayerHandler
                {
                    Properties = new KeyBindLayerHandlerProperties
                    {
                        _PrimaryColor = Color.Indigo,
                        _Ignore = true
                    }
                }),
                new Layer("Class Background Layer", new SolidFillLayerHandler(),
                    new OverrideLogicBuilder()
                        .SetLookupTable("_PrimaryColor", new OverrideLookupTableBuilder<Color>()
                            .AddEntry(Color.FromArgb(255, 122, 45, 102), new BooleanGSIEnum { StatePath = "Player/JobType", EnumValue = JobType.Unknown })
                            .AddEntry(Color.FromArgb(255, 0, 0, 128), new BooleanGSIEnum { StatePath = "Player/JobType", EnumValue = JobType.Tank })
                            .AddEntry(Color.FromArgb(255, 0, 128, 0), new BooleanGSIEnum { StatePath = "Player/JobType", EnumValue = JobType.Healer })
                            .AddEntry(Color.FromArgb(255, 128, 0, 0), new BooleanGSIEnum { StatePath = "Player/JobType", EnumValue = JobType.MeleeDps })
                            .AddEntry(Color.FromArgb(255, 128, 0, 0), new BooleanGSIEnum { StatePath = "Player/JobType", EnumValue = JobType.RangedDps })
                            .AddEntry(Color.FromArgb(255, 128, 0, 0), new BooleanGSIEnum { StatePath = "Player/JobType", EnumValue = JobType.CasterDps })
                            .AddEntry(Color.FromArgb(255, 101, 128, 0), new BooleanGSIEnum { StatePath = "Player/JobType", EnumValue = JobType.Crafter })
                            .AddEntry(Color.FromArgb(255, 0, 101, 128), new BooleanGSIEnum { StatePath = "Player/JobType", EnumValue = JobType.Gatherer })
                        ))
            };
        }
    }
}