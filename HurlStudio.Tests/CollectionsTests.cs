using HurlStudio.Collections;
using HurlStudio.Collections.Model.Collection;
using HurlStudio.Collections.Settings;
using HurlStudio.Collections.Utility;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace HurlStudio.Tests
{
    [TestClass]
    public class CollectionsTests
    {
        private ISettingParser? _parser;
        private ICollectionSerializer? _serializer;

        [TestInitialize]
        public void Init()
        {
            _parser = new IniSettingParser();
            _serializer = new IniCollectionSerializer((IniSettingParser)_parser);
        }

        [TestMethod]
        public void TestValidCollection()
        {

            HurlCollection? collection = _serializer?.DeserializeFileAsync(
                Path.Combine("Assets", "Collections", "ValidCollection.hurlc"), Encoding.UTF8).Result;
            Assert.IsNotNull(collection);

            Assert.IsTrue(collection.Name.Equals("Valid collection"));
            Assert.IsTrue(collection.ExcludeRootDirectory);
            Assert.IsTrue(collection.AdditionalLocations.Count == 1);
            Assert.IsTrue(collection.AdditionalLocations.First() == "../HurlFiles/");
            Assert.IsTrue(collection.CollectionSettings.Count == 1);
            Assert.IsNotNull(collection.CollectionSettings.FirstOrDefault());

            // Proxy-Setting in collection settings
            // proxy=protocol:https,host:testproxy.local,port:8080
            Assert.IsInstanceOfType(collection.CollectionSettings.FirstOrDefault(), typeof(ProxySetting));
            Assert.IsTrue((collection.CollectionSettings.FirstOrDefault() as ProxySetting)?.Host == "testproxy.local");
            Assert.IsTrue((collection.CollectionSettings.FirstOrDefault() as ProxySetting)?.Protocol == Common.Enums.ProxyProtocol.HTTPS);
            Assert.IsTrue((collection.CollectionSettings.FirstOrDefault() as ProxySetting)?.Port == 8080);

            // Folder HurlSettings
            Assert.IsTrue(collection.FolderSettings.Count == 1);
            Assert.IsNotNull(collection.FolderSettings.FirstOrDefault());
            Assert.IsTrue(collection.FolderSettings.FirstOrDefault()?.Location == "../HurlFiles/");
            Assert.IsInstanceOfType(collection.FolderSettings.FirstOrDefault()?.FolderSettings[0], typeof(VariableSetting));
            VariableSetting? variableSetting1 = (VariableSetting?)collection.FolderSettings.FirstOrDefault()?.FolderSettings[0];
            VariableSetting? variableSetting2 = (VariableSetting?)collection.FolderSettings.FirstOrDefault()?.FolderSettings[1];

            Assert.AreEqual(variableSetting1?.Key, "fvar1");
            Assert.AreEqual(variableSetting1?.Value, "ftest2");
            Assert.AreEqual(variableSetting2?.Key, "fvar2");
            Assert.AreEqual(variableSetting2?.Value, "ftest3:abc");

            Assert.IsInstanceOfType(collection.FolderSettings.FirstOrDefault()?.FolderSettings[1], typeof(VariableSetting));

            // FileSettings 2: ../HurlFiles/TestFile2.hurl
            HurlFile? settings2 = collection.FileSettings.Where(x => x.FileLocation == "../HurlFiles/TestFile2.hurl").FirstOrDefault();
            Assert.IsNotNull(settings2);
            Assert.IsTrue(settings2.FileSettings.Count >= 4);
            Assert.IsInstanceOfType(settings2.FileSettings[3], typeof(AwsSigV4Setting));
            AwsSigV4Setting? awsSigV4Setting = settings2.FileSettings[3] as AwsSigV4Setting;
            Assert.IsNotNull(awsSigV4Setting);

            Assert.AreEqual(awsSigV4Setting.Provider1, "aws");
            Assert.AreEqual(awsSigV4Setting.Provider2, "");
            Assert.AreEqual(awsSigV4Setting.Region, "eu-central-1");
            Assert.AreEqual(awsSigV4Setting.Service, "foos");
        }

        /// <summary>
        /// Tests a list of valid setting strings for the AwsSigV4Setting type
        /// > Asserts, if they can be parsed 
        /// > no validity control
        /// </summary>
        [TestMethod]
        public void TestValidAwsV4Settings()
        {
            string[] testValues = {
                "aws_sig_v4=aws:amz:eu-central-1:foos",
                "aws_sig_v4=aws::eu-central-1:foos",
                "aws_sig_v4=aws:amz::foos",
                "aws_sig_v4=aws:amz::",
                "aws_sig_v4=aws:9::",
                "aws_sig_v4=aws:,::",
                "aws_sig_v4=aws:::foos",
                "aws_sig_v4=aws:::",
                "aws_sig_v4=:::",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(AwsSigV4Setting));
            }
        }

        /// <summary>
        /// Tests a list of valid setting strings for the AwsSigV4Setting type
        /// > Asserts, if they can be parsed 
        /// > no validity control
        /// </summary>
        [TestMethod]
        public void TestValidVariableSettings()
        {
            string[] testValues = {
                "variable=test1:test2",
                "variable=test1:test2:3",
                "variable=test2:"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(VariableSetting));
            }
        }

        /// <summary>
        /// Tests a list of valid setting strings for the AwsSigV4Setting type
        /// > Asserts, if they can be parsed 
        /// > no validity control
        /// </summary>
        [TestMethod]
        public void TestValidProxySettings()
        {
            string[] testValues = {
                "proxy=protocol:https,host:testproxy.local,port:8080,user:testuser,password:testpassword",
                "proxy=protocol:http,host:testproxy.local,port:8080,user:testuser,password:testpassword",
                "proxy=protocol:http,host:testproxy.local,port:8080,user:testuser,password:",
                "proxy=protocol:http,host:testproxy.local,port:8080,user:,password:",
                "proxy=protocol:http,host:testproxy.local,port:8123,user:,password:",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(ProxySetting));
            }
        }

        [TestMethod]
        public void TestValidCaCertSettings()
        {
            string[] testValues = {
                "ca_cert=E:\\Files\\test.pem",
                "ca_cert=E:/Files/test.pem"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(CaCertSetting));
            }
        }

        [TestMethod]
        public void TestValidClientCertificateSettings()
        {
            string[] testValues = {
                @"client_certificate=D:\Files\Certs\Test.crt|TestPassword",
                @"client_certificate=D:/Files/Certs/Test.crt|TestPassword",
                @"client_certificate=D:\Files\Certs\Test.crt|"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(ClientCertificateSetting));
            }
        }

        [TestMethod]
        public void TestValidConnectTimeoutSettings()
        {
            string[] testValues = {
                @"connect_timeout=30",
                @"connect_timeout=60",
                @"connect_timeout=0",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(ConnectTimeoutSetting));
            }
        }


        [TestMethod]
        public void TestValidConnectToSettings()
        {
            List<ConnectToSetting> settings = new List<ConnectToSetting>();
            string[] testValues = {
                @"connect_to=google.com:80:bing.com:8080"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(ConnectToSetting));
                
                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as ConnectToSetting);
            }

            Assert.AreEqual(1, settings.Count);
            Assert.AreEqual(settings[0].Host1, "google.com");
            Assert.AreEqual(settings[0].Host2, "bing.com");
            Assert.AreEqual(settings[0].Port1, (ushort)80);
            Assert.AreEqual(settings[0].Port2, (ushort)8080);
        }

        [TestMethod]
        public void TestValidContinueOnErrorSettings()
        {
            List<ContinueOnErrorSetting> settings = new List<ContinueOnErrorSetting>();
            string[] testValues = {
                @"continue_on_error=true",
                @"continue_on_error=false",
                @"continue_on_error=True",
                @"continue_on_error=False"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(ContinueOnErrorSetting));

                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as ContinueOnErrorSetting);
            }

            Assert.AreEqual(4, settings.Count);
            Assert.AreEqual(settings[0].ContinueOnError, true);
            Assert.AreEqual(settings[1].ContinueOnError, false);
            Assert.AreEqual(settings[2].ContinueOnError, true);
            Assert.AreEqual(settings[3].ContinueOnError, false);
        }

        [TestMethod]
        public void TestValidCookieSettings()
        {
            List<CookieSetting> settings = new List<CookieSetting>();
            string[] testValues = {
                @"cookies=",
                @"cookies=D:/Files/cookies.txt",
                @"cookies=D:/Files/cookies.txt;E:/Files/cookies.txt",
                @"cookies=;D:/Files/cookies.txt",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(CookieSetting));

                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as CookieSetting);
            }

            Assert.AreEqual(4, settings.Count);
            Assert.AreEqual(settings[0].CookieReadFile, "");
            Assert.AreEqual(settings[0].CookieWriteFile, "");
            Assert.AreEqual(settings[1].CookieReadFile, "D:/Files/cookies.txt");
            Assert.AreEqual(settings[1].CookieWriteFile, "");
            Assert.AreEqual(settings[2].CookieReadFile, "D:/Files/cookies.txt");
            Assert.AreEqual(settings[2].CookieWriteFile, "E:/Files/cookies.txt");
            Assert.AreEqual(settings[3].CookieReadFile, "");
            Assert.AreEqual(settings[3].CookieWriteFile, "D:/Files/cookies.txt");
        }
    }
}