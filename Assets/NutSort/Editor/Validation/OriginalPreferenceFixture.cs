using System.IO;
using NutSort.Content;
using UnityEngine;

namespace NutSort.Validation
{
    // Validation-only backup survives Unity domain reload and interrupted runs.
    // Runtime continues to use the real PlayerPrefs implementation on all platforms.
    public static class OriginalPreferenceFixture
    {
        private static string Backup => Path.GetFullPath(Path.Combine(Application.dataPath,
            "../Library/ValidationBackups/GameplayUserLocalData.bin"));

        public static void Begin(string json = "{}")
        {
            Restore();
            Directory.CreateDirectory(Path.GetDirectoryName(Backup));
            using (var writer = new BinaryWriter(File.Create(Backup)))
            {
                writer.Write(PlayerPrefs.HasKey(OriginalUserStore.Key));
                writer.Write(PlayerPrefs.GetString(OriginalUserStore.Key, string.Empty));
            }
            PlayerPrefs.SetString(OriginalUserStore.Key, json);
            PlayerPrefs.Save();
        }

        public static void Restore()
        {
            if (!File.Exists(Backup)) return;
            bool existed; string json;
            using (var reader = new BinaryReader(File.OpenRead(Backup)))
            { existed = reader.ReadBoolean(); json = reader.ReadString(); }
            if (existed) PlayerPrefs.SetString(OriginalUserStore.Key, json);
            else PlayerPrefs.DeleteKey(OriginalUserStore.Key);
            PlayerPrefs.Save();
            File.Delete(Backup);
        }
    }
}
