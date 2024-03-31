using HurlStudio.Collections.Settings;
using HurlStudio.Collections.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Tests
{
    [TestClass]
    public class SettingsTests
    {
        private ISettingParser? _parser;
        private ICollectionSerializer? _serializer;

        [TestInitialize]
        public void Init()
        {
            _parser = new IniSettingParser();
            _serializer = new IniCollectionSerializer((IniSettingParser)_parser);
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
                @"client_certificate=D:\Files\Certs\Test.crt|TestPassword|D:\Files\Keys\TestKey.key",
                @"client_certificate=D:/Files/Certs/Test.crt|TestPassword|",
                @"client_certificate=D:\Files\Certs\Test.crt|TestPassword",
                @"client_certificate=D:/Files/Certs/Test.crt|TestPassword",
                @"client_certificate=D:\Files\Certs\Test.crt|",
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

        [TestMethod]
        public void TestValidDelaySettings()
        {
            List<DelaySetting> settings = new List<DelaySetting>();
            string[] testValues = {
                @"delay=1000",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(DelaySetting));

                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as DelaySetting);
            }

            Assert.AreEqual(1, settings.Count);
            Assert.AreEqual(settings[0].Delay, (uint)1000);
        }


        [TestMethod]
        public void TestValidHttpVersionSettings()
        {
            List<HttpVersionSetting> settings = new List<HttpVersionSetting>();
            string[] testValues = {
                @"http_version=Http1_0",
                @"http_version=Http1_1",
                @"http_version=Http2",
                @"http_version=Http3",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(HttpVersionSetting));

                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as HttpVersionSetting);
            }

            Assert.AreEqual(4, settings.Count);
            Assert.AreEqual(settings[0].Version, Collections.Enums.HttpVersion.Http1_0);
            Assert.AreEqual(settings[1].Version, Collections.Enums.HttpVersion.Http1_1);
            Assert.AreEqual(settings[2].Version, Collections.Enums.HttpVersion.Http2);
            Assert.AreEqual(settings[3].Version, Collections.Enums.HttpVersion.Http3);
        }

        [TestMethod]
        public void TestValidIgnoreAssertsSettings()
        {
            List<IgnoreAssertsSetting> settings = new List<IgnoreAssertsSetting>();
            string[] testValues = {
                @"ignore_asserts=true",
                @"ignore_asserts=false",
                @"ignore_asserts=True",
                @"ignore_asserts=False"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(IgnoreAssertsSetting));

                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as IgnoreAssertsSetting);
            }

            Assert.AreEqual(4, settings.Count);
            Assert.AreEqual(settings[0].IgnoreAsserts, true);
            Assert.AreEqual(settings[1].IgnoreAsserts, false);
            Assert.AreEqual(settings[2].IgnoreAsserts, true);
            Assert.AreEqual(settings[3].IgnoreAsserts, false);
        }
    }
}
