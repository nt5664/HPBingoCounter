using System.Reactive.Subjects;
using HPBingoCounter.Core.Config;
using HPBingoCounter.Core.Models;
using HPBingoCounter.Core.Types;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using Newtonsoft.Json;

namespace HPBingoCounter.Core
{
    public class HPBingoService : IBingoService
    {
        public HPBingoService()
        {
            _newBoardSubject = new Subject<HPBingoBoardDto>();
        }

        private readonly Subject<HPBingoBoardDto> _newBoardSubject;
        public IObservable<HPBingoBoardDto> NewBoardObservable => _newBoardSubject;

        private IBingoVersions? _versions;
        public IBingoVersions? Versions
        {
            get
            {
                if (_versions is null)
                    ForceReloadVersions();

                return _versions;
            }
        }

        public void ForceReloadVersions()
        {
            if (HPBingoConfigManager.Current is null || HPBingoConfigManager.Current.VersionUrl is null)
                throw new InvalidOperationException("Invalid config; cannot resolve versions. Reload the config and try again");

            string versions;
            if (!HPBingoConfigManager.Current.UseLocalVersions)
            {
                using var hc = new HttpClient();

                var req = hc.GetStringAsync(HPBingoConfigManager.GetVersionsPath());
                req.Wait();
                versions = req.Result;
            }
            else
            {
                versions = File.ReadAllText(HPBingoConfigManager.GetVersionsPath());
            }

            if (string.IsNullOrEmpty(versions))
                throw new InvalidOperationException("Cannot obtain versions or the file is invalid");

            _versions = JsonConvert.DeserializeObject<HPBingoVersions>(versions);
        }

        public void Dispose()
        {
            _newBoardSubject.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task RequestNewBoardAsync(string version, HPBingoCardTypes cardType, string seed)
        {
            if (HPBingoConfigManager.Current is null)
                throw new InvalidOperationException("Invalid config; cannot resolve data. Reload the config and try again");

            string api;
            string goalList;
            string generator;

            try
            {
                using var hc = new HttpClient();

                string generatorPath = HPBingoConfigManager.GetGeneratorForVersion(version);
                generator = HPBingoConfigManager.Current.UseLocalGenerator ? 
                    File.ReadAllText(generatorPath) :
                    await hc.GetStringAsync(generatorPath);

                string goalsPath = HPBingoConfigManager.GetGoalsUrlForVersion(version);
                goalList = HPBingoConfigManager.Current.UseLocalGoals ?
                    File.ReadAllText(goalsPath) :
                    await hc.GetStringAsync(goalsPath);

                api = File.ReadAllText(HPBingoConfigManager.ApiPath);
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException("The generator files cannot be opened or don't exist. Check the config", ex);
            }

            if (string.IsNullOrEmpty(api) || string.IsNullOrEmpty(goalList) || string.IsNullOrEmpty(generator))
                throw new InvalidProgramException("The JS functions are invalid. Check the URLs in the config");

            var docInfo = new DocumentInfo
            {
                Category = ModuleCategory.CommonJS
            };

            string? rawGoals;
            try
            {
                using var engine = new V8ScriptEngine();

                engine.Execute(goalList);
                engine.Execute(docInfo, generator);
                engine.Execute(api);
                rawGoals = engine.Script.getCards(version, cardType.ToString().ToLower(), seed);
            }
            catch (Exception ex)
            {
                throw new InvalidProgramException("The JS generator is corrupted, check the config", ex);
            }

            if (string.IsNullOrEmpty(rawGoals))
                throw new Exception("Board generation failed, couldn't obtain the goals");

            IEnumerable<HPBingoGoal>? goals = JsonConvert.DeserializeObject<IEnumerable<HPBingoGoal>>(rawGoals);
            HPBingoBoardDto board = new HPBingoBoardDto(version, seed, cardType, goals);
            _newBoardSubject.OnNext(board);
        }
    }
}
