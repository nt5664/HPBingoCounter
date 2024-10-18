using HPBingoCounter.Core.Config;
using HPBingoCounter.Core.Models;
using HPBingoCounter.Core.Types;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace HPBingoCounter.Core
{
    public class HPBingoService : IDisposable
    {
        public HPBingoService()
        {
            _newBoardSubject = new Subject<IEnumerable<HPBingoGoal>?>();
        }

        private readonly Subject<IEnumerable<HPBingoGoal>?> _newBoardSubject;
        public IObservable<IEnumerable<HPBingoGoal>?> NewBoardObservable => _newBoardSubject;

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

            string api;
            string goalList;
            string generator;

            using (var hc = new HttpClient())
            {
                api = await hc.GetStringAsync(HPBingoConfigManager.Current.GeneratorFuncUrl);
                goalList = await hc.GetStringAsync(HPBingoConfigManager.GetGoalsUrlForVersion(version));
                generator = await hc.GetStringAsync(HPBingoConfigManager.GetGeneratorUrlForVersion(version));
            }

            if (string.IsNullOrEmpty(api) || string.IsNullOrEmpty(goalList) || string.IsNullOrEmpty(generator))
                throw new InvalidProgramException("The JS functions are invalid. Check the URLs in the config");

            //generator = generator.Replace("return bingoBoard;", "return JSON.stringify(bingoBoard.map(x => { return { name: x.name, diff: x.difficulty } }));");
            api = api.Replace(".map(x => { id = x.id, name = x.name })", ".map(x => { return { name: x.name }})"); // TODO: add this change to goals.js
            var docInfo = new DocumentInfo
            {
                Category = ModuleCategory.CommonJS
            };

            string? rawGoals;
            using (var engine = new V8ScriptEngine())
            {
                engine.Execute(goalList);
                engine.Execute(docInfo, generator);
                engine.Execute(api);
                rawGoals = engine.Script.getCards(version, cardType.ToString().ToLower(), seed);
            }

            if (string.IsNullOrEmpty(rawGoals))
                throw new Exception("Board generation failed, couldn't obtain the cards");

            IEnumerable<HPBingoGoal>? goals = JsonConvert.DeserializeObject<IEnumerable<HPBingoGoal>>(rawGoals);
            _newBoardSubject.OnNext(goals);
        }
    }
}
