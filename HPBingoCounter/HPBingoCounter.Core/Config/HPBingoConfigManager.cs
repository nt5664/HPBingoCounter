using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HPBingoCounter.Core.Config
{
    public static class HPBingoConfigManager
    {
        private const string URL_WILDCARD = "???";

        static HPBingoConfigManager()
        {
            ReloadConfig();
        }

        public static IConfig? Current { get; private set; }

        public static IConfig? ReloadConfig()
        {
            string configString = File.ReadAllText($@"{Directory.GetCurrentDirectory()}\{HPBingoConstants.CONFIG_FILE}");
            Current = JsonConvert.DeserializeObject<HPBingoConfig>(configString)
                ?? throw new ApplicationException("Cannot load config data");

            return Current;
        }

        public static string GetGeneratorUrlForVersion(string version)
        {
            if (Current is null || string.IsNullOrEmpty(Current.GeneratorUrl))
                throw new InvalidOperationException("Invalid config, reload and try again");

            ArgumentNullException.ThrowIfNull(version);

            return Current.GeneratorUrl.Replace(URL_WILDCARD, version);
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
