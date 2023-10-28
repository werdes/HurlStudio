using HurlUI.Collections;
using HurlUI.Collections.Model.Collection;
using HurlUI.Collections.Settings;
using HurlUI.Collections.Utility;
using System.Text;

namespace HurlUI.Tests
{
    [TestClass]
    public class CollectionsTests
    {
        [TestMethod]
        public void TestValidCollection()
        {
            HurlCollection collection = IniCollectionSerializer.Instance.DeserializeFileAsync(
                Path.Combine("Assets", "Collections", "ValidCollection.hurlc"), Encoding.UTF8).Result;
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Name.Equals("Valid collection"));
            Assert.IsTrue(collection.Locations.Count == 1);
            Assert.IsTrue(collection.Locations.First() == "../HurlFiles/");
            Assert.IsTrue(collection.CollectionSettings.Count == 1);
            Assert.IsNotNull(collection.CollectionSettings.FirstOrDefault());

            // Proxy-Setting in collection settings
            // proxy=protocol:https,host:testproxy.local,port:8080
            Assert.IsInstanceOfType(collection.CollectionSettings.FirstOrDefault(), typeof(ProxySetting));
            Assert.IsTrue((collection.CollectionSettings.FirstOrDefault() as ProxySetting)?.Host == "testproxy.local");
            Assert.IsTrue((collection.CollectionSettings.FirstOrDefault() as ProxySetting)?.Protocol == Common.Enums.ProxyProtocol.HTTPS);
            Assert.IsTrue((collection.CollectionSettings.FirstOrDefault() as ProxySetting)?.Port == 8080);

            // Folder Settings
            Assert.IsTrue(collection.FolderSettings.Count == 1);
            Assert.IsNotNull(collection.FolderSettings.FirstOrDefault());
            Assert.IsTrue(collection.FolderSettings.FirstOrDefault()?.Directory == "../HurlFiles/");
        }
    }
}