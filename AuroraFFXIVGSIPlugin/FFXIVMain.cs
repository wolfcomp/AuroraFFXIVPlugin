using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aurora;
using AuroraFFXIVGSIPluginTest;
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
        public JObject GSI = new JObject { { "actions", new byte[0] }, { "player", new JObject() }, { "keybinds", new JArray() } };

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
                            GSI["actions"] = actions.ActionContainers.SelectMany(t => t.ActionItems.SelectMany(f => new ActionStructure(f).ToBytes())).ToJArray().ToString(Formatting.None);
                        }
                        else
                        {
                            if (GSI["actions"].HasValues)
                                GSI["actions"] = new byte[0];
                        }
                        var player = Reader.GetCurrentPlayer();
                        var actors = Reader.GetActors();
                        var playerActor = actors.CurrentPCs.FirstOrDefault(t => t.Value.Name == player.CurrentPlayer.Name).Value;
                        GSI["player"] = JObject.FromObject(new PlayerStruct(player, playerActor));
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
                            foreach (KeyValuePair<IntPtr, IntPtr> kvp in uniqueAddresses)
                            {
                                try
                                {
                                    var characterAddress = new IntPtr(kvp.Value.ToInt64());
                                    byte[] source = MemoryHandler.Instance.GetByteArray(characterAddress, 9200);
                                    var ID = TryBitConverter.TryToUInt32(source, 116);
                                    var Type = (Actor.Type) source[140];
                                    var set = false;
                                    switch (Type)
                                    {
                                        case Actor.Type.PC:
                                            if (playerActor.ID == ID)
                                            {
                                                (GSI["player"] as JObject).Add("Status", source[6362]);
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

        public async Task MainAsync() { }

        private void WatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.Contains("FFXIV_CHR") && !e.FullPath.Contains("\\log\\"))
                ReadFiles(new FileInfo(e.FullPath).DirectoryName);
        }

        private void ReadFiles(string folder)
        {
            if (!folder.Contains("FFXIV_CHR")) throw new ArgumentException("Folder must be a FFXIV_CHR folder", nameof(folder));
            BinaryReader reader = new BinaryReader(File.Open(Path.Combine(folder, "KEYBIND.dat"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            var header = GetHeader(reader);
            var arr = new JArray();
            while (reader.BaseStream.Position < header["data_size"].ToObject<long>())
                arr.Add(ReadKeybind(reader));
            GSI["keybinds"] = arr.SelectMany(obj =>
            {
                var l = new List<byte>();
                var str = Encoding.UTF8.GetBytes(obj["command"].ToString());
                l.AddRange(BitConverter.GetBytes(str.Length));
                l.AddRange(str);
                l.Add(obj["key1"]["key"].ToObject<byte>());
                l.Add(obj["key1"]["keyraw"].ToObject<byte>());
                l.Add((byte) obj["key1"]["keymod"].ToObject<FFXIVModifierKey>());
                return l.ToArray();
            }).ToJArray().ToString(Formatting.None);
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
            reader.BaseStream.Seek(0, SeekOrigin.End);
            var fs = reader.BaseStream.Length;
            reader.BaseStream.Seek(4, SeekOrigin.Begin);
            var hfs = reader.ReadUInt32();
            if (fs - hfs != 32) throw new IOException("Inconsistent file lenght for KEYBIND.dat");
            return fs;
        }

        private JObject ReadKeybind(BinaryReader reader)
        {
            var obj = new JObject();
            obj["command"] = ReadSection(reader, 0x73)["data"].ToString();
            obj["keystr"] = ReadSection(reader, 0x73);
            var keys = obj["keystr"]["data"].ToString().Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s =>
            {
                var obj = new JObject();
                var sp = s.Split(new [] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                var b = Convert.ToByte(sp[0], 16);
                obj["key"] = (byte) (ByteToKey(b) & 0xFF);
                obj["keyraw"] = b;
                obj["keymod"] = ((FFXIVModifierKey) Convert.ToByte(sp[1], 16)).ToString();
                return obj;
            }).ToArray();
            for (var i = 0; i < keys.Length; i++)
            {
                obj["key" + (i + 1)] = keys[i];
            }
            obj.Remove("keystr");
            return obj;
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

        private short ByteToKey(byte i) => i switch
        {
            0 => -1,
            8 => 30,
            9 => 38,
            13 => 72,
            19 => 16,
            20 => 59,
            27 => 1,
            32 => 97,
            33 => 33,
            34 => 54,
            35 => 53,
            36 => 32,
            37 => 102,
            38 => 89,
            39 => 104,
            40 => 103,
            44 => 14,
            45 => 31,
            46 => 52,
            48 => 27,
            49 => 18,
            50 => 19,
            51 => 20,
            52 => 21,
            53 => 22,
            54 => 23,
            55 => 24,
            56 => 25,
            57 => 26,
            58 => 69,
            65 => 60,
            66 => 82,
            67 => 80,
            68 => 62,
            69 => 41,
            70 => 63,
            71 => 64,
            72 => 65,
            73 => 46,
            74 => 66,
            75 => 67,
            76 => 68,
            77 => 84,
            78 => 83,
            79 => 47,
            80 => 48,
            81 => 39,
            82 => 42,
            83 => 61,
            84 => 43,
            85 => 45,
            86 => 81,
            87 => 40,
            88 => 79,
            89 => 44,
            90 => 78,
            96 => 105,
            97 => 90,
            98 => 91,
            99 => 92,
            100 => 73,
            101 => 74,
            102 => 75,
            103 => 55,
            104 => 56,
            105 => 57,
            106 => 36,
            107 => 58,
            109 => 37,
            110 => 106,
            111 => 35,
            112 => 2,
            113 => 3,
            114 => 4,
            115 => 5,
            116 => 6,
            117 => 7,
            118 => 8,
            119 => 9,
            120 => 10,
            121 => 11,
            122 => 12,
            123 => 13,
            130 => 29,
            131 => 85,
            132 => 28,
            133 => 86,
            137 => 49,
            138 => 51,
            139 => 50,
            144 => 34,
            145 => 15,
            _ => i
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