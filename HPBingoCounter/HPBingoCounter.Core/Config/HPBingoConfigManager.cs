using Newtonsoft.Json;

namespace HPBingoCounter.Core.Config
{
    public static class HPBingoConfigManager
    {
        private const string CONFIG_FILE = @"Config\appconfig.json";
        private const string API_FILE = @"Generator\api.js";
        private const string URL_WILDCARD = "???";

        static HPBingoConfigManager()
        {
            ReloadConfig();
        }

        public static IConfig? Current { get; private set; }

        public static string ApiPath => $@"{Directory.GetCurrentDirectory()}\{API_FILE}";

        public static IConfig? ReloadConfig(string? configFilePath = null)
        {
            try
            {
                string configString = File.ReadAllText(configFilePath ?? $@"{Directory.GetCurrentDirectory()}\{CONFIG_FILE}");
                HPBingoConfig config = JsonConvert.DeserializeObject<HPBingoConfig>(configString)
                    ?? throw new ApplicationException("Cannot load config data");

                config.FilePath = configFilePath ?? CONFIG_FILE;
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
