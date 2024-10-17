using HPBingoCounter.Core.Config;
using HPBingoCounter.Core.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPBingoCounter.Core
{
    public class HPBingoService
    {
        private IBingoVersions? _versions;
        public IBingoVersions? Versions
        {
            get
            {
                if (_versions is null)
                {
                    if (HPBingoConfigManager.Current is null || HPBingoConfigManager.Current.VersionUrl is null)
                        throw new InvalidOperationException("Invalid config; cannot resolve versions. Reload the config and try again");

                    string versions;
                    using (var hc = new HttpClient())
                    {
                        var req = hc.GetStringAsync(HPBingoConfigManager.Current.VersionUrl);
                        req.Wait();
                        versions = req.Result;
                    }

                    if (string.IsNullOrEmpty(versions))
                        throw new ArgumentException("Cannot obtain versions or the file is invalid");

                    _versions = JsonConvert.DeserializeObject<HPBingoVersions>(versions);
                }

                return _versions;
            }
        }

        public void RequestNewBoard(string version, HPBingoCardTypes cardType, string seed)
        {

        }
    }
}
