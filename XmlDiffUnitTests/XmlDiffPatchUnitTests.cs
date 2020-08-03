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
        public void DiffWhitespaceEqualTest()
        {
            var node1 = LoadNode("<XML><TEST>TEST1</TEST><TEST id=\"id\">TEST1</TEST></XML>");
            var node2 = LoadNode("<XML>\n\t<TEST>TEST1</TEST>\n\t<TEST id = 'id' >\n\t\tTEST1\n\t\t</TEST>\n</XML>");
            var diff = new Microsoft.XmlDiffPatch.XmlDiff(Microsoft.XmlDiffPatch.XmlDiffOptions.IgnoreWhitespace);
            Assert.IsTrue(diff.Compare(node1, node2));
        }

        private static System.Xml.XmlNode LoadNode(string xml1)
        {
            var doc1 = new System.Xml.XmlDocument();
            doc1.LoadXml(xml1);
            var node1 = doc1.FirstChild;
            return node1;
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

        [TestMethod]
        public void DiffChildOrderEqualTest()
        {
            var doc1 = new System.Xml.XmlDocument();
            doc1.LoadXml("<XML xmlns:foo=\"http://www.opengis.net/ogc\"><foo:test>TEST1</foo:test><foo:test>TEST2</foo:test></XML>");
            var node1 = doc1.FirstChild;
            var doc2 = new System.Xml.XmlDocument();
            doc2.LoadXml("<XML xmlns:bar=\"http://www.opengis.net/ogc\"><bar:test>TEST2</bar:test><bar:test>TEST1</bar:test></XML>");
            var node2 = doc2.FirstChild;
            var diff = new Microsoft.XmlDiffPatch.XmlDiff(Microsoft.XmlDiffPatch.XmlDiffOptions.IgnorePrefixes | Microsoft.XmlDiffPatch.XmlDiffOptions.IgnoreChildOrder);
            Assert.IsTrue(diff.Compare(node1, node2));
        }


        [TestMethod]
        public void DiffCommentsEqualTest()
        {
            var doc1 = new System.Xml.XmlDocument();
            doc1.LoadXml("<XML xmlns:foo=\"http://www.opengis.net/ogc\"><!--foo--><foo:test>TEST1</foo:test><foo:test>TEST2</foo:test></XML>");
            var node1 = doc1.FirstChild;
            var doc2 = new System.Xml.XmlDocument();
            doc2.LoadXml("<XML xmlns:bar=\"http://www.opengis.net/ogc\"><!--bar--><bar:test>TEST2</bar:test><bar:test>TEST1</bar:test></XML>");
            var node2 = doc2.FirstChild;
            var diff = new Microsoft.XmlDiffPatch.XmlDiff(Microsoft.XmlDiffPatch.XmlDiffOptions.IgnorePrefixes | Microsoft.XmlDiffPatch.XmlDiffOptions.IgnoreChildOrder | Microsoft.XmlDiffPatch.XmlDiffOptions.IgnoreComments);
            Assert.IsTrue(diff.Compare(node1, node2));
        }

        [TestMethod]
        public void DiffXmlDeclEqualTest()
        {
            var doc1 = new System.Xml.XmlDocument();
            doc1.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?><XML xmlns:foo=\"http://www.opengis.net/ogc\"><!--foo--><foo:test>TEST1</foo:test><foo:test>TEST2</foo:test></XML>");
            var node1 = doc1.FirstChild;
            var doc2 = new System.Xml.XmlDocument();
            doc2.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-16\"?><XML xmlns:bar=\"http://www.opengis.net/ogc\"><!--bar--><bar:test>TEST2</bar:test><bar:test>TEST1</bar:test></XML>");
            var node2 = doc2.FirstChild;
            var diff = new Microsoft.XmlDiffPatch.XmlDiff(Microsoft.XmlDiffPatch.XmlDiffOptions.IgnorePrefixes | Microsoft.XmlDiffPatch.XmlDiffOptions.IgnoreChildOrder | Microsoft.XmlDiffPatch.XmlDiffOptions.IgnoreComments | Microsoft.XmlDiffPatch.XmlDiffOptions.IgnoreXmlDecl);
            Assert.IsTrue(diff.Compare(node1, node2));
        }
    }
}
