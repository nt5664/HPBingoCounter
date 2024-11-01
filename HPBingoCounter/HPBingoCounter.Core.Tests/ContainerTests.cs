using HPBingoCounter.Core.Config;
using HPBingoCounter.Core.Models;
using HPBingoCounter.Core.Types;
using Newtonsoft.Json;

namespace HPBingoCounter.Core.Tests
{
    [TestFixture]
    public class ContainerTests
    {
        public static IEnumerable<TestCaseData> ConfigTestData
        {
            get
            {
                string directoryPath = $@"{Directory.GetCurrentDirectory()}\TestFiles\ConfigTests";
                yield return new TestCaseData(File.ReadAllText($@"{directoryPath}\configtest1.json"), true, new[] { true, true, true }, new bool?[] { true, false, false }, null);
                yield return new TestCaseData(File.ReadAllText($@"{directoryPath}\configtest2.json"), true, new[] { false, false, false }, new bool?[] { false, false, false }, null);
                yield return new TestCaseData(File.ReadAllText($@"{directoryPath}\configtest3.json"), false, new[] { false, false, false }, new bool?[] { null, null, null }, typeof(JsonReaderException));
                yield return new TestCaseData(File.ReadAllText($@"{directoryPath}\configtest4.json"), false, new[] { false, false, false }, new bool?[] { null, null, null }, typeof(JsonSerializationException));
                yield return new TestCaseData(File.ReadAllText($@"{directoryPath}\configtest5.json"), true, new[] { false, true, true }, new bool?[] { false, true, true }, null);
                yield return new TestCaseData(File.ReadAllText($@"{directoryPath}\configtest6.json"), true, new[] { true, false, true }, new bool?[] { false, true, false }, null);
                yield return new TestCaseData(File.ReadAllText($@"{directoryPath}\configtest7.json"), true, new[] { true, true, false }, new bool?[] { true, true, true }, null);
                yield return new TestCaseData(File.ReadAllText($@"{directoryPath}\configtest8.json"), true, new[] { false, true, false }, new bool?[] { false, false, false }, null);
                yield return new TestCaseData(File.ReadAllText($@"{directoryPath}\configtest9.json"), true, new[] { false, false, false }, new bool?[] { true, false, false }, null);
                yield return new TestCaseData(File.ReadAllText($@"{directoryPath}\configtest10.json"), true, new[] { true, true, true }, new bool?[] { false, false, true }, null);
                yield return new TestCaseData(File.ReadAllText($@"{directoryPath}\configtest11.json"), false, new[] {false, false, false}, new bool?[] { null, null, null }, null);
            }
        }

        public static IEnumerable<TestCaseData> BoardDtoTestData
        {
            get
            {
                yield return new TestCaseData("TEST1.0", "123456", HPBingoCardTypes.Normal, new[] { new HPBingoGoal("t1", "Test", 3, 1, null), new HPBingoGoal("t2", "Test2", 6, 6, null) });
                yield return new TestCaseData("TEST7.2", "000000", HPBingoCardTypes.Short, null);
                yield return new TestCaseData("TEST160", "999999", HPBingoCardTypes.Blackout, new[] { null, new HPBingoGoal("t5", "Test5", 0, 0, null), new HPBingoGoal(string.Empty, string.Empty, 1, -1, null) });
                yield return new TestCaseData("TEST160", "999999", HPBingoCardTypes.Blackout, Array.Empty<HPBingoGoal>());
            }
        }

        public static IEnumerable<TestCaseData> VersionTestData
        {
            get
            {
                string directoryPath = $@"{Directory.GetCurrentDirectory()}\TestFiles\VersionTests";
                yield return new TestCaseData(File.ReadAllText($@"{directoryPath}\versiontest1.json"), false, null, null, null);
                yield return new TestCaseData(File.ReadAllText($@"{directoryPath}\versiontest2.json"), true, (bool?)true, "v1.0", new Dictionary<string, string?> { { "v1.0", "1.0" }, { "v1.2", "1.2" } });
                yield return new TestCaseData(File.ReadAllText($@"{directoryPath}\versiontest3.json"), true, (bool?)false, "v1.9", null);
                yield return new TestCaseData(File.ReadAllText($@"{directoryPath}\versiontest4.json"), true, (bool?)false, " ", new Dictionary<string, string?> { { "v5.3", "test" }, { "v0.5", null } });
                yield return new TestCaseData(File.ReadAllText($@"{directoryPath}\versiontest5.json"), true, (bool?)true, "v5.3", new Dictionary<string, string?> { { "v5.3", "test" }, { "v2.1", null } });
            }
        }

        [TestCaseSource(nameof(ConfigTestData))]
        public void ConfigTest(string json, bool shouldParse, bool[] dataValid, bool?[] flagValues, Type? expectedException)
        {
            IConfig? config = null;

            Assert.Multiple(() =>
            {
                if (shouldParse || expectedException is null)
                    Assert.DoesNotThrow(Deserialize);
                else
                    Assert.Throws(expectedException, Deserialize);

                Assert.That(config != null, Is.EqualTo(shouldParse));
                Assert.That(string.IsNullOrWhiteSpace(config?.VersionUrl), Is.Not.EqualTo(dataValid[0]));
                Assert.That(string.IsNullOrWhiteSpace(config?.GeneratorFile), Is.Not.EqualTo(dataValid[1]));
                Assert.That(string.IsNullOrWhiteSpace(config?.GoalsUrl), Is.Not.EqualTo(dataValid[2]));
                Assert.That(config?.UseLocalVersions, Is.EqualTo(flagValues[0]));
                Assert.That(config?.UseLocalGoals, Is.EqualTo(flagValues[1]));
                Assert.That(config?.UseLocalGenerator, Is.EqualTo(flagValues[2]));
            });

            void Deserialize()
            {
                config = JsonConvert.DeserializeObject<HPBingoConfig>(json);
            }
        }

        [TestCase("{ \"name\": \"test\", \"amount\": 5 }", true, new object?[] { "test", 5 })]
        [TestCase("{ \"name\": \"other test\", \"amount\": \"7\" }", true, new object?[] { "other test", 7 })]
        [TestCase("{ \"name\": \"third test\" }", true, new object?[] { "third test", 0 })]
        [TestCase("{ \"name\": null, \"amount\": 12 }", true, new object?[] { null, 12 })]
        [TestCase("null", false, new object?[] { null, null })]
        public void GoalTest(string json, bool shouldParse, object?[] expValues)
        {
            HPBingoGoal? goal = null;

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => goal = JsonConvert.DeserializeObject<HPBingoGoal>(json));

                Assert.That(goal != null, Is.EqualTo(shouldParse));
                Assert.That(goal?.Name, Is.EqualTo(expValues[0]));
                Assert.That(goal?.RequiredAmount, Is.EqualTo(expValues[1]));
            });
        }

        [TestCaseSource(nameof(BoardDtoTestData))]
        public void BoardDtoTest(string version, string seed, HPBingoCardTypes cardType, HPBingoGoal[]? goals)
        {
            HPBingoBoardDto dto = new HPBingoBoardDto(version, seed, cardType, goals?.AsEnumerable());

            Assert.Multiple(() =>
            {
                Assert.That(dto.Version, Is.EqualTo(version));
                Assert.That(dto.Seed, Is.EqualTo(seed));
                Assert.That(dto.CardType, Is.EqualTo(cardType));
                Assert.That(dto.Goals, goals != null ? Is.EquivalentTo(goals) : Is.Null);
            });
        }

        [TestCaseSource(nameof(VersionTestData))]
        public void VersionTest(string json, bool shouldParse, bool? shouldBeValid, string? defaultVersion, Dictionary<string, string?>? versions)
        {
            IBingoVersions? version = null;

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => version = JsonConvert.DeserializeObject<HPBingoVersions>(json));
                Assert.That(version != null, Is.EqualTo(shouldParse));
                Assert.That(version?.IsValid, Is.EqualTo(shouldBeValid));
                Assert.That(version?.DefaultVersion, Is.EqualTo(defaultVersion));
                Assert.That(version?.VersionDictionary, versions != null ? Is.EquivalentTo(versions) : Is.Null);
            });
        }
    }
}