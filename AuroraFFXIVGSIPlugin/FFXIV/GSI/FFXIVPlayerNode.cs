using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aurora.Profiles;

namespace AuroraFFXIVGSIPlugin.FFXIV.GSI
{
    public class FFXIVPlayerNode : Node<FFXIVPlayerNode>
    {
        public JobType JobType;
        public int HPCurrent;
        public int HPMax;
        public int MPCurrent;
        public int MPMax;
        public int CPCurrent;
        public int CPMax;
        public int GPCurrent;
        public int GPMax;
        public double CastingPercentage;
        public bool InCombat;
        public bool WeaponUnsheathed;
        public double LevelProgression;
        public byte Status;

        internal FFXIVPlayerNode() : base()
        {

        }

        internal FFXIVPlayerNode(string json) : base(json)
        {
            JobType = GetEnum<JobType>("JobType");
            HPCurrent = Get<int>("HPCurrent");
            HPMax = Get<int>("HPMax");
            MPCurrent = Get<int>("MPCurrent");
            MPMax = Get<int>("MPMax");
            CPCurrent = Get<int>("CPCurrent");
            CPMax = Get<int>("CPMax");
            GPCurrent = Get<int>("GPCurrent");
            GPMax = Get<int>("GPMax");
            CastingPercentage = Get<double>("CastingPercentage");
            InCombat = Get<bool>("InCombat");
            WeaponUnsheathed = Get<bool>("WeaponUnsheathed");
            LevelProgression = Get<double>("LevelProgression");
            Status = Get<byte>("Status");
        }
    }

    public enum JobType
    {
        Unknown,
        Tank,
        Healer,
        MeleeDps,
        RangedDps,
        CasterDps,
        Crafter,
        Gatherer
    }
}
