using Newtonsoft.Json;

namespace HPBingoCounter.Core.Config
{
    public static class HPBingoConfigManager
    {
        private const string DEFAULT_CONFIG_FILE = @"appconfig.json";
        private const string API_FILE = @"Generator\api.js";
        private const string URL_WILDCARD = "???";

        static HPBingoConfigManager()
        {
            ReloadConfig();
        }

        public static IConfig? Current { get; private set; }

        public static string ApiPath => $@"{Directory.GetCurrentDirectory()}\{API_FILE}";

        private static string ConfigDirectory => $@"{Directory.GetCurrentDirectory()}\Config";

        public static IEnumerable<string> AvailableConfigFiles
        {
            get
            {
                var options = new EnumerationOptions
                {
                    RecurseSubdirectories = false,
                    MatchCasing = MatchCasing.CaseInsensitive,
                    MatchType = MatchType.Simple
                };

                return Directory.GetFiles(ConfigDirectory, "*.json", options)
                    .Select(x => x.Split('\\').Last());
            }
        }

        public static IConfig? ReloadConfig(string? configFileName = null)
        {
            try
            {
                string configPath = $@"{ConfigDirectory}\{configFileName ?? DEFAULT_CONFIG_FILE}";
                string configString = File.ReadAllText(configPath);
                HPBingoConfig config = JsonConvert.DeserializeObject<HPBingoConfig>(configString)
                    ?? throw new ApplicationException("Cannot load config data");

                config.FileName = configFileName ?? DEFAULT_CONFIG_FILE;
                Current = config;
            }
            catch (Exception ex)
            {
                Current = null;
                throw new Exception("Something went wrong while loading the config. Check the config file and reload", ex);
            }

            return Current;
        }

        public static string GetGeneratorForVersion(string version)
        {
            if (Current is null || string.IsNullOrEmpty(Current.GeneratorFile))
                throw new InvalidOperationException("Invalid config, reload and try again");

            ArgumentNullException.ThrowIfNull(version);

            return $@"{Directory.GetCurrentDirectory()}\{Current.GeneratorFile.Replace(URL_WILDCARD, version)}";
        }

        public static string GetGoalsUrlForVersion(string version)
        {
            if (Current is null || string.IsNullOrEmpty(Current.GoalsUrl))
                throw new InvalidOperationException("Invalid config, reload and try again");

            ArgumentNullException.ThrowIfNull(version);

            return Current.GoalsUrl.Replace(URL_WILDCARD, version);
        }
    }
}
