using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPBingoCounter.Core.Config
{
    public interface IBingoVersions
    {
        string DefaultVersion { get; }
        IDictionary<string, string> VersionDictionary { get; }
        bool IsValid { get; }
    }
}
