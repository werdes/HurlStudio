using HurlStudio.Collections.Settings;
using HurlStudio.Collections.Utility;

namespace HurlStudio.Tests
{
    [TestClass]
    public class SettingsTests
    {
        private ISettingParser? _parser;

        [TestInitialize]
        public void Init()
        {
            _parser = new IniSettingParser();
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

                settings.Add(hurlSetting as AwsSigV4Setting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(settings.Count, testValues.Length);
            Assert.AreEqual(settings[0].Provider1, "aws");
            Assert.AreEqual(settings[0].Provider2, "amz");
            Assert.AreEqual(settings[0].Region, "eu-central-1");
            Assert.AreEqual(settings[0].Service, "foos");
            Assert.AreEqual(settings[1].Provider1, "aws");
            Assert.AreEqual(settings[1].Provider2, "");
            Assert.AreEqual(settings[1].Region, "eu-central-1");
            Assert.AreEqual(settings[1].Service, "foos");
            Assert.AreEqual(settings[2].Provider1, "aws");
            Assert.AreEqual(settings[2].Provider2, "amz");
            Assert.AreEqual(settings[2].Region, "");
            Assert.AreEqual(settings[2].Service, "foos");
            Assert.AreEqual(settings[3].Provider1, "aws");
            Assert.AreEqual(settings[3].Provider2, "amz");
            Assert.AreEqual(settings[3].Region, "");
            Assert.AreEqual(settings[3].Service, "");
            Assert.AreEqual(settings[4].Provider1, "aws");
            Assert.AreEqual(settings[4].Provider2, "9");
            Assert.AreEqual(settings[4].Region, "");
            Assert.AreEqual(settings[4].Service, "");
            Assert.AreEqual(settings[5].Provider1, "aws");
            Assert.AreEqual(settings[5].Provider2, ",");
            Assert.AreEqual(settings[5].Region, "");
            Assert.AreEqual(settings[5].Service, "");
            Assert.AreEqual(settings[6].Provider1, "aws");
            Assert.AreEqual(settings[6].Provider2, "");
            Assert.AreEqual(settings[6].Region, "");
            Assert.AreEqual(settings[6].Service, "foos");
            Assert.AreEqual(settings[7].Provider1, "aws");
            Assert.AreEqual(settings[7].Provider2, "");
            Assert.AreEqual(settings[7].Region, "");
            Assert.AreEqual(settings[7].Service, "");
            Assert.AreEqual(settings[8].Provider1, "");
            Assert.AreEqual(settings[8].Provider2, "");
            Assert.AreEqual(settings[8].Region, "");
            Assert.AreEqual(settings[8].Service, "");
        }

        [TestMethod]
        public void TestValidBasicUserSettings()
        {
            List<BasicUserSetting> settings = new List<BasicUserSetting>();
            string[] testValues =
            {
                "user=user1:cGFzc3dvcmQx", // password1
                "user=user2:",
                "user=user3:",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(BasicUserSetting));

                settings.Add(hurlSetting as BasicUserSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(settings.Count, testValues.Length);
            Assert.AreEqual(settings[0].User, "user1");
            Assert.AreEqual(settings[0].Password, "password1");
            Assert.AreEqual(settings[1].User, "user2");
            Assert.AreEqual(settings[1].Password, null);
            Assert.AreEqual(settings[2].User, "user3");
            Assert.AreEqual(settings[2].Password, null);
        }


        [TestMethod]
        public void TestValidCaCertSettings()
        {
            string[] testValues =
            {
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
            string[] testValues =
            {
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
        public void TestValidConnectToSettings()
        {
            List<ConnectToSetting> settings = new List<ConnectToSetting>();
            string[] testValues =
            {
                @"connect_to=google.com:80:bing.com:8080"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(ConnectToSetting));

                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as ConnectToSetting ?? throw new InvalidOperationException());
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
            string[] testValues =
            {
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
                settings.Add(hurlSetting as ContinueOnErrorSetting ?? throw new InvalidOperationException());
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
            string[] testValues =
            {
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
                settings.Add(hurlSetting as CookieSetting ?? throw new InvalidOperationException());
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
            string[] testValues =
            {
                @"delay=1000",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(DelaySetting));

                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as DelaySetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(1, settings.Count);
            Assert.AreEqual(settings[0].Delay, 1000u);
        }

        [TestMethod]
        public void TestValidFileRootSettings()
        {
            List<FileRootSetting> settings = new List<FileRootSetting>();
            string[] testValues =
            {
                @"file_root=D:\Files\HurlStudio\test\",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(FileRootSetting));

                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as FileRootSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(1, settings.Count);
            Assert.AreEqual(settings[0].Directory, @"D:\Files\HurlStudio\test\");
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
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsInstanceOfType(hurlSetting, typeof(HttpVersionSetting));

                Assert.IsNotNull(hurlSetting);
                settings.Add(hurlSetting as HttpVersionSetting ?? throw new InvalidOperationException());
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
            string[] testValues =
            {
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
                settings.Add(hurlSetting as IgnoreAssertsSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(4, settings.Count);
            Assert.AreEqual(settings[0].IgnoreAsserts, true);
            Assert.AreEqual(settings[1].IgnoreAsserts, false);
            Assert.AreEqual(settings[2].IgnoreAsserts, true);
            Assert.AreEqual(settings[3].IgnoreAsserts, false);
        }

        [TestMethod]
        public void TestValidNoProxySettings()
        {
            List<NoProxySetting> settings = new List<NoProxySetting>();
            string[] testValues =
            {
                @"no_proxy=google.com,cloudflare.com,amazon.com",
                @"no_proxy=google.com"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(NoProxySetting));

                settings.Add(hurlSetting as NoProxySetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].NoProxyHosts.Count, 3);
            Assert.AreEqual(settings[0].NoProxyHosts[0], "google.com");
            Assert.AreEqual(settings[0].NoProxyHosts[1], "cloudflare.com");
            Assert.AreEqual(settings[0].NoProxyHosts[2], "amazon.com");

            Assert.AreEqual(settings[1].NoProxyHosts.Count, 1);
            Assert.AreEqual(settings[1].NoProxyHosts[0], "google.com");
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
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(PathAsIsSetting));

                settings.Add(hurlSetting as PathAsIsSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].PathAsIs, true);
            Assert.AreEqual(settings[1].PathAsIs, true);
            Assert.AreEqual(settings[2].PathAsIs, false);
            Assert.AreEqual(settings[3].PathAsIs, false);
        }

        [TestMethod]
        public void TestValidProxySettings()
        {
            string[] testValues =
            {
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
        public void TestValidRedirectionsSettings()
        {
            List<RedirectionsSetting> settings = new List<RedirectionsSetting>();
            string[] testValues =
            {
                @"redirections=true",
                @"redirections=True",
                @"redirections=false",
                @"redirections=False",
                @"redirections=true:true",
                @"redirections=true:false",
                @"redirections=true:true:50"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(RedirectionsSetting));

                settings.Add(hurlSetting as RedirectionsSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].AllowRedirections, true);
            Assert.AreEqual(settings[1].AllowRedirections, true);
            Assert.AreEqual(settings[2].AllowRedirections, false);
            Assert.AreEqual(settings[3].AllowRedirections, false);
            Assert.AreEqual(settings[4].AllowRedirections, true);
            Assert.AreEqual(settings[4].RedirectionsTrusted, true);
            Assert.AreEqual(settings[5].AllowRedirections, true);
            Assert.AreEqual(settings[5].RedirectionsTrusted, false);
            Assert.AreEqual(settings[6].AllowRedirections, true);
            Assert.AreEqual(settings[6].RedirectionsTrusted, true);
            Assert.AreEqual(settings[6].MaxRedirections, 50u);
        }

        [TestMethod]
        public void TestValidResolveSetting()
        {
            List<ResolveSetting> settings = new List<ResolveSetting>();
            string[] testValues =
            {
                @"resolve=www.example.com:80:127.0.0.1"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(ResolveSetting));

                settings.Add(hurlSetting as ResolveSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].Host, "www.example.com");
            Assert.AreEqual(settings[0].Port, (ushort)80);
            Assert.AreEqual(settings[0].Address, "127.0.0.1");
        }

        [TestMethod]
        public void TestValidRetrySetting()
        {
            List<RetrySetting> settings = new List<RetrySetting>();
            string[] testValues =
            {
                @"retry=10:1000",
                @"retry=20:0"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(RetrySetting));

                settings.Add(hurlSetting as RetrySetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].RetryCount, 10u);
            Assert.AreEqual(settings[0].RetryInterval, 1000u);
            Assert.AreEqual(settings[1].RetryCount, 20u);
            Assert.AreEqual(settings[1].RetryInterval, 0u);
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
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(SslNoRevokeSetting));

                settings.Add(hurlSetting as SslNoRevokeSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(testValues.Length, settings.Count);
            Assert.AreEqual(settings[0].SslNoRevoke, true);
            Assert.AreEqual(settings[1].SslNoRevoke, true);
            Assert.AreEqual(settings[2].SslNoRevoke, false);
            Assert.AreEqual(settings[3].SslNoRevoke, false);
        }

        [TestMethod]
        public void TestValidTimeoutSettings()
        {
            List<TimeoutSetting> settings = new List<TimeoutSetting>();

            string[] testValues =
            {
                @"timeout=30:30",
                @"timeout=60:30",
                @"timeout=0:30",
                @"timeout=30:0",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(TimeoutSetting));

                settings.Add(hurlSetting as TimeoutSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(settings.Count, testValues.Length);
            Assert.AreEqual(settings[0].ConnectTimeoutSeconds, 30u);
            Assert.AreEqual(settings[0].MaxTimeSeconds, 30u);
            Assert.AreEqual(settings[1].ConnectTimeoutSeconds, 60u);
            Assert.AreEqual(settings[1].MaxTimeSeconds, 30u);
            Assert.AreEqual(settings[2].ConnectTimeoutSeconds, 0u);
            Assert.AreEqual(settings[2].MaxTimeSeconds, 30u);
            Assert.AreEqual(settings[3].ConnectTimeoutSeconds, 30u);
            Assert.AreEqual(settings[3].MaxTimeSeconds, 0u);
        }

        [TestMethod]
        public void TestValidToEntrySettings()
        {
            List<ToEntrySetting> settings = new List<ToEntrySetting>();

            string[] testValues =
            {
                @"to_entry=3",
                @"to_entry=300"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(ToEntrySetting));

                settings.Add(hurlSetting as ToEntrySetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(settings.Count, testValues.Length);
            Assert.AreEqual(settings[0].ToEntry, 3u);
            Assert.AreEqual(settings[1].ToEntry, 300u);
        }

        [TestMethod]
        public void TestValidUnixSocketSettings()
        {
            List<UnixSocketSetting> settings = new List<UnixSocketSetting>();
            string[] testValues =
            {
                "unix_socket=/var/run/docker.sock"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(UnixSocketSetting));

                settings.Add(hurlSetting as UnixSocketSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(settings.Count, testValues.Length);
            Assert.AreEqual(settings[0].Path, "/var/run/docker.sock");
        }

        [TestMethod]
        public void TestValidUserAgentSettings()
        {
            List<UserAgentSetting> settings = new List<UserAgentSetting>();
            string[] testValues =
            {
                "user_agent=Mozilla/5.0 (iPhone; CPU iPhone OS 5_0 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9A334 Safari/7534.48.3",
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(UserAgentSetting));

                settings.Add(hurlSetting as UserAgentSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(settings.Count, testValues.Length);
            Assert.AreEqual(settings[0].UserAgent,
                "Mozilla/5.0 (iPhone; CPU iPhone OS 5_0 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9A334 Safari/7534.48.3");
        }

        [TestMethod]
        public void TestValidVariableSettings()
        {
            List<VariableSetting> settings = new List<VariableSetting>();
            string[] testValues =
            {
                "variable=test1:test2",
                "variable=test1:test2:3",
                "variable=test2:"
            };

            foreach (string testValue in testValues)
            {
                IHurlSetting? hurlSetting = _parser?.Parse(testValue);
                Assert.IsNotNull(hurlSetting);
                Assert.IsInstanceOfType(hurlSetting, typeof(VariableSetting));

                settings.Add(hurlSetting as VariableSetting ?? throw new InvalidOperationException());
            }

            Assert.AreEqual(settings.Count, testValues.Length);
            Assert.AreEqual(settings[0].Key, "test1");
            Assert.AreEqual(settings[0].Value, "test2");
            Assert.AreEqual(settings[1].Key, "test1");
            Assert.AreEqual(settings[1].Value, "test2:3");
            Assert.AreEqual(settings[2].Key, "test2");
            Assert.AreEqual(settings[2].Value, "");
        }
    }
}