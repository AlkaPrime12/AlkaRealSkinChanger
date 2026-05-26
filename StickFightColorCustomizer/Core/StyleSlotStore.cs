using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using StickFightColorCustomizer.Models;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    public static class StyleSlotStore
    {
        private const string ManifestFileName = "manifest.json";
        private const int DefaultSlotCount = 3;
        private const int MaxSlots = 24;

        private static StyleSlotManifest _manifest;

        public static string SlotsDirectory
        {
            get { return ModStoragePaths.SlotsRoot; }
        }

        public static StyleSlotManifest GetManifest()
        {
            if (_manifest == null)
            {
                _manifest = LoadManifest();
            }

            return _manifest;
        }

        public static void ReloadManifest()
        {
            _manifest = LoadManifest();
        }

        public static void SaveAll(ColorConfig currentConfig)
        {
            try
            {
                if (currentConfig != null)
                {
                    ColorConfigStore.Save(currentConfig);
                }

                WriteManifest(GetManifest());
            }
            catch (Exception ex)
            {
                ModLog.Warning("SFCC: SaveAll slots: " + ex.Message);
            }
        }

        public static bool HasSlot(int index)
        {
            return index >= 1 && File.Exists(ModStoragePaths.GetSlotFilePath(index));
        }

        public static bool SaveSlot(int index, ColorConfig config, string displayName)
        {
            if (config == null || index < 1)
            {
                ModLog.Warning("SFCC: SaveSlot rechazado (config null o index " + index + ")");
                return false;
            }

            try
            {
                ModStoragePaths.EnsureSlotsDirectory();
                EnsureManifestSlot(index, displayName);

                ColorConfig snapshot = ColorConfigStore.ParseFromJson(ColorConfigStore.SerializeToJson(config));
                if (snapshot == null)
                {
                    ModLog.Warning("SFCC: SaveSlot no pudo serializar config para slot " + index);
                    return false;
                }

                snapshot.BodyCustomizationActive = true;
                string path = ModStoragePaths.GetSlotFilePath(index);
                File.WriteAllText(path, ColorConfigStore.SerializeToJson(snapshot), Encoding.UTF8);

                StyleSlotEntry entry = FindEntry(index);
                if (entry != null)
                {
                    entry.Index = index;
                    entry.HasData = true;
                    if (!string.IsNullOrEmpty(displayName))
                    {
                        entry.Name = displayName.Trim();
                    }
                }

                NormalizeManifest(GetManifest());
                WriteManifest(GetManifest());
                ModLog.Info("SFCC: slot " + index + " guardado en " + path);
                return true;
            }
            catch (Exception ex)
            {
                ModLog.Warning("SFCC: no se pudo guardar slot " + index + ": " + ex.Message);
                return false;
            }
        }

        public static bool LoadSlot(int index, out ColorConfig config)
        {
            config = null;
            if (index < 1)
            {
                return false;
            }

            string path = ModStoragePaths.GetSlotFilePath(index);
            if (!File.Exists(path))
            {
                return false;
            }

            try
            {
                string json = File.ReadAllText(path, Encoding.UTF8);
                config = ColorConfigStore.ParseFromJson(json);
                return config != null;
            }
            catch (Exception ex)
            {
                ModLog.Warning("SFCC: no se pudo cargar slot " + index + ": " + ex.Message);
                return false;
            }
        }

        public static bool AddSlot()
        {
            StyleSlotManifest manifest = GetManifest();
            NormalizeManifest(manifest);
            if (manifest.Slots.Count >= MaxSlots)
            {
                return false;
            }

            int next = manifest.Slots.Count + 1;
            manifest.Slots.Add(new StyleSlotEntry
            {
                Index = next,
                Name = "Slot " + next,
                HasData = false
            });
            WriteManifest(manifest);
            return true;
        }

        public static void SetSlotName(int index, string name)
        {
            if (index < 1)
            {
                return;
            }

            EnsureManifestSlot(index, name);
            StyleSlotEntry entry = FindEntry(index);
            if (entry != null)
            {
                entry.Index = index;
                entry.Name = string.IsNullOrEmpty(name) ? "Slot " + index : name.Trim();
            }

            NormalizeManifest(GetManifest());
            WriteManifest(GetManifest());
        }

        public static string GetSlotName(int index)
        {
            StyleSlotEntry entry = FindEntry(index);
            return entry != null && !string.IsNullOrEmpty(entry.Name) ? entry.Name : "Slot " + index;
        }

        private static void EnsureManifestSlot(int index, string name)
        {
            StyleSlotManifest manifest = GetManifest();
            NormalizeManifest(manifest);

            StyleSlotEntry entry = FindEntry(index);
            if (entry == null)
            {
                manifest.Slots.Add(new StyleSlotEntry
                {
                    Index = index,
                    Name = string.IsNullOrEmpty(name) ? "Slot " + index : name.Trim(),
                    HasData = HasSlot(index)
                });
            }
            else
            {
                entry.Index = index;
            }
        }

        private static StyleSlotEntry FindEntry(int index)
        {
            if (index < 1)
            {
                return null;
            }

            StyleSlotManifest manifest = GetManifest();
            for (int i = 0; i < manifest.Slots.Count; i++)
            {
                if (manifest.Slots[i].Index == index)
                {
                    return manifest.Slots[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Índices 1..N por posición en la lista (evita manifest corrupto).
        /// </summary>
        private static void NormalizeManifest(StyleSlotManifest manifest)
        {
            if (manifest == null || manifest.Slots == null)
            {
                return;
            }

            if (manifest.Slots.Count == 0)
            {
                manifest.Slots = StyleSlotManifest.CreateDefault(DefaultSlotCount).Slots;
            }

            for (int i = 0; i < manifest.Slots.Count; i++)
            {
                StyleSlotEntry entry = manifest.Slots[i];
                entry.Index = i + 1;
                entry.HasData = HasSlot(entry.Index);
            }
        }

        private static StyleSlotManifest LoadManifest()
        {
            ModStoragePaths.EnsureSlotsDirectory();
            string path = Path.Combine(ModStoragePaths.SlotsRoot, ManifestFileName);
            var manifest = StyleSlotManifest.CreateDefault(DefaultSlotCount);
            if (!File.Exists(path))
            {
                NormalizeManifest(manifest);
                return manifest;
            }

            try
            {
                string json = File.ReadAllText(path, Encoding.UTF8);
                List<StyleSlotEntry> parsed = ParseManifest(json);
                if (parsed.Count > 0)
                {
                    manifest.Slots = parsed;
                }

                NormalizeManifest(manifest);
            }
            catch (Exception ex)
            {
                ModLog.Warning("SFCC: manifest slots: " + ex.Message);
            }

            return manifest;
        }

        private static List<StyleSlotEntry> ParseManifest(string json)
        {
            var list = new List<StyleSlotEntry>();
            if (string.IsNullOrEmpty(json))
            {
                return list;
            }

            int slotsKey = json.IndexOf("\"slots\"", StringComparison.OrdinalIgnoreCase);
            int searchFrom = slotsKey >= 0 ? slotsKey : 0;
            int i = searchFrom;
            int slotNumber = 0;
            while (i < json.Length)
            {
                int idxKey = json.IndexOf("\"index\"", i, StringComparison.OrdinalIgnoreCase);
                if (idxKey < 0)
                {
                    break;
                }

                slotNumber++;
                int index = ReadIntAfter(json, idxKey);
                if (index < 1)
                {
                    index = slotNumber;
                }

                int nameKey = json.IndexOf("\"name\"", idxKey, StringComparison.OrdinalIgnoreCase);
                string name = ReadStringAfter(json, nameKey);
                list.Add(new StyleSlotEntry
                {
                    Index = index,
                    Name = string.IsNullOrEmpty(name) ? "Slot " + index : name,
                    HasData = HasSlot(index)
                });
                i = idxKey + 7;
            }

            return list;
        }

        private static void WriteManifest(StyleSlotManifest manifest)
        {
            NormalizeManifest(manifest);
            ModStoragePaths.EnsureSlotsDirectory();

            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"slots\": [");
            for (int i = 0; i < manifest.Slots.Count; i++)
            {
                StyleSlotEntry e = manifest.Slots[i];
                e.HasData = HasSlot(e.Index);
                sb.Append("    { \"index\": " + e.Index + ", \"name\": \"" + Escape(e.Name) + "\" }");
                sb.AppendLine(i < manifest.Slots.Count - 1 ? "," : "");
            }

            sb.AppendLine("  ]");
            sb.AppendLine("}");
            File.WriteAllText(Path.Combine(ModStoragePaths.SlotsRoot, ManifestFileName), sb.ToString(), Encoding.UTF8);
        }

        private static int ReadIntAfter(string json, int keyPos)
        {
            int colon = json.IndexOf(':', keyPos);
            if (colon < 0)
            {
                return 0;
            }

            int end = colon + 1;
            while (end < json.Length && (char.IsDigit(json[end]) || json[end] == '-'))
            {
                end++;
            }

            int value;
            int.TryParse(json.Substring(colon + 1, end - colon - 1).Trim(), out value);
            return value;
        }

        private static string ReadStringAfter(string json, int keyPos)
        {
            if (keyPos < 0)
            {
                return "";
            }

            int colon = json.IndexOf(':', keyPos);
            if (colon < 0)
            {
                return "";
            }

            int quoteStart = json.IndexOf('"', colon + 1);
            if (quoteStart < 0)
            {
                return "";
            }

            int quoteEnd = json.IndexOf('"', quoteStart + 1);
            if (quoteEnd < 0)
            {
                return "";
            }

            return Unescape(json.Substring(quoteStart + 1, quoteEnd - quoteStart - 1));
        }

        private static string Escape(string value)
        {
            return (value ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private static string Unescape(string value)
        {
            return (value ?? "").Replace("\\\"", "\"").Replace("\\\\", "\\");
        }
    }
}
