// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffElement
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffElement : XmlDiffParentNode
  {
    internal XmlDiffAttributeOrNamespace _attributes = (XmlDiffAttributeOrNamespace) null;
    internal ulong _allAttributesHash = 0;
    internal ulong _attributesHashAH = 0;
    internal ulong _attributesHashIQ = 0;
    internal ulong _attributesHashRZ = 0;
    private string _localName;
    private string _prefix;
    private string _ns;

    internal XmlDiffElement(int position, string localName, string prefix, string ns)
      : base(position)
    {
      this._localName = localName;
      this._prefix = prefix;
      this._ns = ns;
    }

    internal override XmlDiffNodeType NodeType
    {
      get
      {
        return XmlDiffNodeType.Element;
      }
    }

    internal string LocalName
    {
      get
      {
        return this._localName;
      }
    }

    internal string NamespaceURI
    {
      get
      {
        return this._ns;
      }
    }

    internal string Prefix
    {
      get
      {
        return this._prefix;
      }
    }

    internal string Name
    {
      get
      {
        return this._prefix.Length > 0 ? this._prefix + ":" + this._localName : this._localName;
      }
    }

    internal override void ComputeHashValue(XmlHash xmlHash)
    {
      this._hashValue = xmlHash.ComputeHashXmlDiffElement(this);
    }

    internal override XmlDiffOperation GetDiffOperation(
      XmlDiffNode changedNode,
      XmlDiff xmlDiff)
    {
      if (changedNode.NodeType != XmlDiffNodeType.Element)
        return XmlDiffOperation.Undefined;
      XmlDiffElement xmlDiffElement = (XmlDiffElement) changedNode;
      bool flag = false;
      if (this.LocalName == xmlDiffElement.LocalName)
      {
        if (xmlDiff.IgnoreNamespaces)
          flag = true;
        else if (this.NamespaceURI == xmlDiffElement.NamespaceURI && (xmlDiff.IgnorePrefixes || this.Prefix == xmlDiffElement.Prefix))
          flag = true;
      }
      if ((long) xmlDiffElement._allAttributesHash == (long) this._allAttributesHash)
        return !flag ? XmlDiffOperation.ChangeElementName : XmlDiffOperation.Match;
      int num = ((long) xmlDiffElement._attributesHashAH == (long) this._attributesHashAH ? 0 : 1) + ((long) xmlDiffElement._attributesHashIQ == (long) this._attributesHashIQ ? 0 : 1) + ((long) xmlDiffElement._attributesHashRZ == (long) this._attributesHashRZ ? 0 : 1);
      return flag ? (XmlDiffOperation) (3 + num) : (XmlDiffOperation) (6 + num);
    }

    internal override bool IsSameAs(XmlDiffNode node, XmlDiff xmlDiff)
    {
      if (node.NodeType != XmlDiffNodeType.Element)
        return false;
      XmlDiffElement xmlDiffElement = (XmlDiffElement) node;
      if (this.LocalName != xmlDiffElement.LocalName || !xmlDiff.IgnoreNamespaces && (this.NamespaceURI != xmlDiffElement.NamespaceURI || !xmlDiff.IgnorePrefixes && this.Prefix != xmlDiffElement.Prefix))
        return false;
      XmlDiffAttributeOrNamespace attributeOrNamespace1 = this._attributes;
      while (attributeOrNamespace1 != null && attributeOrNamespace1.NodeType == XmlDiffNodeType.Namespace)
        attributeOrNamespace1 = (XmlDiffAttributeOrNamespace) attributeOrNamespace1._nextSibling;
      XmlDiffAttributeOrNamespace attributeOrNamespace2 = this._attributes;
      while (attributeOrNamespace2 != null && attributeOrNamespace2.NodeType == XmlDiffNodeType.Namespace)
        attributeOrNamespace2 = (XmlDiffAttributeOrNamespace) attributeOrNamespace2._nextSibling;
      for (; attributeOrNamespace1 != null && attributeOrNamespace2 != null; attributeOrNamespace2 = (XmlDiffAttributeOrNamespace) attributeOrNamespace2._nextSibling)
      {
        if (!attributeOrNamespace1.IsSameAs((XmlDiffNode) attributeOrNamespace2, xmlDiff))
          return false;
        attributeOrNamespace1 = (XmlDiffAttributeOrNamespace) attributeOrNamespace1._nextSibling;
      }
      return attributeOrNamespace1 == null && attributeOrNamespace2 == null;
    }

    internal void InsertAttributeOrNamespace(XmlDiffAttributeOrNamespace newAttrOrNs)
    {
      newAttrOrNs._parent = (XmlDiffParentNode) this;
      XmlDiffAttributeOrNamespace node1 = this._attributes;
      XmlDiffAttributeOrNamespace attributeOrNamespace = (XmlDiffAttributeOrNamespace) null;
      for (; node1 != null && XmlDiffDocument.OrderAttributesOrNamespaces(node1, newAttrOrNs) <= 0; node1 = (XmlDiffAttributeOrNamespace) node1._nextSibling)
        attributeOrNamespace = node1;
      if (attributeOrNamespace == null)
      {
        newAttrOrNs._nextSibling = (XmlDiffNode) this._attributes;
        this._attributes = newAttrOrNs;
      }
      else
      {
        newAttrOrNs._nextSibling = attributeOrNamespace._nextSibling;
        attributeOrNamespace._nextSibling = (XmlDiffNode) newAttrOrNs;
      }
      this._allAttributesHash += newAttrOrNs.HashValue;
      char c;
      if (newAttrOrNs.NodeType == XmlDiffNodeType.Attribute)
      {
        c = newAttrOrNs.LocalName[0];
      }
      else
      {
        XmlDiffNamespace xmlDiffNamespace = (XmlDiffNamespace) newAttrOrNs;
        c = xmlDiffNamespace.Prefix == string.Empty ? 'A' : xmlDiffNamespace.Prefix[0];
      }
      char upper = char.ToUpper(c);
      if (upper >= 'R')
        this._attributesHashRZ += newAttrOrNs.HashValue;
      else if (upper >= 'I')
        this._attributesHashIQ += newAttrOrNs.HashValue;
      else
        this._attributesHashAH += newAttrOrNs.HashValue;
      if (newAttrOrNs.NodeType != XmlDiffNodeType.Namespace)
        return;
      this._bDefinesNamespaces = true;
    }

    internal override void WriteTo(XmlWriter w)
    {
      w.WriteStartElement(this.Prefix, this.LocalName, this.NamespaceURI);
      for (XmlDiffAttributeOrNamespace attributeOrNamespace = this._attributes; attributeOrNamespace != null; attributeOrNamespace = (XmlDiffAttributeOrNamespace) attributeOrNamespace._nextSibling)
        attributeOrNamespace.WriteTo(w);
      this.WriteContentTo(w);
      w.WriteEndElement();
    }

    internal override void WriteContentTo(XmlWriter w)
    {
      for (XmlDiffNode xmlDiffNode = this._firstChildNode; xmlDiffNode != null; xmlDiffNode = xmlDiffNode._nextSibling)
        xmlDiffNode.WriteTo(w);
    }
  }
}
