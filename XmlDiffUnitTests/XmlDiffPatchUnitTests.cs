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

        [TestMethod]
        public void DiffNodeEqualTest()
        {
            var xml = "<XML>TEST</XML>";
            var doc1 = new System.Xml.XmlDocument();
            doc1.LoadXml(xml);
            var doc2 = new System.Xml.XmlDocument();
            doc2.LoadXml(xml);
            var node1 = doc1.FirstChild;
            var node2 = doc2.FirstChild;
            var diff = new Microsoft.XmlDiffPatch.XmlDiff();
            Assert.IsTrue(diff.Compare(node1, node2));
        }

        [TestMethod]
        public void DiffNsPrefixEqualTest()
        {
            var doc1 = new System.Xml.XmlDocument();
            doc1.LoadXml("<XML xmlns:foo=\"http://www.opengis.net/ogc\"><foo:test>TEST</foo:test></XML>");
            var node1 = doc1.FirstChild;
            var doc2 = new System.Xml.XmlDocument();
            doc2.LoadXml("<XML xmlns:bar=\"http://www.opengis.net/ogc\"><bar:test>TEST</bar:test></XML>");
            var node2 = doc2.FirstChild;
            var diff = new Microsoft.XmlDiffPatch.XmlDiff(Microsoft.XmlDiffPatch.XmlDiffOptions.IgnorePrefixes);
            Assert.IsTrue(diff.Compare(node1, node2));
        }

    }
}
