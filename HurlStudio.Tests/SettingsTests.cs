using HurlStudio.Collections.Settings;
using HurlStudio.Collections.Utility;
using HurlStudio.Common.Extensions;
using Microsoft.Extensions.Logging;

namespace HurlStudio.Tests
{
    [TestClass]
    public class SettingsTests
    {
        private ISettingParser? _parser;

        [TestInitialize]
        public void Init()
        {
            ILogger<IniSettingParser> logger = LoggerFactory.Create(builder => builder.AddConsole().AddFilter("*", LogLevel.Trace))
                                                            .CreateLogger<IniSettingParser>();
            _parser = new IniSettingParser(logger);
        }

        [TestMethod]
        public void TestValidAllowInsecureSettings()
        {
            List<AllowInsecureSetting> settings = new List<AllowInsecureSetting>();
            string[] testValues =
            {
                @"allow_insecure=true",
                @"allow_insecure=True",
                @"allow_insecure=false",
                @"allow_insecure=False",
                @"#allow_insecure=true",
                @"#allow_insecure=True",
                @"#allow_insecure=false",
                @"#allow_insecure=False",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(AllowInsecureSetting));

                settings.Add(hurlSetting as AllowInsecureSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].AllowInsecure, true);
            Assert.AreEqual(settings[1].AllowInsecure, true);
            Assert.AreEqual(settings[2].AllowInsecure, false);
            Assert.AreEqual(settings[3].AllowInsecure, false);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[2].IsEnabled, true);
            Assert.AreEqual(settings[3].IsEnabled, true);
            Assert.AreEqual(settings[4].AllowInsecure, true);
            Assert.AreEqual(settings[5].AllowInsecure, true);
            Assert.AreEqual(settings[6].AllowInsecure, false);
            Assert.AreEqual(settings[7].AllowInsecure, false);
            Assert.AreEqual(settings[4].IsEnabled, false);
            Assert.AreEqual(settings[5].IsEnabled, false);
            Assert.AreEqual(settings[6].IsEnabled, false);
            Assert.AreEqual(settings[7].IsEnabled, false);
        }

        /// <summary>
        /// Tests a list of valid setting strings for the AwsSigV4Setting type
        /// > Asserts, if they can be parsed 
        /// > no validity control
        /// </summary>
        [TestMethod]
        public void TestValidAwsV4Settings()
        {
            List<AwsSigV4Setting> settings = new List<AwsSigV4Setting>();

            string[] testValues =
            {
                "aws_sig_v4=aws,amz,eu-central-1,foos",
                "aws_sig_v4=aws,,eu-central-1,foos",
                "aws_sig_v4=aws,amz,,foos",
                "aws_sig_v4=aws,amz,,",
                "aws_sig_v4=aws,9,,",
                "aws_sig_v4=aws,,,foos",
                "aws_sig_v4=aws,,,",
                "aws_sig_v4=,,,",
                "#aws_sig_v4=aws,amz,eu-central-1,foos",
                "#aws_sig_v4=aws,,eu-central-1,foos",
                "#aws_sig_v4=aws,amz,,foos",
                "#aws_sig_v4=aws,amz,,",
                "#aws_sig_v4=aws,9,,",
                "#aws_sig_v4=aws,,,foos",
                "#aws_sig_v4=aws,,,",
                "#aws_sig_v4=,,,",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(AwsSigV4Setting));

                settings.Add(hurlSetting as AwsSigV4Setting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(settings.Count, testValues.Length);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].Provider1, "aws");
            Assert.AreEqual(settings[0].Provider2, "amz");
            Assert.AreEqual(settings[0].Region, "eu-central-1");
            Assert.AreEqual(settings[0].Service, "foos");
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].Provider1, "aws");
            Assert.AreEqual(settings[1].Provider2, "");
            Assert.AreEqual(settings[1].Region, "eu-central-1");
            Assert.AreEqual(settings[1].Service, "foos");
            Assert.AreEqual(settings[2].IsEnabled, true);
            Assert.AreEqual(settings[2].Provider1, "aws");
            Assert.AreEqual(settings[2].Provider2, "amz");
            Assert.AreEqual(settings[2].Region, "");
            Assert.AreEqual(settings[2].Service, "foos");
            Assert.AreEqual(settings[3].IsEnabled, true);
            Assert.AreEqual(settings[3].Provider1, "aws");
            Assert.AreEqual(settings[3].Provider2, "amz");
            Assert.AreEqual(settings[3].Region, "");
            Assert.AreEqual(settings[3].Service, "");
            Assert.AreEqual(settings[4].IsEnabled, true);
            Assert.AreEqual(settings[4].Provider1, "aws");
            Assert.AreEqual(settings[4].Provider2, "9");
            Assert.AreEqual(settings[4].Region, "");
            Assert.AreEqual(settings[4].Service, "");
            Assert.AreEqual(settings[5].IsEnabled, true);
            Assert.AreEqual(settings[5].Provider1, "aws");
            Assert.AreEqual(settings[5].Provider2, "");
            Assert.AreEqual(settings[5].Region, "");
            Assert.AreEqual(settings[5].Service, "foos");
            Assert.AreEqual(settings[6].IsEnabled, true);
            Assert.AreEqual(settings[6].Provider1, "aws");
            Assert.AreEqual(settings[6].Provider2, "");
            Assert.AreEqual(settings[6].Region, "");
            Assert.AreEqual(settings[6].Service, "");
            Assert.AreEqual(settings[7].IsEnabled, true);
            Assert.AreEqual(settings[7].Provider1, "");
            Assert.AreEqual(settings[7].Provider2, "");
            Assert.AreEqual(settings[7].Region, "");
            Assert.AreEqual(settings[7].Service, "");
            Assert.AreEqual(settings[8].IsEnabled, false);
            Assert.AreEqual(settings[8].Provider1, "aws");
            Assert.AreEqual(settings[8].Provider2, "amz");
            Assert.AreEqual(settings[8].Region, "eu-central-1");
            Assert.AreEqual(settings[8].Service, "foos");
            Assert.AreEqual(settings[9].IsEnabled, false);
            Assert.AreEqual(settings[9].Provider1, "aws");
            Assert.AreEqual(settings[9].Provider2, "");
            Assert.AreEqual(settings[9].Region, "eu-central-1");
            Assert.AreEqual(settings[9].Service, "foos");
            Assert.AreEqual(settings[10].IsEnabled, false);
            Assert.AreEqual(settings[10].Provider1, "aws");
            Assert.AreEqual(settings[10].Provider2, "amz");
            Assert.AreEqual(settings[10].Region, "");
            Assert.AreEqual(settings[10].Service, "foos");
            Assert.AreEqual(settings[11].IsEnabled, false);
            Assert.AreEqual(settings[11].Provider1, "aws");
            Assert.AreEqual(settings[11].Provider2, "amz");
            Assert.AreEqual(settings[11].Region, "");
            Assert.AreEqual(settings[11].Service, "");
            Assert.AreEqual(settings[12].IsEnabled, false);
            Assert.AreEqual(settings[12].Provider1, "aws");
            Assert.AreEqual(settings[12].Provider2, "9");
            Assert.AreEqual(settings[12].Region, "");
            Assert.AreEqual(settings[12].Service, "");
            Assert.AreEqual(settings[13].IsEnabled, false);
            Assert.AreEqual(settings[13].Provider1, "aws");
            Assert.AreEqual(settings[13].Provider2, "");
            Assert.AreEqual(settings[13].Region, "");
            Assert.AreEqual(settings[13].Service, "foos");
            Assert.AreEqual(settings[14].IsEnabled, false);
            Assert.AreEqual(settings[14].Provider1, "aws");
            Assert.AreEqual(settings[14].Provider2, "");
            Assert.AreEqual(settings[14].Region, "");
            Assert.AreEqual(settings[14].Service, "");
            Assert.AreEqual(settings[15].IsEnabled, false);
            Assert.AreEqual(settings[15].Provider1, "");
            Assert.AreEqual(settings[15].Provider2, "");
            Assert.AreEqual(settings[15].Region, "");
            Assert.AreEqual(settings[15].Service, "");
        }

        [TestMethod]
        public void TestValidBasicUserSettings()
        {
            List<BasicUserSetting> settings = new List<BasicUserSetting>();
            string[] testValues =
            {
                "user=user1,cGFzc3dvcmQx", // password1
                "user=user2,",
                "user=user3,",
                "#user=user1,cGFzc3dvcmQx", // password1
                "#user=user2,",
                "#user=user3,",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(BasicUserSetting));

                settings.Add(hurlSetting as BasicUserSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(settings.Count, testValues.Length);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].User, "user1");
            Assert.AreEqual(settings[0].Password, "password1");
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].User, "user2");
            Assert.AreEqual(settings[1].Password, null);
            Assert.AreEqual(settings[2].IsEnabled, true);
            Assert.AreEqual(settings[2].User, "user3");
            Assert.AreEqual(settings[2].Password, null);
            Assert.AreEqual(settings[3].IsEnabled, false);
            Assert.AreEqual(settings[3].User, "user1");
            Assert.AreEqual(settings[3].Password, "password1");
            Assert.AreEqual(settings[4].IsEnabled, false);
            Assert.AreEqual(settings[4].User, "user2");
            Assert.AreEqual(settings[4].Password, null);
            Assert.AreEqual(settings[5].IsEnabled, false);
            Assert.AreEqual(settings[5].User, "user3");
            Assert.AreEqual(settings[5].Password, null);
        }


        [TestMethod]
        public void TestValidCaCertSettings()
        {
            List<CaCertSetting> settings = new List<CaCertSetting>();
            string[] testValues =
            {
                "ca_cert=E:\\Files\\test.pem",
                "ca_cert=E:/Files/test.pem",
                "#ca_cert=E:\\Files\\test.pem",
                "#ca_cert=E:/Files/test.pem"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(CaCertSetting));
                settings.Add(hurlSetting as CaCertSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(settings.Count, testValues.Length);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].File, "E:\\Files\\test.pem");
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].File, "E:/Files/test.pem");
            Assert.AreEqual(settings[2].IsEnabled, false);
            Assert.AreEqual(settings[2].File, "E:\\Files\\test.pem");
            Assert.AreEqual(settings[3].IsEnabled, false);
            Assert.AreEqual(settings[3].File, "E:/Files/test.pem");
        }

        [TestMethod]
        public void TestValidClientCertificateSettings()
        {
            List<ClientCertificateSetting> settings = new List<ClientCertificateSetting>();
            string[] testValues =
            {
                @"client_certificate=D:\Files\Certs\Test.crt,VGVzdFBhc3N3b3Jk,D:\Files\Keys\TestKey.key",
                @"client_certificate=D:/Files/Certs/Test.crt,VGVzdFBhc3N3b3Jk,",
                @"client_certificate=D:\Files\Certs\Test.crt,VGVzdFBhc3N3b3Jk",
                @"client_certificate=D:/Files/Certs/Test.crt,VGVzdFBhc3N3b3Jk",
                @"client_certificate=D:\Files\Certs\Test.crt,",
                @"#client_certificate=D:\Files\Certs\Test.crt,VGVzdFBhc3N3b3Jk,D:\Files\Keys\TestKey.key",
                @"#client_certificate=D:/Files/Certs/Test.crt,VGVzdFBhc3N3b3Jk,",
                @"#client_certificate=D:\Files\Certs\Test.crt,VGVzdFBhc3N3b3Jk",
                @"#client_certificate=D:/Files/Certs/Test.crt,VGVzdFBhc3N3b3Jk",
                @"#client_certificate=D:\Files\Certs\Test.crt,",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(ClientCertificateSetting));
                settings.Add(hurlSetting as ClientCertificateSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(settings.Count, testValues.Length);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].CertificateFile, "D:\\Files\\Certs\\Test.crt");
            Assert.AreEqual(settings[0].Password, "TestPassword");
            Assert.AreEqual(settings[0].KeyFile, "D:\\Files\\Keys\\TestKey.key");
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].CertificateFile, "D:/Files/Certs/Test.crt");
            Assert.AreEqual(settings[1].Password, "TestPassword");
            Assert.AreEqual(settings[1].KeyFile, "");
            Assert.AreEqual(settings[2].IsEnabled, true);
            Assert.AreEqual(settings[2].CertificateFile, "D:\\Files\\Certs\\Test.crt");
            Assert.AreEqual(settings[2].Password, "TestPassword");
            Assert.AreEqual(settings[2].KeyFile, null);
            Assert.AreEqual(settings[3].IsEnabled, true);
            Assert.AreEqual(settings[3].CertificateFile, "D:/Files/Certs/Test.crt");
            Assert.AreEqual(settings[3].Password, "TestPassword");
            Assert.AreEqual(settings[3].KeyFile, null);
            Assert.AreEqual(settings[4].IsEnabled, true);
            Assert.AreEqual(settings[4].CertificateFile, "D:\\Files\\Certs\\Test.crt");
            Assert.AreEqual(settings[4].Password, "");
            Assert.AreEqual(settings[4].KeyFile, null);
            Assert.AreEqual(settings[5].IsEnabled, false);
            Assert.AreEqual(settings[5].CertificateFile, "D:\\Files\\Certs\\Test.crt");
            Assert.AreEqual(settings[5].Password, "TestPassword");
            Assert.AreEqual(settings[5].KeyFile, "D:\\Files\\Keys\\TestKey.key");
            Assert.AreEqual(settings[6].IsEnabled, false);
            Assert.AreEqual(settings[6].CertificateFile, "D:/Files/Certs/Test.crt");
            Assert.AreEqual(settings[6].Password, "TestPassword");
            Assert.AreEqual(settings[6].KeyFile, "");
            Assert.AreEqual(settings[7].IsEnabled, false);
            Assert.AreEqual(settings[7].CertificateFile, "D:\\Files\\Certs\\Test.crt");
            Assert.AreEqual(settings[7].Password, "TestPassword");
            Assert.AreEqual(settings[7].KeyFile, null);
            Assert.AreEqual(settings[8].IsEnabled, false);
            Assert.AreEqual(settings[8].CertificateFile, "D:/Files/Certs/Test.crt");
            Assert.AreEqual(settings[8].Password, "TestPassword");
            Assert.AreEqual(settings[8].KeyFile, null);
            Assert.AreEqual(settings[9].IsEnabled, false);
            Assert.AreEqual(settings[9].CertificateFile, "D:\\Files\\Certs\\Test.crt");
            Assert.AreEqual(settings[9].Password, "");
            Assert.AreEqual(settings[9].KeyFile, null);
        }

        [TestMethod]
        public void TestValidConnectToSettings()
        {
            List<ConnectToSetting> settings = new List<ConnectToSetting>();
            string[] testValues =
            {
                @"connect_to=google.com,80,bing.com,8080",
                @"#connect_to=google.com,80,bing.com,8080"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(ConnectToSetting));

                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as ConnectToSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].Host1, "google.com");
            Assert.AreEqual(settings[0].Host2, "bing.com");
            Assert.AreEqual(settings[0].Port1, (ushort)80);
            Assert.AreEqual(settings[0].Port2, (ushort)8080);
            Assert.AreEqual(settings[1].IsEnabled, false);
            Assert.AreEqual(settings[1].Host1, "google.com");
            Assert.AreEqual(settings[1].Host2, "bing.com");
            Assert.AreEqual(settings[1].Port1, (ushort)80);
            Assert.AreEqual(settings[1].Port2, (ushort)8080);
        }

        [TestMethod]
        public void TestValidContinueOnErrorSettings()
        {
            List<ContinueOnErrorSetting> settings = new List<ContinueOnErrorSetting>();
            string[] testValues =
            {
                @"continue_on_error=true",
                @"continue_on_error=false",
                @"continue_on_error=True",
                @"continue_on_error=False",
                @"#continue_on_error=true",
                @"#continue_on_error=false",
                @"#continue_on_error=True",
                @"#continue_on_error=False"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(ContinueOnErrorSetting));

                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as ContinueOnErrorSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].ContinueOnError, true);
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].ContinueOnError, false);
            Assert.AreEqual(settings[2].IsEnabled, true);
            Assert.AreEqual(settings[2].ContinueOnError, true);
            Assert.AreEqual(settings[3].IsEnabled, true);
            Assert.AreEqual(settings[3].ContinueOnError, false);
            Assert.AreEqual(settings[4].IsEnabled, false);
            Assert.AreEqual(settings[4].ContinueOnError, true);
            Assert.AreEqual(settings[5].IsEnabled, false);
            Assert.AreEqual(settings[5].ContinueOnError, false);
            Assert.AreEqual(settings[6].IsEnabled, false);
            Assert.AreEqual(settings[6].ContinueOnError, true);
            Assert.AreEqual(settings[7].IsEnabled, false);
            Assert.AreEqual(settings[7].ContinueOnError, false);
        }

        [TestMethod]
        public void TestValidCookieSettings()
        {
            List<CookieSetting> settings = new List<CookieSetting>();
            string[] testValues =
            {
                @"cookies=",
                @"cookies=D:/Files/cookies.txt",
                @"cookies=D:/Files/cookies.txt,E:/Files/cookies.txt",
                @"cookies=,D:/Files/cookies.txt",
                @"#cookies=",
                @"#cookies=D:/Files/cookies.txt",
                @"#cookies=D:/Files/cookies.txt,E:/Files/cookies.txt",
                @"#cookies=,D:/Files/cookies.txt",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(CookieSetting));

                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as CookieSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].CookieReadFile, "");
            Assert.AreEqual(settings[0].CookieWriteFile, "");
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].CookieReadFile, "D:/Files/cookies.txt");
            Assert.AreEqual(settings[1].CookieWriteFile, "");
            Assert.AreEqual(settings[2].IsEnabled, true);
            Assert.AreEqual(settings[2].CookieReadFile, "D:/Files/cookies.txt");
            Assert.AreEqual(settings[2].CookieWriteFile, "E:/Files/cookies.txt");
            Assert.AreEqual(settings[3].IsEnabled, true);
            Assert.AreEqual(settings[3].CookieReadFile, "");
            Assert.AreEqual(settings[3].CookieWriteFile, "D:/Files/cookies.txt");
            Assert.AreEqual(settings[4].IsEnabled, false);
            Assert.AreEqual(settings[4].CookieReadFile, "");
            Assert.AreEqual(settings[4].CookieWriteFile, "");
            Assert.AreEqual(settings[5].IsEnabled, false);
            Assert.AreEqual(settings[5].CookieReadFile, "D:/Files/cookies.txt");
            Assert.AreEqual(settings[5].CookieWriteFile, "");
            Assert.AreEqual(settings[6].IsEnabled, false);
            Assert.AreEqual(settings[6].CookieReadFile, "D:/Files/cookies.txt");
            Assert.AreEqual(settings[6].CookieWriteFile, "E:/Files/cookies.txt");
            Assert.AreEqual(settings[7].IsEnabled, false);
            Assert.AreEqual(settings[7].CookieReadFile, "");
            Assert.AreEqual(settings[7].CookieWriteFile, "D:/Files/cookies.txt");
        }

        [TestMethod]
        public void TestValidDelaySettings()
        {
            List<DelaySetting> settings = new List<DelaySetting>();
            string[] testValues =
            {
                @"delay=1000",
                @"#delay=1000",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(DelaySetting));

                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as DelaySetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].Delay, 1000u);
            Assert.AreEqual(settings[1].IsEnabled, false);
            Assert.AreEqual(settings[1].Delay, 1000u);
        }

        [TestMethod]
        public void TestValidFileRootSettings()
        {
            List<FileRootSetting> settings = new List<FileRootSetting>();
            string[] testValues =
            {
                @"file_root=D:\Files\HurlStudio\test\",
                @"#file_root=D:\Files\HurlStudio\test\",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(FileRootSetting));

                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as FileRootSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].Directory, @"D:\Files\HurlStudio\test\");
            Assert.AreEqual(settings[1].IsEnabled, false);
            Assert.AreEqual(settings[1].Directory, @"D:\Files\HurlStudio\test\");
        }


        [TestMethod]
        public void TestValidHttpVersionSettings()
        {
            List<HttpVersionSetting> settings = new List<HttpVersionSetting>();
            string[] testValues =
            {
                @"http_version=Http1_0",
                @"http_version=Http1_1",
                @"http_version=Http2",
                @"http_version=Http3", 
                @"#http_version=Http1_0",
                @"#http_version=Http1_1",
                @"#http_version=Http2",
                @"#http_version=Http3",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(HttpVersionSetting));

                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as HttpVersionSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].Version, Common.Enums.HttpVersion.Http1_0);
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].Version, Common.Enums.HttpVersion.Http1_1);
            Assert.AreEqual(settings[2].IsEnabled, true);
            Assert.AreEqual(settings[2].Version, Common.Enums.HttpVersion.Http2);
            Assert.AreEqual(settings[3].IsEnabled, true);
            Assert.AreEqual(settings[3].Version, Common.Enums.HttpVersion.Http3);
            Assert.AreEqual(settings[4].IsEnabled, false);
            Assert.AreEqual(settings[4].Version, Common.Enums.HttpVersion.Http1_0);
            Assert.AreEqual(settings[5].IsEnabled, false);
            Assert.AreEqual(settings[5].Version, Common.Enums.HttpVersion.Http1_1);
            Assert.AreEqual(settings[6].IsEnabled, false);
            Assert.AreEqual(settings[6].Version, Common.Enums.HttpVersion.Http2);
            Assert.AreEqual(settings[7].IsEnabled, false);
            Assert.AreEqual(settings[7].Version, Common.Enums.HttpVersion.Http3);
        }

        [TestMethod]
        public void TestValidIgnoreAssertsSettings()
        {
            List<IgnoreAssertsSetting> settings = new List<IgnoreAssertsSetting>();
            string[] testValues =
            {
                @"ignore_asserts=true",
                @"ignore_asserts=false",
                @"ignore_asserts=True",
                @"ignore_asserts=False",
                @"#ignore_asserts=true",
                @"#ignore_asserts=false",
                @"#ignore_asserts=True",
                @"#ignore_asserts=False"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(IgnoreAssertsSetting));

                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as IgnoreAssertsSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].IgnoreAsserts, true);
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].IgnoreAsserts, false);
            Assert.AreEqual(settings[2].IsEnabled, true);
            Assert.AreEqual(settings[2].IgnoreAsserts, true);
            Assert.AreEqual(settings[3].IsEnabled, true);
            Assert.AreEqual(settings[3].IgnoreAsserts, false);
            Assert.AreEqual(settings[4].IsEnabled, false);
            Assert.AreEqual(settings[4].IgnoreAsserts, true);
            Assert.AreEqual(settings[5].IsEnabled, false);
            Assert.AreEqual(settings[5].IgnoreAsserts, false);
            Assert.AreEqual(settings[6].IsEnabled, false);
            Assert.AreEqual(settings[6].IgnoreAsserts, true);
            Assert.AreEqual(settings[7].IsEnabled, false);
            Assert.AreEqual(settings[7].IgnoreAsserts, false);
        }


        [TestMethod]
        public void TestValidIpVersionSettings()
        {
            List<IpVersionSetting> settings = new List<IpVersionSetting>();
            string[] testValues =
            {
                @"ip_version=IPv4",
                @"ip_version=IPv6",
                @"ip_version=IPV4",
                @"ip_version=IPV6",
                @"#ip_version=IPv4",
                @"#ip_version=IPv6",
                @"#ip_version=IPV4",
                @"#ip_version=IPV6",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(IpVersionSetting));

                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as IpVersionSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].IpVersion, Common.Enums.IpVersion.IPv4);
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].IpVersion, Common.Enums.IpVersion.IPv6);
            Assert.AreEqual(settings[2].IsEnabled, true);
            Assert.AreEqual(settings[2].IpVersion, Common.Enums.IpVersion.IPv4);
            Assert.AreEqual(settings[3].IsEnabled, true);
            Assert.AreEqual(settings[3].IpVersion, Common.Enums.IpVersion.IPv6);
            Assert.AreEqual(settings[4].IsEnabled, false);
            Assert.AreEqual(settings[4].IpVersion, Common.Enums.IpVersion.IPv4);
            Assert.AreEqual(settings[5].IsEnabled, false);
            Assert.AreEqual(settings[5].IpVersion, Common.Enums.IpVersion.IPv6);
            Assert.AreEqual(settings[6].IsEnabled, false);
            Assert.AreEqual(settings[6].IpVersion, Common.Enums.IpVersion.IPv4);
            Assert.AreEqual(settings[7].IsEnabled, false);
            Assert.AreEqual(settings[7].IpVersion, Common.Enums.IpVersion.IPv6);
        }


        [TestMethod]
        public void TestValidMaxFilesizeSettings()
        {
            List<MaxFilesizeSetting> settings = new List<MaxFilesizeSetting>();
            string[] testValues =
            {
                @"max_filesize=1024",
                @"max_filesize=4096",
                @"#max_filesize=1024",
                @"#max_filesize=4096"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(MaxFilesizeSetting));

                settings.Add(hurlSetting as MaxFilesizeSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].MaxFilesize, 1024u);
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].MaxFilesize, 4096u);
            Assert.AreEqual(settings[2].IsEnabled, false);
            Assert.AreEqual(settings[2].MaxFilesize, 1024u);
            Assert.AreEqual(settings[3].IsEnabled, false);
            Assert.AreEqual(settings[3].MaxFilesize, 4096u);
        }

        [TestMethod]
        public void TestValidNetrcSettings()
        {
            List<NetrcSetting> settings = new List<NetrcSetting>();
            string[] testValues =
            {
                @"netrc=true,true,",
                @"netrc=true,false,/home/test/.netrc",
                @"netrc=false,false,/home/test/.netrc",
                @"#netrc=true,true,",
                @"#netrc=true,false,/home/test/.netrc",
                @"#netrc=false,false,/home/test/.netrc",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(NetrcSetting));

                settings.Add(hurlSetting as NetrcSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].IsOptional, true);
            Assert.AreEqual(settings[0].IsAutomatic, true);
            Assert.AreEqual(settings[0].File, "");
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].IsOptional, true);
            Assert.AreEqual(settings[1].IsAutomatic, false);
            Assert.AreEqual(settings[1].File, "/home/test/.netrc");
            Assert.AreEqual(settings[2].IsEnabled, true);
            Assert.AreEqual(settings[2].IsOptional, false);
            Assert.AreEqual(settings[2].IsAutomatic, false);
            Assert.AreEqual(settings[2].File, "/home/test/.netrc");
            Assert.AreEqual(settings[3].IsEnabled, false);
            Assert.AreEqual(settings[3].IsOptional, true);
            Assert.AreEqual(settings[3].IsAutomatic, true);
            Assert.AreEqual(settings[3].File, "");
            Assert.AreEqual(settings[4].IsEnabled, false);
            Assert.AreEqual(settings[4].IsOptional, true);
            Assert.AreEqual(settings[4].IsAutomatic, false);
            Assert.AreEqual(settings[4].File, "/home/test/.netrc");
            Assert.AreEqual(settings[5].IsEnabled, false);
            Assert.AreEqual(settings[5].IsOptional, false);
            Assert.AreEqual(settings[5].IsAutomatic, false);
            Assert.AreEqual(settings[5].File, "/home/test/.netrc");
        }

        [TestMethod]
        public void TestValidNoProxySettings()
        {
            List<NoProxySetting> settings = new List<NoProxySetting>();
            string[] testValues =
            {
                @"no_proxy=google.com,cloudflare.com,amazon.com",
                @"no_proxy=google.com",
                @"#no_proxy=google.com,cloudflare.com,amazon.com",
                @"#no_proxy=google.com"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(NoProxySetting));

                settings.Add(hurlSetting as NoProxySetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].NoProxyHosts.Count, 3);
            Assert.AreEqual(settings[0].NoProxyHosts[0].Host, "google.com");
            Assert.AreEqual(settings[0].NoProxyHosts[1].Host, "cloudflare.com");
            Assert.AreEqual(settings[0].NoProxyHosts[2].Host, "amazon.com");
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].NoProxyHosts.Count, 1);
            Assert.AreEqual(settings[1].NoProxyHosts[0].Host, "google.com");
            Assert.AreEqual(settings[2].IsEnabled, false);
            Assert.AreEqual(settings[2].NoProxyHosts.Count, 3);
            Assert.AreEqual(settings[2].NoProxyHosts[0].Host, "google.com");
            Assert.AreEqual(settings[2].NoProxyHosts[1].Host, "cloudflare.com");
            Assert.AreEqual(settings[2].NoProxyHosts[2].Host, "amazon.com");
            Assert.AreEqual(settings[3].IsEnabled, false);
            Assert.AreEqual(settings[3].NoProxyHosts.Count, 1);
            Assert.AreEqual(settings[3].NoProxyHosts[0].Host, "google.com");
        }

        [TestMethod]
        public void TestValidPathAsIsSettings()
        {
            List<PathAsIsSetting> settings = new List<PathAsIsSetting>();
            string[] testValues =
            {
                @"path_as_is=true",
                @"path_as_is=True",
                @"path_as_is=false",
                @"path_as_is=False",
                @"#path_as_is=true",
                @"#path_as_is=True",
                @"#path_as_is=false",
                @"#path_as_is=False",
            };    

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(PathAsIsSetting));

                settings.Add(hurlSetting as PathAsIsSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].PathAsIs, true);
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].PathAsIs, true);
            Assert.AreEqual(settings[2].IsEnabled, true);
            Assert.AreEqual(settings[2].PathAsIs, false);
            Assert.AreEqual(settings[3].IsEnabled, true);
            Assert.AreEqual(settings[3].PathAsIs, false);
            Assert.AreEqual(settings[4].IsEnabled, false);
            Assert.AreEqual(settings[4].PathAsIs, true);
            Assert.AreEqual(settings[5].IsEnabled, false);
            Assert.AreEqual(settings[5].PathAsIs, true);
            Assert.AreEqual(settings[6].IsEnabled, false);
            Assert.AreEqual(settings[6].PathAsIs, false);
            Assert.AreEqual(settings[7].IsEnabled, false);
            Assert.AreEqual(settings[7].PathAsIs, false);
        }

        [TestMethod]
        public void TestValidProxySettings()
        {
            List<ProxySetting> settings = new List<ProxySetting>();
            string[] testValues =
            {
                "proxy=https,testproxy.local,8080,testuser,dGVzdHBhc3N3b3Jk",
                "proxy=http,testproxy.local,8080,testuser,dGVzdHBhc3N3b3Jk",
                "proxy=http,testproxy.local,8080,testuser",
                "proxy=http,testproxy.local,8080,,",
                "proxy=http,testproxy.local,8123,,",
                "#proxy=https,testproxy.local,8080,testuser,dGVzdHBhc3N3b3Jk",
                "#proxy=http,testproxy.local,8080,testuser,dGVzdHBhc3N3b3Jk",
                "#proxy=http,testproxy.local,8080,testuser",
                "#proxy=http,testproxy.local,8080,,",
                "#proxy=http,testproxy.local,8123,,",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(ProxySetting));

                settings.Add(hurlSetting as ProxySetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].Protocol, Common.Enums.ProxyProtocol.HTTPS);
            Assert.AreEqual(settings[0].Host, "testproxy.local");
            Assert.AreEqual(settings[0].Port, (ushort)8080);
            Assert.AreEqual(settings[0].User, "testuser");
            Assert.AreEqual(settings[0].Password, "testpassword");
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].Protocol, Common.Enums.ProxyProtocol.HTTP);
            Assert.AreEqual(settings[1].Host, "testproxy.local");
            Assert.AreEqual(settings[1].Port, (ushort)8080);
            Assert.AreEqual(settings[1].User, "testuser");
            Assert.AreEqual(settings[1].Password, "testpassword");
            Assert.AreEqual(settings[2].IsEnabled, true);
            Assert.AreEqual(settings[2].Protocol, Common.Enums.ProxyProtocol.HTTP);
            Assert.AreEqual(settings[2].Host, "testproxy.local");
            Assert.AreEqual(settings[2].Port, (ushort)8080);
            Assert.AreEqual(settings[2].User, "testuser");
            Assert.AreEqual(settings[2].Password, "");
            Assert.AreEqual(settings[3].IsEnabled, true);
            Assert.AreEqual(settings[3].Protocol, Common.Enums.ProxyProtocol.HTTP);
            Assert.AreEqual(settings[3].Host, "testproxy.local");
            Assert.AreEqual(settings[3].Port, (ushort)8080);
            Assert.AreEqual(settings[3].User, "");
            Assert.AreEqual(settings[3].Password, "");
            Assert.AreEqual(settings[4].IsEnabled, true);
            Assert.AreEqual(settings[4].Protocol, Common.Enums.ProxyProtocol.HTTP);
            Assert.AreEqual(settings[4].Host, "testproxy.local");
            Assert.AreEqual(settings[4].Port, (ushort)8123);
            Assert.AreEqual(settings[4].User, "");
            Assert.AreEqual(settings[4].Password, "");
            Assert.AreEqual(settings[5].IsEnabled, false);
            Assert.AreEqual(settings[5].Protocol, Common.Enums.ProxyProtocol.HTTPS);
            Assert.AreEqual(settings[5].Host, "testproxy.local");
            Assert.AreEqual(settings[5].Port, (ushort)8080);
            Assert.AreEqual(settings[5].User, "testuser");
            Assert.AreEqual(settings[5].Password, "testpassword");
            Assert.AreEqual(settings[6].IsEnabled, false);
            Assert.AreEqual(settings[6].Protocol, Common.Enums.ProxyProtocol.HTTP);
            Assert.AreEqual(settings[6].Host, "testproxy.local");
            Assert.AreEqual(settings[6].Port, (ushort)8080);
            Assert.AreEqual(settings[6].User, "testuser");
            Assert.AreEqual(settings[6].Password, "testpassword");
            Assert.AreEqual(settings[7].IsEnabled, false);
            Assert.AreEqual(settings[7].Protocol, Common.Enums.ProxyProtocol.HTTP);
            Assert.AreEqual(settings[7].Host, "testproxy.local");
            Assert.AreEqual(settings[7].Port, (ushort)8080);
            Assert.AreEqual(settings[7].User, "testuser");
            Assert.AreEqual(settings[7].Password, "");
            Assert.AreEqual(settings[8].IsEnabled, false);
            Assert.AreEqual(settings[8].Protocol, Common.Enums.ProxyProtocol.HTTP);
            Assert.AreEqual(settings[8].Host, "testproxy.local");
            Assert.AreEqual(settings[8].Port, (ushort)8080);
            Assert.AreEqual(settings[8].User, "");
            Assert.AreEqual(settings[8].Password, "");
            Assert.AreEqual(settings[9].IsEnabled, false);
            Assert.AreEqual(settings[9].Protocol, Common.Enums.ProxyProtocol.HTTP);
            Assert.AreEqual(settings[9].Host, "testproxy.local");
            Assert.AreEqual(settings[9].Port, (ushort)8123);
            Assert.AreEqual(settings[9].User, "");
            Assert.AreEqual(settings[9].Password, "");
        }

        [TestMethod]
        public void TestValidRedirectionsSettings()
        {
            List<RedirectionsSetting> settings = new List<RedirectionsSetting>();
            string[] testValues =
            {
                @"redirections=true",
                @"redirections=True",
                @"redirections=false",
                @"redirections=False",
                @"redirections=true,true",
                @"redirections=true,false",
                @"redirections=true,true,50",
                @"#redirections=true",
                @"#redirections=True",
                @"#redirections=false",
                @"#redirections=False",
                @"#redirections=true,true",
                @"#redirections=true,false",
                @"#redirections=true,true,50"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(RedirectionsSetting));

                settings.Add(hurlSetting as RedirectionsSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].AllowRedirections, true);
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].AllowRedirections, true);
            Assert.AreEqual(settings[2].IsEnabled, true);
            Assert.AreEqual(settings[2].AllowRedirections, false);
            Assert.AreEqual(settings[3].IsEnabled, true);
            Assert.AreEqual(settings[3].AllowRedirections, false);
            Assert.AreEqual(settings[4].IsEnabled, true);
            Assert.AreEqual(settings[4].AllowRedirections, true);
            Assert.AreEqual(settings[4].RedirectionsTrusted, true);
            Assert.AreEqual(settings[5].IsEnabled, true);
            Assert.AreEqual(settings[5].AllowRedirections, true);
            Assert.AreEqual(settings[5].RedirectionsTrusted, false);
            Assert.AreEqual(settings[6].IsEnabled, true);
            Assert.AreEqual(settings[6].AllowRedirections, true);
            Assert.AreEqual(settings[6].RedirectionsTrusted, true);
            Assert.AreEqual(settings[6].MaxRedirections, 50u);
            Assert.AreEqual(settings[7].IsEnabled, false);
            Assert.AreEqual(settings[7].AllowRedirections, true);
            Assert.AreEqual(settings[8].IsEnabled, false);
            Assert.AreEqual(settings[8].AllowRedirections, true);
            Assert.AreEqual(settings[9].IsEnabled, false);
            Assert.AreEqual(settings[9].AllowRedirections, false);
            Assert.AreEqual(settings[10].IsEnabled, false);
            Assert.AreEqual(settings[10].AllowRedirections, false);
            Assert.AreEqual(settings[11].IsEnabled, false);
            Assert.AreEqual(settings[11].AllowRedirections, true);
            Assert.AreEqual(settings[11].RedirectionsTrusted, true);
            Assert.AreEqual(settings[12].IsEnabled, false);
            Assert.AreEqual(settings[12].AllowRedirections, true);
            Assert.AreEqual(settings[12].RedirectionsTrusted, false);
            Assert.AreEqual(settings[13].IsEnabled, false);
            Assert.AreEqual(settings[13].AllowRedirections, true);
            Assert.AreEqual(settings[13].RedirectionsTrusted, true);
            Assert.AreEqual(settings[13].MaxRedirections, 50u);
        }

        [TestMethod]
        public void TestValidResolveSetting()
        {
            List<ResolveSetting> settings = new List<ResolveSetting>();
            string[] testValues =
            {
                @"resolve=www.example.com,80,127.0.0.1",
                @"#resolve=www.example.com,80,127.0.0.1"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(ResolveSetting));

                settings.Add(hurlSetting as ResolveSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].Host, "www.example.com");
            Assert.AreEqual(settings[0].Port, (ushort)80);
            Assert.AreEqual(settings[0].Address, "127.0.0.1");
            Assert.AreEqual(settings[1].IsEnabled, false);
            Assert.AreEqual(settings[1].Host, "www.example.com");
            Assert.AreEqual(settings[1].Port, (ushort)80);
            Assert.AreEqual(settings[1].Address, "127.0.0.1");
        }

        [TestMethod]
        public void TestValidRetrySetting()
        {
            List<RetrySetting> settings = new List<RetrySetting>();
            string[] testValues =
            {
                @"retry=10,1000",
                @"retry=20,0",
                @"#retry=10,1000",
                @"#retry=20,0"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(RetrySetting));

                settings.Add(hurlSetting as RetrySetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].RetryCount, 10);
            Assert.AreEqual(settings[0].RetryInterval, 1000u);
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].RetryCount, 20);
            Assert.AreEqual(settings[1].RetryInterval, 0u);
            Assert.AreEqual(settings[2].IsEnabled, false);
            Assert.AreEqual(settings[2].RetryCount, 10);
            Assert.AreEqual(settings[2].RetryInterval, 1000u);
            Assert.AreEqual(settings[3].IsEnabled, false);
            Assert.AreEqual(settings[3].RetryCount, 20);
            Assert.AreEqual(settings[3].RetryInterval, 0u);
        }

        [TestMethod]
        public void TestValidSslNoRevokeSettings()
        {
            List<SslNoRevokeSetting> settings = new List<SslNoRevokeSetting>();
            string[] testValues =
            {
                @"ssl_no_revoke=true",
                @"ssl_no_revoke=True",
                @"ssl_no_revoke=false",
                @"ssl_no_revoke=False",
                @"#ssl_no_revoke=true",
                @"#ssl_no_revoke=True",
                @"#ssl_no_revoke=false",
                @"#ssl_no_revoke=False",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(SslNoRevokeSetting));

                settings.Add(hurlSetting as SslNoRevokeSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].SslNoRevoke, true);
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].SslNoRevoke, true);
            Assert.AreEqual(settings[2].IsEnabled, true);
            Assert.AreEqual(settings[2].SslNoRevoke, false);
            Assert.AreEqual(settings[3].IsEnabled, true);
            Assert.AreEqual(settings[3].SslNoRevoke, false);
            Assert.AreEqual(settings[4].IsEnabled, false);
            Assert.AreEqual(settings[4].SslNoRevoke, true);
            Assert.AreEqual(settings[5].IsEnabled, false);
            Assert.AreEqual(settings[5].SslNoRevoke, true);
            Assert.AreEqual(settings[6].IsEnabled, false);
            Assert.AreEqual(settings[6].SslNoRevoke, false);
            Assert.AreEqual(settings[7].IsEnabled, false);
            Assert.AreEqual(settings[7].SslNoRevoke, false);
        }

        [TestMethod]
        public void TestValidTimeoutSettings()
        {
            List<TimeoutSetting> settings = new List<TimeoutSetting>();

            string[] testValues =
            {
                @"timeout=30,30",
                @"timeout=60,30",
                @"timeout=0,30",
                @"timeout=30,0",
                @"#timeout=30,30",
                @"#timeout=60,30",
                @"#timeout=0,30",
                @"#timeout=30,0",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(TimeoutSetting));

                settings.Add(hurlSetting as TimeoutSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(settings.Count, testValues.Length);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].ConnectTimeoutSeconds, 30u);
            Assert.AreEqual(settings[0].MaxTimeSeconds, 30u);
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].ConnectTimeoutSeconds, 60u);
            Assert.AreEqual(settings[1].MaxTimeSeconds, 30u);
            Assert.AreEqual(settings[2].IsEnabled, true);
            Assert.AreEqual(settings[2].ConnectTimeoutSeconds, 0u);
            Assert.AreEqual(settings[2].MaxTimeSeconds, 30u);
            Assert.AreEqual(settings[3].IsEnabled, true);
            Assert.AreEqual(settings[3].ConnectTimeoutSeconds, 30u);
            Assert.AreEqual(settings[3].MaxTimeSeconds, 0u);
            Assert.AreEqual(settings[4].IsEnabled, false);
            Assert.AreEqual(settings[4].ConnectTimeoutSeconds, 30u);
            Assert.AreEqual(settings[4].MaxTimeSeconds, 30u);
            Assert.AreEqual(settings[5].IsEnabled, false);
            Assert.AreEqual(settings[5].ConnectTimeoutSeconds, 60u);
            Assert.AreEqual(settings[5].MaxTimeSeconds, 30u);
            Assert.AreEqual(settings[6].IsEnabled, false);
            Assert.AreEqual(settings[6].ConnectTimeoutSeconds, 0u);
            Assert.AreEqual(settings[6].MaxTimeSeconds, 30u);
            Assert.AreEqual(settings[7].IsEnabled, false);
            Assert.AreEqual(settings[7].ConnectTimeoutSeconds, 30u);
            Assert.AreEqual(settings[7].MaxTimeSeconds, 0u);
        }

        [TestMethod]
        public void TestValidToEntrySettings()
        {
            List<ToEntrySetting> settings = new List<ToEntrySetting>();

            string[] testValues =
            {
                @"to_entry=3",
                @"to_entry=300",
                @"#to_entry=3",
                @"#to_entry=300"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(ToEntrySetting));

                settings.Add(hurlSetting as ToEntrySetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(settings.Count, testValues.Length);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].ToEntry, 3u);
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].ToEntry, 300u);
            Assert.AreEqual(settings[2].IsEnabled, false);
            Assert.AreEqual(settings[2].ToEntry, 3u);
            Assert.AreEqual(settings[3].IsEnabled, false);
            Assert.AreEqual(settings[3].ToEntry, 300u);
        }

        [TestMethod]
        public void TestValidUnixSocketSettings()
        {
            List<UnixSocketSetting> settings = new List<UnixSocketSetting>();
            string[] testValues =
            {
                "unix_socket=/var/run/docker.sock",
                "#unix_socket=/var/run/docker.sock"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(UnixSocketSetting));

                settings.Add(hurlSetting as UnixSocketSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(settings.Count, testValues.Length);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].Path, "/var/run/docker.sock");
            Assert.AreEqual(settings[1].IsEnabled, false);
            Assert.AreEqual(settings[1].Path, "/var/run/docker.sock");
        }

        [TestMethod]
        public void TestValidUserAgentSettings()
        {
            List<UserAgentSetting> settings = new List<UserAgentSetting>();
            const string USERAGENT = "Mozilla/5.0 (iPhone; CPU iPhone OS 5_0 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9A334 Safari/7534.48.3";

            string[] testValues =
            {
                "user_agent=Mozilla%2F5.0%20%28iPhone%3B%20CPU%20iPhone%20OS%205_0%20like%20Mac%20OS%20X%29%20AppleWebKit%2F534.46%20%28KHTML%2C%20like%20Gecko%29%20Version%2F5.1%20Mobile%2F9A334%20Safari%2F7534.48.3",
                "#user_agent=Mozilla%2F5.0%20%28iPhone%3B%20CPU%20iPhone%20OS%205_0%20like%20Mac%20OS%20X%29%20AppleWebKit%2F534.46%20%28KHTML%2C%20like%20Gecko%29%20Version%2F5.1%20Mobile%2F9A334%20Safari%2F7534.48.3",
                "user_agent=" + USERAGENT.EncodeUrl()
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(UserAgentSetting));

                settings.Add(hurlSetting as UserAgentSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(settings.Count, testValues.Length);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].UserAgent, USERAGENT);
            Assert.AreEqual(settings[1].IsEnabled, false);
            Assert.AreEqual(settings[1].UserAgent, USERAGENT);
            Assert.AreEqual(settings[2].IsEnabled, true);
            Assert.AreEqual(settings[2].UserAgent, USERAGENT);
        }

        [TestMethod]
        public void TestValidVariableSettings()
        {
            List<VariableSetting> settings = new List<VariableSetting>();
            string[] testValues =
            {
                "variable=test1,test2",
                "variable=test1,test2%2C3",
                "variable=test2,",
                "#variable=test1,test2",
                "#variable=test1,test2%2C3",
                "#variable=test2,"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(VariableSetting));

                settings.Add(hurlSetting as VariableSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(settings.Count, testValues.Length);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].Key, "test1");
            Assert.AreEqual(settings[0].Value, "test2");
            Assert.AreEqual(settings[1].IsEnabled, true);
            Assert.AreEqual(settings[1].Key, "test1");
            Assert.AreEqual(settings[1].Value, "test2,3");
            Assert.AreEqual(settings[2].IsEnabled, true);
            Assert.AreEqual(settings[2].Key, "test2");
            Assert.AreEqual(settings[2].Value, "");
            Assert.AreEqual(settings[3].IsEnabled, false);
            Assert.AreEqual(settings[3].Key, "test1");
            Assert.AreEqual(settings[3].Value, "test2");
            Assert.AreEqual(settings[4].IsEnabled, false);
            Assert.AreEqual(settings[4].Key, "test1");
            Assert.AreEqual(settings[4].Value, "test2,3");
            Assert.AreEqual(settings[5].IsEnabled, false);
            Assert.AreEqual(settings[5].Key, "test2");
            Assert.AreEqual(settings[5].Value, "");
        }

        [TestMethod]
        public void TestValidVariablesFileSettings()
        {
            List<VariablesFileSetting> settings = new List<VariablesFileSetting>();
            string[] testValues =
            {
                @"variables_file=D:/Files/vars.txt",
                @"#variables_file=D:/Files/vars.txt"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(VariablesFileSetting));

                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as VariablesFileSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].IsEnabled, true);
            Assert.AreEqual(settings[0].File, "D:/Files/vars.txt");
            Assert.AreEqual(settings[1].IsEnabled, false);
            Assert.AreEqual(settings[1].File, "D:/Files/vars.txt");
        }
    }
}