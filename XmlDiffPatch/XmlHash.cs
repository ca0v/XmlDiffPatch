// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlHash
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlHash
  {
    private bool _bIgnoreChildOrder = false;
    private bool _bIgnoreComments = false;
    private bool _bIgnorePI = false;
    private bool _bIgnoreWhitespace = false;
    private bool _bIgnoreNamespaces = false;
    private bool _bIgnorePrefixes = false;
    private bool _bIgnoreXmlDecl = false;
    private bool _bIgnoreDtd = false;
    private const string Delimiter = "\0x01";

    internal XmlHash(XmlDiff xmlDiff)
    {
      this._bIgnoreChildOrder = xmlDiff.IgnoreChildOrder;
      this._bIgnoreComments = xmlDiff.IgnoreComments;
      this._bIgnorePI = xmlDiff.IgnorePI;
      this._bIgnoreWhitespace = xmlDiff.IgnoreWhitespace;
      this._bIgnoreNamespaces = xmlDiff.IgnoreNamespaces;
      this._bIgnorePrefixes = xmlDiff.IgnorePrefixes;
      this._bIgnoreXmlDecl = xmlDiff.IgnoreXmlDecl;
      this._bIgnoreDtd = xmlDiff.IgnoreDtd;
    }

    internal XmlHash()
    {
    }

    private void ClearFlags()
    {
      this._bIgnoreChildOrder = false;
      this._bIgnoreComments = false;
      this._bIgnorePI = false;
      this._bIgnoreWhitespace = false;
      this._bIgnoreNamespaces = false;
      this._bIgnorePrefixes = false;
      this._bIgnoreXmlDecl = false;
      this._bIgnoreDtd = false;
    }

    internal ulong ComputeHash(XmlNode node, XmlDiffOptions options)
    {
      this._bIgnoreChildOrder = (options & XmlDiffOptions.IgnoreChildOrder) > XmlDiffOptions.None;
      this._bIgnoreComments = (options & XmlDiffOptions.IgnoreComments) > XmlDiffOptions.None;
      this._bIgnorePI = (options & XmlDiffOptions.IgnorePI) > XmlDiffOptions.None;
      this._bIgnoreWhitespace = (options & XmlDiffOptions.IgnoreWhitespace) > XmlDiffOptions.None;
      this._bIgnoreNamespaces = (options & XmlDiffOptions.IgnoreNamespaces) > XmlDiffOptions.None;
      this._bIgnorePrefixes = (options & XmlDiffOptions.IgnorePrefixes) > XmlDiffOptions.None;
      this._bIgnoreXmlDecl = (options & XmlDiffOptions.IgnoreXmlDecl) > XmlDiffOptions.None;
      this._bIgnoreDtd = (options & XmlDiffOptions.IgnoreDtd) > XmlDiffOptions.None;
      return this.ComputeHash(node);
    }

    internal ulong ComputeHash(XmlNode node)
    {
      switch (node.NodeType)
      {
        case XmlNodeType.Document:
          return this.ComputeHashXmlDocument((XmlDocument) node);
        case XmlNodeType.DocumentFragment:
          return this.ComputeHashXmlFragment((XmlDocumentFragment) node);
        default:
          return this.ComputeHashXmlNode(node);
      }
    }

    private ulong ComputeHashXmlDocument(XmlDocument doc)
    {
            var ha = new HashAlgorithm();
      this.HashDocument(ha);
      this.ComputeHashXmlChildren(ha, (XmlNode) doc);
      return ha.Hash;
    }

    private ulong ComputeHashXmlFragment(XmlDocumentFragment frag)
    {
            var ha = new HashAlgorithm();
      this.ComputeHashXmlChildren(ha, (XmlNode) frag);
      return ha.Hash;
    }

    internal ulong ComputeHashXmlDiffDocument(XmlDiffDocument doc)
    {
            var ha = new HashAlgorithm();
      this.HashDocument(ha);
      this.ComputeHashXmlDiffChildren(ha, (XmlDiffParentNode) doc);
      return ha.Hash;
    }

    internal ulong ComputeHashXmlDiffElement(XmlDiffElement el)
    {
            var ha = new HashAlgorithm();
      this.HashElement(ha, el.LocalName, el.Prefix, el.NamespaceURI);
      this.ComputeHashXmlDiffAttributes(ha, el);
      this.ComputeHashXmlDiffChildren(ha, (XmlDiffParentNode) el);
      return ha.Hash;
    }

    private void ComputeHashXmlDiffAttributes(HashAlgorithm ha, XmlDiffElement el)
    {
      var i = 0;
      ulong u = 0;
      for (var attributeOrNamespace = el._attributes; attributeOrNamespace != null; attributeOrNamespace = (XmlDiffAttributeOrNamespace) attributeOrNamespace._nextSibling)
      {
        u += attributeOrNamespace.HashValue;
        ++i;
      }
      if (i <= 0)
        return;
      ha.AddULong(u);
      ha.AddInt(i);
    }

    private void ComputeHashXmlDiffChildren(HashAlgorithm ha, XmlDiffParentNode parent)
    {
      var i = 0;
      if (this._bIgnoreChildOrder)
      {
        ulong u = 0;
        for (var xmlDiffNode = parent.FirstChildNode; xmlDiffNode != null; xmlDiffNode = xmlDiffNode._nextSibling)
        {
          u += xmlDiffNode.HashValue;
          ++i;
        }
        ha.AddULong(u);
      }
      else
      {
        for (var xmlDiffNode = parent.FirstChildNode; xmlDiffNode != null; xmlDiffNode = xmlDiffNode._nextSibling)
        {
          ha.AddULong(xmlDiffNode.HashValue);
          ++i;
        }
      }
      if (i == 0)
        return;
      ha.AddInt(i);
    }

    private void ComputeHashXmlChildren(HashAlgorithm ha, XmlNode parent)
    {
      if (parent is XmlElement)
      {
        ulong u = 0;
        var i = 0;
                var attributes = parent.Attributes;
        for (var index = 0; index < attributes.Count; ++index)
        {
                    var xmlAttribute = (XmlAttribute) attributes.Item(index);
          ulong num;
          if (xmlAttribute.LocalName == "xmlns" && xmlAttribute.Prefix == string.Empty)
          {
            if (!this._bIgnoreNamespaces)
              num = this.HashNamespace(string.Empty, xmlAttribute.Value);
            else
              continue;
          }
          else if (xmlAttribute.Prefix == "xmlns")
          {
            if (!this._bIgnoreNamespaces)
              num = this.HashNamespace(xmlAttribute.LocalName, xmlAttribute.Value);
            else
              continue;
          }
          else
            num = !this._bIgnoreWhitespace ? this.HashAttribute(xmlAttribute.LocalName, xmlAttribute.Prefix, xmlAttribute.NamespaceURI, xmlAttribute.Value) : this.HashAttribute(xmlAttribute.LocalName, xmlAttribute.Prefix, xmlAttribute.NamespaceURI, XmlDiff.NormalizeText(xmlAttribute.Value));
          ++i;
          u += num;
        }
        if (i != 0)
        {
          ha.AddULong(u);
          ha.AddInt(i);
        }
      }
      var i1 = 0;
      if (this._bIgnoreChildOrder)
      {
        ulong u = 0;
        for (var node = parent.FirstChild; node != null; node = node.NextSibling)
        {
          var hashXmlNode = this.ComputeHashXmlNode(node);
          if (hashXmlNode != 0UL)
          {
            u += hashXmlNode;
            ++i1;
          }
        }
        ha.AddULong(u);
      }
      else
      {
        for (var node = parent.FirstChild; node != null; node = node.NextSibling)
        {
          var hashXmlNode = this.ComputeHashXmlNode(node);
          if (hashXmlNode != 0UL)
          {
            ha.AddULong(hashXmlNode);
            ++i1;
          }
        }
      }
      if (i1 == 0)
        return;
      ha.AddInt(i1);
    }

    private ulong ComputeHashXmlNode(XmlNode node)
    {
      switch (node.NodeType)
      {
        case XmlNodeType.Element:
                    var xmlElement = (XmlElement) node;
                    var ha = new HashAlgorithm();
          this.HashElement(ha, xmlElement.LocalName, xmlElement.Prefix, xmlElement.NamespaceURI);
          this.ComputeHashXmlChildren(ha, (XmlNode) xmlElement);
          return ha.Hash;
        case XmlNodeType.Attribute:
          return 0;
        case XmlNodeType.Text:
                    var xmlCharacterData1 = (XmlCharacterData) node;
          return this._bIgnoreWhitespace ? this.HashCharacterNode(xmlCharacterData1.NodeType, XmlDiff.NormalizeText(xmlCharacterData1.Value)) : this.HashCharacterNode(xmlCharacterData1.NodeType, xmlCharacterData1.Value);
        case XmlNodeType.CDATA:
                    var xmlCharacterData2 = (XmlCharacterData) node;
          return this.HashCharacterNode(xmlCharacterData2.NodeType, xmlCharacterData2.Value);
        case XmlNodeType.EntityReference:
          return this.HashER(node.Name);
        case XmlNodeType.ProcessingInstruction:
          if (this._bIgnorePI)
            return 0;
                    var processingInstruction = (XmlProcessingInstruction) node;
          return this.HashPI(processingInstruction.Target, processingInstruction.Value);
        case XmlNodeType.Comment:
          return !this._bIgnoreComments ? this.HashCharacterNode(XmlNodeType.Comment, node.Value) : 0UL;
        case XmlNodeType.DocumentType:
          if (this._bIgnoreDtd)
            return 0;
                    var xmlDocumentType = (XmlDocumentType) node;
          return this.HashDocumentType(xmlDocumentType.Name, xmlDocumentType.PublicId, xmlDocumentType.SystemId, xmlDocumentType.InternalSubset);
        case XmlNodeType.DocumentFragment:
          return 0;
        case XmlNodeType.Whitespace:
          return 0;
        case XmlNodeType.SignificantWhitespace:
          if (this._bIgnoreWhitespace)
            return 0;
          goto case XmlNodeType.Text;
        case XmlNodeType.XmlDeclaration:
          return this._bIgnoreXmlDecl ? 0UL : this.HashXmlDeclaration(XmlDiff.NormalizeXmlDeclaration(node.Value));
        default:
          return 0;
      }
    }

    private void HashDocument(HashAlgorithm ha)
    {
    }

    internal void HashElement(HashAlgorithm ha, string localName, string prefix, string ns)
    {
      ha.AddString(1.ToString() + "\0x01" + (this._bIgnoreNamespaces || this._bIgnorePrefixes ? (object) string.Empty : (object) prefix) + "\0x01" + (this._bIgnoreNamespaces ? (object) string.Empty : (object) ns) + "\0x01" + localName);
    }

    internal ulong HashAttribute(string localName, string prefix, string ns, string value)
    {
      return HashAlgorithm.GetHash(2.ToString() + "\0x01" + (this._bIgnoreNamespaces || this._bIgnorePrefixes ? (object) string.Empty : (object) prefix) + "\0x01" + (this._bIgnoreNamespaces ? (object) string.Empty : (object) ns) + "\0x01" + localName + "\0x01" + value);
    }

    internal ulong HashNamespace(string prefix, string ns)
    {
      return HashAlgorithm.GetHash(100.ToString() + "\0x01" + (this._bIgnorePrefixes ? (object) string.Empty : (object) prefix) + "\0x01" + ns);
    }

    internal ulong HashCharacterNode(XmlNodeType nodeType, string value)
    {
      return HashAlgorithm.GetHash(((int) nodeType).ToString() + "\0x01" + value);
    }

    internal ulong HashPI(string target, string value)
    {
      return HashAlgorithm.GetHash(7.ToString() + "\0x01" + target + "\0x01" + value);
    }

    internal ulong HashER(string name)
    {
      return HashAlgorithm.GetHash(5.ToString() + "\0x01" + name);
    }

    internal ulong HashXmlDeclaration(string value)
    {
      return HashAlgorithm.GetHash(17.ToString() + "\0x01" + value);
    }

    internal ulong HashDocumentType(string name, string publicId, string systemId, string subset)
    {
      return HashAlgorithm.GetHash(10.ToString() + "\0x01" + name + "\0x01" + publicId + "\0x01" + systemId + "\0x01" + subset);
    }
  }
}
