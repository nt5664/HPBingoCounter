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
                        throw new InvalidOperationException("Cannot obtain versions or the file is invalid");

                    _versions = JsonConvert.DeserializeObject<HPBingoVersions>(versions);
                }

                return _versions;
            }
        }

        public void Dispose()
        {
            _newBoardSubject.Dispose();
        }

        public async void RequestNewBoard(string version, HPBingoCardTypes cardType, string seed)
        {
            if (HPBingoConfigManager.Current is null)
                throw new InvalidOperationException("Invalid config; cannot resolve data. Reload the config and try again");

            string generatorFile = HPBingoConfigManager.GetGeneratorForVersion(version);
            if (!File.Exists(generatorFile))
                throw new InvalidOperationException($"Unsopported version; cannot find generator for {version}. Update the generator or reload the config");

            string api;
            string goalList;
            string generator;

            try
            {
                using (var hc = new HttpClient())
                {
                    goalList = await hc.GetStringAsync(HPBingoConfigManager.GetGoalsUrlForVersion(version));
                }

                api = File.ReadAllText(HPBingoConfigManager.ApiPath);
                generator = File.ReadAllText(generatorFile);
            }
            catch (Exception e)
            {
                throw new FileNotFoundException("The generator files cannot be opened or don't exist. Check the config", e);
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
                using (var engine = new V8ScriptEngine())
                {
                    engine.Execute(goalList);
                    engine.Execute(docInfo, generator);
                    engine.Execute(api);
                    rawGoals = engine.Script.getCards(version, cardType.ToString().ToLower(), seed);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidProgramException("The JS generator is corrupted, check the config", ex);
            }

            if (string.IsNullOrEmpty(rawGoals))
                throw new Exception("Board generation failed, couldn't obtain the cards");

            IEnumerable<HPBingoGoal>? goals = JsonConvert.DeserializeObject<IEnumerable<HPBingoGoal>>(rawGoals);
            HPBingoBoardDto board = new HPBingoBoardDto(version, seed, cardType, goals);
            _newBoardSubject.OnNext(board);
        }
    }
}
