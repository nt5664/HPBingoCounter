using HPBingoCounter.Core.Config;
using HPBingoCounter.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPBingoCounter.Core.Tests
{
    [TestFixture]
    public class ServiceTests
    {
        private HPBingoService _service;

        public static IEnumerable<TestCaseData> VersionTestSource
        {
            get
            {
                yield return new TestCaseData("invalidversionsconfig1.json", false, typeof(AggregateException));
                yield return new TestCaseData("invalidversionsconfig2.json", false, typeof(FileNotFoundException));
                yield return new TestCaseData("invalidversionsconfig3.json", false, null);
                yield return new TestCaseData("invalidversionsconfig4.json", false, null);
                yield return new TestCaseData("validversionsconfig1.json", true, null);
            }
        }

        [SetUp]
        public void Setup()
        {
            HPBingoConfigManager.SetConfigBaseDirectory($@"{Directory.GetCurrentDirectory()}\TestFiles\ServiceTests");
            _service = new HPBingoService();
        }

        [TearDown]
        public void TearDown()
        {
            _service.Dispose();
            HPBingoConfigManager.SetConfigBaseDirectory();
        }

        [TestCase("nullconfig.json")]
        [TestCase("emptyconfig.json")]
        [TestCase("nonexistentfile.json")]
        public void TestNullConfig(string fileName)
        {
            IBingoVersions? versions = null;

            Assert.Multiple(() =>
            {
                Assert.Throws<Exception>(() => HPBingoConfigManager.ReloadConfig(fileName));
                Assert.Throws<InvalidOperationException>(() => versions = _service.Versions);
                Assert.Throws<InvalidOperationException>(_service.ForceReloadVersions);
                Assert.That(versions, Is.Null);
                Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.RequestNewBoardAsync("1.0", HPBingoCardTypes.Normal, "0"));
            });
        }

        [TestCaseSource(nameof(VersionTestSource))]
        public void TestVersions(string fileName, bool shouldBeValid, Type? expectedException)
        {
            IBingoVersions? versions = null;

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => HPBingoConfigManager.ReloadConfig(fileName));
                if (expectedException != null)
                { 
                    Assert.Throws(expectedException, () => versions = _service.Versions);
                    Assert.Throws(expectedException, _service.ForceReloadVersions);
                }
                else
                { 
                    Assert.DoesNotThrow(() => versions = _service.Versions);
                    Assert.DoesNotThrow(_service.ForceReloadVersions);
                }

                Assert.That(versions?.IsValid, expectedException != null ? Is.Null : Is.EqualTo(shouldBeValid));
                if (shouldBeValid)
                {
                    Assert.That(string.IsNullOrWhiteSpace(versions?.DefaultVersion), Is.False);
                    Assert.That(versions?.VersionDictionary?.Any(), Is.True);
                    Assert.That(versions?.VersionDictionary?.Values.Contains(versions.DefaultVersion), Is.True);
                }
            });
        }

        [Test]
        public void TestVersionReloading()
        {
            IBingoVersions? versions1 = null;
            IBingoVersions? versions2 = null;

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => HPBingoConfigManager.ReloadConfig("validversionsconfig1.json"));
                Assert.DoesNotThrow(() => versions1 = _service.Versions);
                Assert.That(versions1?.IsValid, Is.True);

                Assert.DoesNotThrow(() => HPBingoConfigManager.ReloadConfig("validversionsconfig2.json"));
                Assert.DoesNotThrow(() =>
                {
                    _service.ForceReloadVersions();
                    versions2 = _service.Versions;
                });

                Assert.That(versions2?.IsValid, Is.True);
                Assert.That(versions1?.Equals(versions2), Is.False);
            });
        }

        [Test]
        public void TestBoardRequesting()
        {
            Assert.DoesNotThrow(() => HPBingoConfigManager.ReloadConfig("validversionsconfig3.json"));

            IBingoVersions? version = null;
            Assert.DoesNotThrow(() => version = _service.Versions ?? throw new Exception());

            string expVersion = version.DefaultVersion;
            HPBingoCardTypes expCardType = HPBingoCardTypes.Normal;
            string expSeed = "0";

            using IDisposable boardSubscription = _service.NewBoardObservable.Subscribe(board =>
            {
                Assert.That(board.Goals, Is.Not.Null);
                Assert.That(board.Seed, Is.EqualTo(expSeed));
                Assert.That(board.Version, Is.EqualTo(expVersion));
                Assert.That(board.CardType, Is.EqualTo(expCardType));
                Assert.Multiple(() =>
                {
                    foreach (var goal in board.Goals)
                    {
                        Assert.That(goal, Is.Not.Null);
                        Assert.That(string.IsNullOrWhiteSpace(goal.Name), Is.False);
                        Assert.That(goal.RequiredAmount, Is.GreaterThan(0));
                    }
                });
            });

            Assert.DoesNotThrowAsync(async () => await _service.RequestNewBoardAsync(expVersion, expCardType, expSeed));
        }
    }
}
