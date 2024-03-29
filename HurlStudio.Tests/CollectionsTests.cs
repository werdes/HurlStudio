using HurlStudio.Collections;
using HurlStudio.Collections.Model.Collection;
using HurlStudio.Collections.Settings;
using HurlStudio.Collections.Utility;
using System.Text;

namespace HurlStudio.Tests
{
    [TestClass]
    public class CollectionsTests
    {
        [TestMethod]
        public void TestValidCollection()
        {
            IniCollectionSerializer iniCollectionSerializer = new IniCollectionSerializer(new IniSettingParser());

            HurlCollection collection = iniCollectionSerializer.DeserializeFileAsync(
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
        }                                                                     
    }}