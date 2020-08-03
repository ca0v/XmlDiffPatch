using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmlDiffPatchTests
{
    [TestClass]
    public class XmlDiffPatchUnitTests
    {
        [TestMethod]
        public void SameNodeEqualTest()
        {
            var doc1 = new System.Xml.XmlDocument();
            doc1.LoadXml("<XML>TEST</XML>");
            var node1 = doc1.FirstChild;
            var diff = new Microsoft.XmlDiffPatch.XmlDiff();
            Assert.IsTrue(diff.Compare(node1, node1));
        }
    }
}
