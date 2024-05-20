using HurlStudio.Collections;
using HurlStudio.Collections.Model.Collection;
using HurlStudio.Collections.Model.Environment;
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
            ILogger<IniSettingParser> logger = LoggerFactory.Create(builder => builder.AddConsole().AddFilter("*", LogLevel.Trace))
                                                            .CreateLogger<IniSettingParser>();

            _parser = new IniSettingParser(logger);
            _serializer = new IniEnvironmentSerializer((IniSettingParser)_parser);
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