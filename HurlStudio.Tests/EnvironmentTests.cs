using HurlStudio.Collections;
using HurlStudio.Collections.Model;
using HurlStudio.Collections.Settings;
using HurlStudio.Collections.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Runtime.ConstrainedExecution;
using System.Text;
using static System.Net.WebRequestMethods;

namespace HurlStudio.Tests
{
    [TestClass]
    public class EnvironmentTests
    {
        private ISettingParser? _parser;
        private IEnvironmentSerializer? _serializer;

        [TestInitialize]
        public void Init()
        {
            ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole().AddFilter("*", LogLevel.Trace));

            _parser = new IniSettingParser(factory.CreateLogger<IniSettingParser>());
            _serializer = new IniEnvironmentSerializer((IniSettingParser)_parser, factory.CreateLogger<IniEnvironmentSerializer>());
        }

        [TestMethod]
        public void TestValidCollection()
        {

            HurlEnvironment? environment = _serializer?.DeserializeFileAsync(
                Path.Combine("Assets", "Environments", "default.hurle"), Encoding.UTF8).Result;
            Assert.IsNotNull(environment);
            Assert.AreEqual(environment.Name, "Default");

        }
    }
}