using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Mirador
{
    public class Settings
    {
        private static readonly string AppName = "Mirador";
        private static readonly string DefaultFileName = "settings.json";
        private static readonly string DefaultFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            AppName,
            DefaultFileName
        );

        private static Settings _current;

        public bool IsDesktopSHIconsToggled { get; set; } = false;
        public bool IsTaskbarToggled { get; set; } = false;
        public bool IsCursorToggled { get; set; } = false;
        public bool AutoHide { get; set; } = false;
        public bool DoubleClickToHide { get; set; } = false;
        public bool CursorUnhide { get; set; } = false;
        public int CursorUnhideRegion { get; set; } = 0;
        public List<uint> ShortcutKeys { get; set; } = new List<uint>();
        public bool IsShortcutToggled { get; set; } = false;
        public int HideDelay { get; set; } = 500;

        public static Settings Load(string filePath = null)
        {
            filePath ??= DefaultFilePath;

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Settings file not found. Creating default settings.");
                _current = new Settings();
                _current.Save(filePath);
            }
            else
            {
                try
                {
                    var json = File.ReadAllText(filePath);
                    _current = JsonSerializer.Deserialize<Settings>(json);
                    Console.WriteLine("Settings loaded successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load settings: {ex.Message}");
                    _current = new Settings();
                }
            }

            return _current;
        }

        public void Save(string filePath = null)
        {
            filePath ??= DefaultFilePath;

            try
            {
                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
                Console.WriteLine("Settings saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }

        public static Settings Current
        {
            get
            {
                if (_current == null)
                {
                    Load();
                }
                return _current;
            }
        }
    }
}
