using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aurora;
using Aurora.Devices;
using AuroraFFXIVGSIPlugin.FFXIV.GSI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sharlayan;
using Sharlayan.Core.Enums;
using Sharlayan.Models;
using Action = System.Action;

namespace AuroraFFXIVGSIPlugin
{
    public class FFXIVMain
    {
        public GameState_FFXIV GameState = new GameState_FFXIV();

        public event Action MemoryRead;

        private bool read = false;

        public FFXIVMain()
        {
            FileWatcher();
            ReaderTask();
        }

        private void FileWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "FINAL FANTASY XIV - A Realm Reborn");
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            watcher.Changed += WatcherOnChanged;
            watcher.Created += WatcherOnChanged;
            watcher.Deleted += WatcherOnChanged;
            watcher.Filter = "*.*";
            watcher.EnableRaisingEvents = true;
        }

        private async Task ReaderTask()
        {
            while (true)
            {
                if (read)
                {
                    try
                    {
                        var actions = Reader.GetActions();
                        if (actions.ActionContainers.Any())
                        {
                            GameState.Actions.Clear();
                            GameState.Actions.AddRange(actions.ActionContainers.SelectMany(t => t.ActionItems.Select(f => new ActionStructure(f))));
                        }
                        var player = Reader.GetCurrentPlayer();
                        var actors = Reader.GetActors();
                        var playerActor = actors.CurrentPCs.FirstOrDefault(t => t.Value.Name == player.CurrentPlayer.Name).Value;
                        var playerObj = new PlayerStruct(player, playerActor);
                        if (!Scanner.Instance.IsScanning && playerActor != null)
                        {
                            var characterAddressMap = MemoryHandler.Instance.GetByteArray(Scanner.Instance.Locations[Signatures.CharacterMapKey], 8 * 480);
                            var uniqueAddresses = new Dictionary<IntPtr, IntPtr>();
                            for (var i = 0; i < 480; i++)
                            {
                                var characterAddress = new IntPtr(TryBitConverter.TryToInt64(characterAddressMap, i * 8));

                                if (characterAddress == IntPtr.Zero)
                                {
                                    continue;
                                }

                                uniqueAddresses[characterAddress] = characterAddress;
                            }
                            foreach (var (key, address) in uniqueAddresses)
                            {
                                try
                                {
                                    var characterAddress = new IntPtr(address.ToInt64());
                                    byte[] source = MemoryHandler.Instance.GetByteArray(characterAddress, 9200);
                                    var ID = TryBitConverter.TryToUInt32(source, 116);
                                    var Type = (Actor.Type) source[140];
                                    var set = false;
                                    switch (Type)
                                    {
                                        case Actor.Type.PC:
                                            if (playerActor.ID == ID)
                                            {
                                                playerObj = new PlayerStruct(playerObj, source[6362]);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (set)
                                        break;
                                }
                                catch (Exception)
                                {
                                    //
                                }
                            }
                        }
                        GameState.Player = playerObj;
                        //TODO add in party when there is a way to sort based on the ingame sort
                        //var party = Reader.GetPartyMembers();
                        //var partyActors = party.PartyMembers.Select(t => actors.CurrentPCs[t.Key]).ToList();
                        //gsiJObject["party"] = JToken.Parse(JsonConvert.SerializeObject(partyActors, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
                        MemoryRead?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Global.logger.Error(e, "[FFXIVPlugin] Memory reading failed");
                    }
                    await Task.Delay(TimeSpan.FromMilliseconds(5));
                }
                else
                    await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        public void StartReading()
        {
            SetProcess();
            read = true;
        }

        public void StopReading()
        {
            read = false;
        }

        private void WatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.Contains("FFXIV_CHR") && !e.FullPath.Contains("\\log\\"))
                ReadFiles(new FileInfo(e.FullPath).DirectoryName);
        }

        #region Read KEYBIND.dat
        private void ReadFiles(string folder)
        {
            if (!folder.Contains("FFXIV_CHR")) throw new ArgumentException("Folder must be a FFXIV_CHR folder", nameof(folder));
            BinaryReader reader = new BinaryReader(File.Open(Path.Combine(folder, "KEYBIND.dat"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            var header = GetHeader(reader);
            GameState.KeyBinds.Clear();
            while (reader.BaseStream.Position < header["data_size"].ToObject<long>())
                ReadKeybind(reader);
        }

        private JObject GetHeader(BinaryReader reader)
        {
            var obj = new JObject();
            obj["file_size"] = CheckHeader(reader);
            obj["data_size"] = reader.ReadUInt32();
            reader.BaseStream.Seek(0x11, SeekOrigin.Begin);
            return obj;
        }

        private long CheckHeader(BinaryReader reader)
        {
            var fs = reader.BaseStream.Length;
            reader.BaseStream.Seek(4, SeekOrigin.Begin);
            var hfs = reader.ReadUInt32();
            if (fs - hfs != 32) throw new IOException("Inconsistent file lenght for KEYBIND.dat");
            return fs;
        }

        private void ReadKeybind(BinaryReader reader)
        {
            var command = ReadSection(reader, 0x73)["data"].ToString();
            var keys = ReadSection(reader, 0x73)["data"].ToString().Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s =>
            {
                var sp = s.Split(new [] { '.' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToByte(t, 16)).ToArray();
                var obj = new KeyBindStructure(ByteToKey(sp[0]), sp[0], (FFXIVModifierKey) sp[1]);
                return obj;
            }).ToArray();
            GameState.KeyBinds.Add(new KeyBindStructure(command, keys));
        }

        private JObject ReadSection(BinaryReader reader, byte xor)
        {
            var obj = new JObject();
            var header = Xor(reader.ReadChars(3), xor).Select(t => (byte) t).ToArray();
            obj["type"] = header[0];
            var size = BitConverter.ToUInt16(header, 1);
            var data = Xor(reader.ReadChars(size), xor);
            obj["data"] = new string(data.Take(data.Length - 1).ToArray());
            return obj;
        }

        private char[] Xor(char[] input, byte xor) => input.Select(c => (char) (c ^ xor)).ToArray();
        #endregion

        private void SetProcess()
        {
            Process[] processes = Process.GetProcessesByName("ffxiv_dx11");
            if (processes.Any())
            {
                // supported: English, Chinese, Japanese, French, German, Korean
                string gameLanguage = "English";
                // whether to always hit API on start to get the latest sigs based on patchVersion, or use the local json cache (if the file doesn't exist, API will be hit)
                bool useLocalCache = true;
                // patchVersion of game, or latest
                string patchVersion = "latest";
                Process process = processes[0];
                ProcessModel processModel = new ProcessModel
                {
                    Process = process,
                    IsWin64 = true
                };
                MemoryHandler.Instance.SetProcess(processModel, gameLanguage, patchVersion, useLocalCache);
            }
        }

        private DeviceKeys ByteToKey(byte i) => i switch
        {
            0 => DeviceKeys.NONE,
            8 => DeviceKeys.BACKSPACE,
            9 => DeviceKeys.TAB,
            13 => DeviceKeys.ENTER,
            19 => DeviceKeys.PAUSE_BREAK,
            20 => DeviceKeys.CAPS_LOCK,
            27 => DeviceKeys.ESC,
            32 => DeviceKeys.SPACE,
            33 => DeviceKeys.PAGE_UP,
            34 => DeviceKeys.PAGE_DOWN,
            35 => DeviceKeys.END,
            36 => DeviceKeys.HOME,
            37 => DeviceKeys.ARROW_LEFT,
            38 => DeviceKeys.ARROW_UP,
            39 => DeviceKeys.ARROW_RIGHT,
            40 => DeviceKeys.ARROW_DOWN,
            44 => DeviceKeys.PRINT_SCREEN,
            45 => DeviceKeys.INSERT,
            46 => DeviceKeys.DELETE,
            48 => DeviceKeys.ZERO,
            49 => DeviceKeys.ONE,
            50 => DeviceKeys.TWO,
            51 => DeviceKeys.THREE,
            52 => DeviceKeys.FOUR,
            53 => DeviceKeys.FIVE,
            54 => DeviceKeys.SIX,
            55 => DeviceKeys.SEVEN,
            56 => DeviceKeys.EIGHT,
            57 => DeviceKeys.NINE,
            58 => DeviceKeys.SEMICOLON,
            65 => DeviceKeys.A,
            66 => DeviceKeys.B,
            67 => DeviceKeys.C,
            68 => DeviceKeys.D,
            69 => DeviceKeys.E,
            70 => DeviceKeys.F,
            71 => DeviceKeys.G,
            72 => DeviceKeys.H,
            73 => DeviceKeys.I,
            74 => DeviceKeys.J,
            75 => DeviceKeys.K,
            76 => DeviceKeys.L,
            77 => DeviceKeys.M,
            78 => DeviceKeys.N,
            79 => DeviceKeys.O,
            80 => DeviceKeys.P,
            81 => DeviceKeys.Q,
            82 => DeviceKeys.R,
            83 => DeviceKeys.S,
            84 => DeviceKeys.T,
            85 => DeviceKeys.U,
            86 => DeviceKeys.V,
            87 => DeviceKeys.W,
            88 => DeviceKeys.X,
            89 => DeviceKeys.Y,
            90 => DeviceKeys.Z,
            96 => DeviceKeys.NUM_ZERO,
            97 => DeviceKeys.NUM_ONE,
            98 => DeviceKeys.NUM_TWO,
            99 => DeviceKeys.NUM_THREE,
            100 => DeviceKeys.NUM_FOUR,
            101 => DeviceKeys.NUM_FIVE,
            102 => DeviceKeys.NUM_SIX,
            103 => DeviceKeys.NUM_SEVEN,
            104 => DeviceKeys.NUM_EIGHT,
            105 => DeviceKeys.NUM_NINE,
            106 => DeviceKeys.NUM_ASTERISK,
            107 => DeviceKeys.NUM_PLUS,
            109 => DeviceKeys.NUM_MINUS,
            110 => DeviceKeys.NUM_PERIOD,
            111 => DeviceKeys.NUM_SLASH,
            112 => DeviceKeys.F1,
            113 => DeviceKeys.F2,
            114 => DeviceKeys.F3,
            115 => DeviceKeys.F4,
            116 => DeviceKeys.F5,
            117 => DeviceKeys.F6,
            118 => DeviceKeys.F7,
            119 => DeviceKeys.F8,
            120 => DeviceKeys.F9,
            121 => DeviceKeys.F10,
            122 => DeviceKeys.F11,
            123 => DeviceKeys.F12,
            130 => DeviceKeys.EQUALS,
            131 => DeviceKeys.COMMA,
            132 => DeviceKeys.MINUS,
            133 => DeviceKeys.PERIOD,
            137 => DeviceKeys.OPEN_BRACKET,
            138 => DeviceKeys.BACKSLASH,
            139 => DeviceKeys.CLOSE_BRACKET,
            144 => DeviceKeys.NUM_LOCK,
            145 => DeviceKeys.SCROLL_LOCK,
            _ => DeviceKeys.NONE
        };
    }

    [Flags]
    public enum FFXIVModifierKey
    {
        NONE,
        SHIFT,
        CTRL,
        ALT = 4
    }

    public static class Extensions
    {
        public static JArray ToJArray<T>(this IEnumerable<T> bytes)
        {
            return JArray.FromObject(bytes.Select(t => new JValue(t)));
        }

        public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> pair, out T1 key, out T2 value)
        {
            key = pair.Key;
            value = pair.Value;
        }

        public static string ToHex(this int i, int count = 2)
        {
            var k = i.ToString("X");
            if (k.Length < count) k = new string('0', count - k.Length) + k;
            return k;
        }

        public static string ToHex(this byte i)
        {
            var k = i.ToString("X");
            if (k.Length == 1) k = "0" + k;
            return k;
        }
    }

    internal static class TryBitConverter
    {
        public static bool TryToBoolean(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToBoolean(value, index);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static char TryToChar(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToChar(value, index);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static double TryToDouble(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToDouble(value, index);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static long TryToDoubleToInt64Bits(double value)
        {
            try
            {
                return System.BitConverter.DoubleToInt64Bits(value);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static short TryToInt16(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToInt16(value, index);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static int TryToInt32(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToInt32(value, index);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static long TryToInt64(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToInt64(value, index);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static double TryToInt64BitsToDouble(long value)
        {
            try
            {
                return System.BitConverter.Int64BitsToDouble(value);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static float TryToSingle(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToSingle(value, index);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static string TryToString(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToString(value, index);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static ushort TryToUInt16(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToUInt16(value, index);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static uint TryToUInt32(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToUInt32(value, index);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static ulong TryToUInt64(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToUInt64(value, index);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}