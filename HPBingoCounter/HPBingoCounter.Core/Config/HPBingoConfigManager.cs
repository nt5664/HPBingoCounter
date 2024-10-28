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

        private static string BaseDirectory = Directory.GetCurrentDirectory();

        public static IConfig? Current { get; private set; }

        public static string ApiPath => $@"{BaseDirectory}\{API_FILE}";

        private static string ConfigDirectory => $@"{BaseDirectory}\Config";

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

        public static void SetConfigBaseDirectory(string? directory = null)
        {
            BaseDirectory = directory ?? Directory.GetCurrentDirectory();
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

        public static string GetVersionsPath()
        {
            if (Current is null || string.IsNullOrEmpty(Current.VersionUrl))
                throw new InvalidOperationException("Invalid config, reload and try again");

            string versionUrl = Current.VersionUrl;
            return Current.UseLocalVersions ?
                $@"{BaseDirectory}\{versionUrl}" :
                versionUrl;
        }

        public static string GetGeneratorForVersion(string version)
        {
            if (Current is null || string.IsNullOrEmpty(Current.GeneratorFile))
                throw new InvalidOperationException("Invalid config, reload and try again");

            ArgumentNullException.ThrowIfNull(version);

            string generatorUrl = Current.GeneratorFile.Replace(URL_WILDCARD, version);
            return Current.UseLocalGenerator ? 
                $@"{BaseDirectory}\{generatorUrl}" :
                generatorUrl;
        }

        public static string GetGoalsUrlForVersion(string version)
        {
            if (Current is null || string.IsNullOrEmpty(Current.GoalsUrl))
                throw new InvalidOperationException("Invalid config, reload and try again");

            ArgumentNullException.ThrowIfNull(version);

            string goalsUrl = Current.GoalsUrl.Replace(URL_WILDCARD, version);
            return Current.UseLocalGoals ? 
                $@"{BaseDirectory}\{goalsUrl}" :
                goalsUrl;
        }
    }
}
