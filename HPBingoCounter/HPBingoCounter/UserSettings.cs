using System.IO;
using Newtonsoft.Json;

namespace HPBingoCounter
{
    internal class UserSettings
    {
        private static string SAVE_PATH { get; } = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\HP Bingo Counter";
        private static string SAVE_FILE => $@"{SAVE_PATH}\usersettings.json";

        private UserSettings()
        {
        }

        private static UserSettings? _current = null;
        public static UserSettings Current 
        { 
            get
            {
                return _current ??= new UserSettings();
            }
        }

        public static bool TryLoad()
        {
            if (!File.Exists(SAVE_FILE)) 
                return false;

            var rawData = File.ReadAllText(SAVE_FILE);
            _current = JsonConvert.DeserializeObject<UserSettings>(rawData);
            return true;
        }

        public static void Save()
        {
            var rawData = JsonConvert.SerializeObject(Current);
            if (!Directory.Exists(SAVE_PATH))
                Directory.CreateDirectory(SAVE_PATH);

            File.WriteAllText(SAVE_FILE, rawData);
        }

        [JsonProperty]
        public double WindowWidth { get; set; } = 678;

        [JsonProperty]
        public double WindowHeight { get; set; } = 810;
    }
}
