using Aurora.Profiles;
using Sharlayan.Core;
using Sharlayan.Core.Enums;
using Sharlayan.Models.ReadResults;

namespace AuroraFFXIVPlugin
{
    public class PlayerStruct : Node
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
        public byte Status;
        public double LevelProgression;

        public PlayerStruct()
        {
            JobType = JobType.Unknown;
        }

        public PlayerStruct(CurrentPlayerResult currentPlayer, ActorItem playerActor)
        {
            if (playerActor != null)
            {
                WeaponUnsheathed = (playerActor.CombatFlags & 4U) > 0U;
                InCombat = playerActor.InCombat;
                CastingPercentage = playerActor.CastingPercentage;
                HPCurrent = playerActor.HPCurrent;
                HPMax = playerActor.HPMax;
                MPCurrent = playerActor.MPCurrent;
                MPMax = playerActor.MPMax;
                CPCurrent = playerActor.CPCurrent;
                CPMax = playerActor.CPMax;
                GPCurrent = playerActor.GPCurrent;
                GPMax = playerActor.GPMax;
            }
            else
            {
                InCombat = WeaponUnsheathed = false;
                CastingPercentage = HPCurrent = HPMax = MPCurrent = MPMax = CPCurrent = CPMax = GPCurrent = GPMax = 0;
            }
            getLevelProgression(currentPlayer);
            Status = 0;
        }

        public PlayerStruct(PlayerStruct player, byte status)
        {
            WeaponUnsheathed = player.WeaponUnsheathed;
            InCombat = player.InCombat;
            CastingPercentage = player.CastingPercentage;
            HPCurrent = player.HPCurrent;
            HPMax = player.HPMax;
            MPCurrent = player.MPCurrent;
            MPMax = player.MPMax;
            CPCurrent = player.CPCurrent;
            CPMax = player.CPMax;
            GPCurrent = player.GPCurrent;
            GPMax = player.GPMax;
            JobType = player.JobType;
            Status = status;
            LevelProgression = player.LevelProgression;
        }

        private double getEXPFromLevel(int level)
        {
            return level switch
            {
                1 => 300,
                2 => 600,
                3 => 1100,
                4 => 1700,
                5 => 2300,
                6 => 4200,
                7 => 6000,
                8 => 7350,
                9 => 9930,
                10 => 11800,
                11 => 15600,
                12 => 19600,
                13 => 23700,
                14 => 26400,
                15 => 30500,
                16 => 35400,
                17 => 40500,
                18 => 45700,
                19 => 51000,
                20 => 56600,
                21 => 63900,
                22 => 71400,
                23 => 79100,
                24 => 87100,
                25 => 95200,
                26 => 109800,
                27 => 124800,
                28 => 140200,
                29 => 155900,
                30 => 162500,
                31 => 175900,
                32 => 189600,
                33 => 203500,
                34 => 217900,
                35 => 232320,
                36 => 249900,
                37 => 267800,
                38 => 286200,
                39 => 304900,
                40 => 324000,
                41 => 340200,
                42 => 356800,
                43 => 373700,
                44 => 390800,
                45 => 408200,
                46 => 437600,
                47 => 467500,
                48 => 498000,
                49 => 529000,
                50 => 864000,
                51 => 1058400,
                52 => 1267200,
                53 => 1555200,
                54 => 1872000,
                55 => 2217600,
                56 => 2592000,
                57 => 2995200,
                58 => 3427200,
                59 => 3888000,
                60 => 4470000,
                61 => 4873000,
                62 => 5316000,
                63 => 5809000,
                64 => 6364000,
                65 => 6995000,
                66 => 7722000,
                67 => 8575000,
                68 => 9593000,
                69 => 10826000,
                70 => 12449000,
                71 => 13881000,
                72 => 15556000,
                73 => 17498600,
                74 => 19750000,
                75 => 22330000,
                76 => 25340000,
                77 => 28650000,
                78 => 32750000,
                79 => 37650000,
                80 => 0,
                _ => 0
            };
        }

        private void getLevelProgression(CurrentPlayerResult currentPlayer)
        {
            var EXP = 0;
            var Level = 0;
            switch (currentPlayer.CurrentPlayer.Job)
            {
                case Actor.Job.GLD:
                    EXP = currentPlayer.CurrentPlayer.GLD_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.GLD;
                    JobType = JobType.Tank;
                    break;
                case Actor.Job.PGL:
                    EXP = currentPlayer.CurrentPlayer.PGL_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.PGL;
                    JobType = JobType.MeleeDps;
                    break;
                case Actor.Job.MRD:
                    EXP = currentPlayer.CurrentPlayer.MRD_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.MRD;
                    JobType = JobType.Tank;
                    break;
                case Actor.Job.LNC:
                    EXP = currentPlayer.CurrentPlayer.LNC_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.LNC;
                    JobType = JobType.MeleeDps;
                    break;
                case Actor.Job.ARC:
                    EXP = currentPlayer.CurrentPlayer.ARC_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.ARC;
                    JobType = JobType.RangedDps;
                    break;
                case Actor.Job.CNJ:
                    EXP = currentPlayer.CurrentPlayer.CNJ_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.CNJ;
                    JobType = JobType.Healer;
                    break;
                case Actor.Job.THM:
                    EXP = currentPlayer.CurrentPlayer.THM_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.THM;
                    JobType = JobType.CasterDps;
                    break;
                case Actor.Job.CPT:
                    EXP = currentPlayer.CurrentPlayer.CPT_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.CPT;
                    JobType = JobType.Crafter;
                    break;
                case Actor.Job.BSM:
                    EXP = currentPlayer.CurrentPlayer.BSM_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.BSM;
                    JobType = JobType.Crafter;
                    break;
                case Actor.Job.ARM:
                    EXP = currentPlayer.CurrentPlayer.ARM_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.ARM;
                    JobType = JobType.Crafter;
                    break;
                case Actor.Job.GSM:
                    EXP = currentPlayer.CurrentPlayer.GSM_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.GSM;
                    JobType = JobType.Crafter;
                    break;
                case Actor.Job.LTW:
                    EXP = currentPlayer.CurrentPlayer.LTW_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.LTW;
                    JobType = JobType.Crafter;
                    break;
                case Actor.Job.WVR:
                    EXP = currentPlayer.CurrentPlayer.WVR_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.WVR;
                    JobType = JobType.Crafter;
                    break;
                case Actor.Job.ALC:
                    EXP = currentPlayer.CurrentPlayer.ALC_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.ALC;
                    JobType = JobType.Crafter;
                    break;
                case Actor.Job.CUL:
                    EXP = currentPlayer.CurrentPlayer.CUL_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.CUL;
                    JobType = JobType.Crafter;
                    break;
                case Actor.Job.MIN:
                    EXP = currentPlayer.CurrentPlayer.MIN_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.MIN;
                    JobType = JobType.Gatherer;
                    break;
                case Actor.Job.BTN:
                    EXP = currentPlayer.CurrentPlayer.BTN_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.BTN;
                    JobType = JobType.Gatherer;
                    break;
                case Actor.Job.FSH:
                    EXP = currentPlayer.CurrentPlayer.FSH_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.FSH;
                    JobType = JobType.Gatherer;
                    break;
                case Actor.Job.PLD:
                    EXP = currentPlayer.CurrentPlayer.GLD_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.GLD;
                    JobType = JobType.Tank;
                    break;
                case Actor.Job.MNK:
                    EXP = currentPlayer.CurrentPlayer.PGL_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.PGL;
                    JobType = JobType.MeleeDps;
                    break;
                case Actor.Job.WAR:
                    EXP = currentPlayer.CurrentPlayer.MRD_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.MRD;
                    JobType = JobType.Tank;
                    break;
                case Actor.Job.DRG:
                    EXP = currentPlayer.CurrentPlayer.LNC_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.LNC;
                    JobType = JobType.MeleeDps;
                    break;
                case Actor.Job.BRD:
                    EXP = currentPlayer.CurrentPlayer.ARC_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.ARC;
                    JobType = JobType.RangedDps;
                    break;
                case Actor.Job.WHM:
                    EXP = currentPlayer.CurrentPlayer.CNJ_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.CNJ;
                    JobType = JobType.Healer;
                    break;
                case Actor.Job.BLM:
                    EXP = currentPlayer.CurrentPlayer.THM_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.THM;
                    JobType = JobType.CasterDps;
                    break;
                case Actor.Job.ACN:
                    EXP = currentPlayer.CurrentPlayer.ACN_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.ACN;
                    JobType = JobType.CasterDps;
                    break;
                case Actor.Job.SMN:
                    EXP = currentPlayer.CurrentPlayer.ACN_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.ACN;
                    JobType = JobType.CasterDps;
                    break;
                case Actor.Job.SCH:
                    EXP = currentPlayer.CurrentPlayer.ACN_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.ACN;
                    JobType = JobType.Healer;
                    break;
                case Actor.Job.ROG:
                    EXP = currentPlayer.CurrentPlayer.ROG_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.ROG;
                    JobType = JobType.MeleeDps;
                    break;
                case Actor.Job.NIN:
                    EXP = currentPlayer.CurrentPlayer.ROG_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.ROG;
                    JobType = JobType.MeleeDps;
                    break;
                case Actor.Job.MCH:
                    EXP = currentPlayer.CurrentPlayer.MCH_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.MCH;
                    JobType = JobType.CasterDps;
                    break;
                case Actor.Job.DRK:
                    EXP = currentPlayer.CurrentPlayer.DRK_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.DRK;
                    JobType = JobType.Tank;
                    break;
                case Actor.Job.AST:
                    EXP = currentPlayer.CurrentPlayer.AST_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.AST;
                    JobType = JobType.Healer;
                    break;
                case Actor.Job.SAM:
                    EXP = currentPlayer.CurrentPlayer.SAM_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.SAM;
                    JobType = JobType.MeleeDps;
                    break;
                case Actor.Job.RDM:
                    EXP = currentPlayer.CurrentPlayer.RDM_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.RDM;
                    JobType = JobType.CasterDps;
                    break;
                case Actor.Job.BLU:
                    EXP = currentPlayer.CurrentPlayer.BLU_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.BLU;
                    JobType = JobType.CasterDps;
                    break;
                case Actor.Job.GNB:
                    EXP = currentPlayer.CurrentPlayer.GNB_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.GNB;
                    JobType = JobType.Tank;
                    break;
                case Actor.Job.DNC:
                    EXP = currentPlayer.CurrentPlayer.DNC_CurrentEXP;
                    Level = currentPlayer.CurrentPlayer.DNC;
                    JobType = JobType.RangedDps;
                    break;
                default:
                    EXP = 0;
                    Level = 0;
                    JobType = JobType.Unknown;
                    break;
            }
            LevelProgression = getEXPFromLevel(Level) > 0 ? EXP / getEXPFromLevel(Level) : 1;
        }
    }

    public enum JobType : byte
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