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
        static HPBingoConfigManager()
        {
            ReloadConfig();
        }

        public static IConfig? Current { get; private set; }

        public static IConfig? ReloadConfig()
        {
            string configString = File.ReadAllText($@"{Assembly.GetExecutingAssembly().Location}\{HPBingoConstants.CONFIG_FILE}");
            Current = JsonConvert.DeserializeObject<HPBingoConfig>(configString)
                ?? throw new ApplicationException("Cannot load config data");

            return Current;
        }
    }
}
