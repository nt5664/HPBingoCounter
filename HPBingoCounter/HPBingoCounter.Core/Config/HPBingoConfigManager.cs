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

        public static IConfig? ReloadConfig()
        {
            try
            {
                string configString = File.ReadAllText($@"{Directory.GetCurrentDirectory()}\{CONFIG_FILE}");
                Current = JsonConvert.DeserializeObject<HPBingoConfig>(configString)
                    ?? throw new ApplicationException("Cannot load config data");
            }
            catch (Exception ex)
            {
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
