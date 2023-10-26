using HurlUI.Collections;
using HurlUI.Collections.Model.Collection;
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
                Path.Combine("Assets", "Collections", "ValidCollection.hurlcol"), Encoding.UTF8).Result;
            Assert.IsNotNull(collection);
        }
    }
}