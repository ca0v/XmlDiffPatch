// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffDocument
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System;
using System.Collections;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffDocument : XmlDiffParentNode
  {
    protected XmlDiff _XmlDiff;
    private bool _bLoaded;
    private XmlDiffNode _curLastChild;
    private XmlHash _xmlHash;

    internal XmlDiffDocument(XmlDiff xmlDiff)
      : base(0)
    {
      this._bLoaded = false;
      this._XmlDiff = xmlDiff;
    }

    internal override XmlDiffNodeType NodeType
    {
      get
      {
        return XmlDiffNodeType.Document;
      }
    }

    internal bool IsFragment
    {
      get
      {
        XmlDiffNode xmlDiffNode = this._firstChildNode;
        while (xmlDiffNode != null && xmlDiffNode.NodeType != XmlDiffNodeType.Element)
          xmlDiffNode = xmlDiffNode._nextSibling;
        if (xmlDiffNode == null)
          return true;
        XmlDiffNode nextSibling = xmlDiffNode._nextSibling;
        while (nextSibling != null && nextSibling.NodeType != XmlDiffNodeType.Element)
          nextSibling = nextSibling._nextSibling;
        return nextSibling != null;
      }
    }

    internal override void ComputeHashValue(XmlHash xmlHash)
    {
      this._hashValue = xmlHash.ComputeHashXmlDiffDocument(this);
    }

    internal override XmlDiffOperation GetDiffOperation(
      XmlDiffNode changedNode,
      XmlDiff xmlDiff)
    {
      return changedNode.NodeType != XmlDiffNodeType.Document ? XmlDiffOperation.Undefined : XmlDiffOperation.Match;
    }

    internal virtual void Load(XmlReader reader, XmlHash xmlHash)
    {
      if (this._bLoaded)
        throw new InvalidOperationException("The document already contains data and should not be used again.");
      try
      {
        this._curLastChild = (XmlDiffNode) null;
        this._xmlHash = xmlHash;
        this.LoadChildNodes((XmlDiffParentNode) this, reader, false);
        this.ComputeHashValue(this._xmlHash);
        this._bLoaded = true;
      }
      finally
      {
        this._xmlHash = (XmlHash) null;
      }
    }

    internal void LoadChildNodes(XmlDiffParentNode parent, XmlReader reader, bool bEmptyElement)
    {
      XmlDiffNode curLastChild = this._curLastChild;
      this._curLastChild = (XmlDiffNode) null;
      while (reader.MoveToNextAttribute())
      {
        if (reader.Prefix == "xmlns")
        {
          if (!this._XmlDiff.IgnoreNamespaces)
          {
            XmlDiffNamespace xmlDiffNamespace = new XmlDiffNamespace(reader.LocalName, reader.Value);
            xmlDiffNamespace.ComputeHashValue(this._xmlHash);
            this.InsertAttributeOrNamespace((XmlDiffElement) parent, (XmlDiffAttributeOrNamespace) xmlDiffNamespace);
          }
        }
        else if (reader.Prefix == string.Empty && reader.LocalName == "xmlns")
        {
          if (!this._XmlDiff.IgnoreNamespaces)
          {
            XmlDiffNamespace xmlDiffNamespace = new XmlDiffNamespace(string.Empty, reader.Value);
            xmlDiffNamespace.ComputeHashValue(this._xmlHash);
            this.InsertAttributeOrNamespace((XmlDiffElement) parent, (XmlDiffAttributeOrNamespace) xmlDiffNamespace);
          }
        }
        else
        {
          string str = this._XmlDiff.IgnoreWhitespace ? XmlDiff.NormalizeText(reader.Value) : reader.Value;
          XmlDiffAttribute xmlDiffAttribute = new XmlDiffAttribute(reader.LocalName, reader.Prefix, reader.NamespaceURI, str);
          xmlDiffAttribute.ComputeHashValue(this._xmlHash);
          this.InsertAttributeOrNamespace((XmlDiffElement) parent, (XmlDiffAttributeOrNamespace) xmlDiffAttribute);
        }
      }
      if (!bEmptyElement)
      {
        int position = 0;
        if (reader.Read())
        {
          do
          {
            if (reader.NodeType != XmlNodeType.Whitespace)
            {
              switch (reader.NodeType)
              {
                case XmlNodeType.Element:
                  bool isEmptyElement = reader.IsEmptyElement;
                  XmlDiffElement xmlDiffElement = new XmlDiffElement(++position, reader.LocalName, reader.Prefix, reader.NamespaceURI);
                  this.LoadChildNodes((XmlDiffParentNode) xmlDiffElement, reader, isEmptyElement);
                  xmlDiffElement.ComputeHashValue(this._xmlHash);
                  this.InsertChild(parent, (XmlDiffNode) xmlDiffElement);
                  break;
                case XmlNodeType.Text:
                  string str = this._XmlDiff.IgnoreWhitespace ? XmlDiff.NormalizeText(reader.Value) : reader.Value;
                  XmlDiffCharData xmlDiffCharData1 = new XmlDiffCharData(++position, str, XmlDiffNodeType.Text);
                  xmlDiffCharData1.ComputeHashValue(this._xmlHash);
                  this.InsertChild(parent, (XmlDiffNode) xmlDiffCharData1);
                  break;
                case XmlNodeType.CDATA:
                  XmlDiffCharData xmlDiffCharData2 = new XmlDiffCharData(++position, reader.Value, XmlDiffNodeType.CDATA);
                  xmlDiffCharData2.ComputeHashValue(this._xmlHash);
                  this.InsertChild(parent, (XmlDiffNode) xmlDiffCharData2);
                  break;
                case XmlNodeType.EntityReference:
                  XmlDiffER xmlDiffEr = new XmlDiffER(++position, reader.Name);
                  xmlDiffEr.ComputeHashValue(this._xmlHash);
                  this.InsertChild(parent, (XmlDiffNode) xmlDiffEr);
                  break;
                case XmlNodeType.ProcessingInstruction:
                  ++position;
                  if (!this._XmlDiff.IgnorePI)
                  {
                    XmlDiffPI xmlDiffPi = new XmlDiffPI(position, reader.Name, reader.Value);
                    xmlDiffPi.ComputeHashValue(this._xmlHash);
                    this.InsertChild(parent, (XmlDiffNode) xmlDiffPi);
                    break;
                  }
                  break;
                case XmlNodeType.Comment:
                  ++position;
                  if (!this._XmlDiff.IgnoreComments)
                  {
                    XmlDiffCharData xmlDiffCharData3 = new XmlDiffCharData(position, reader.Value, XmlDiffNodeType.Comment);
                    xmlDiffCharData3.ComputeHashValue(this._xmlHash);
                    this.InsertChild(parent, (XmlDiffNode) xmlDiffCharData3);
                    break;
                  }
                  break;
                case XmlNodeType.DocumentType:
                  ++position;
                  if (!this._XmlDiff.IgnoreDtd)
                  {
                    XmlDiffDocumentType diffDocumentType = new XmlDiffDocumentType(position, reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                    diffDocumentType.ComputeHashValue(this._xmlHash);
                    this.InsertChild(parent, (XmlDiffNode) diffDocumentType);
                    break;
                  }
                  break;
                case XmlNodeType.SignificantWhitespace:
                  if (reader.XmlSpace == XmlSpace.Preserve)
                  {
                    ++position;
                    if (!this._XmlDiff.IgnoreWhitespace)
                    {
                      XmlDiffCharData xmlDiffCharData3 = new XmlDiffCharData(position, reader.Value, XmlDiffNodeType.SignificantWhitespace);
                      xmlDiffCharData3.ComputeHashValue(this._xmlHash);
                      this.InsertChild(parent, (XmlDiffNode) xmlDiffCharData3);
                      break;
                    }
                    break;
                  }
                  break;
                case XmlNodeType.EndElement:
                  goto label_29;
                case XmlNodeType.XmlDeclaration:
                  ++position;
                  if (!this._XmlDiff.IgnoreXmlDecl)
                  {
                    XmlDiffXmlDeclaration diffXmlDeclaration = new XmlDiffXmlDeclaration(position, XmlDiff.NormalizeXmlDeclaration(reader.Value));
                    diffXmlDeclaration.ComputeHashValue(this._xmlHash);
                    this.InsertChild(parent, (XmlDiffNode) diffXmlDeclaration);
                    break;
                  }
                  break;
              }
            }
          }
          while (reader.Read());
        }
      }
label_29:
      this._curLastChild = curLastChild;
    }

    internal virtual void Load(XmlNode node, XmlHash xmlHash)
    {
      if (this._bLoaded)
        throw new InvalidOperationException("The document already contains data and should not be used again.");
      if (node.NodeType != XmlNodeType.Attribute && node.NodeType != XmlNodeType.Entity && node.NodeType != XmlNodeType.Notation)
      {
        if (node.NodeType != XmlNodeType.Whitespace)
        {
          try
          {
            this._curLastChild = (XmlDiffNode) null;
            this._xmlHash = xmlHash;
            if (node.NodeType == XmlNodeType.Document || node.NodeType == XmlNodeType.DocumentFragment)
            {
              this.LoadChildNodes((XmlDiffParentNode) this, node);
              this.ComputeHashValue(this._xmlHash);
            }
            else
            {
              int childPosition = 0;
              XmlDiffNode newChildNode = this.LoadNode(node, ref childPosition);
              if (newChildNode != null)
              {
                this.InsertChildNodeAfter((XmlDiffNode) null, newChildNode);
                this._hashValue = newChildNode.HashValue;
              }
            }
            this._bLoaded = true;
            return;
          }
          finally
          {
            this._xmlHash = (XmlHash) null;
          }
        }
      }
      throw new ArgumentException("Invalid node type.");
    }

    internal XmlDiffNode LoadNode(XmlNode node, ref int childPosition)
    {
      switch (node.NodeType)
      {
        case XmlNodeType.Element:
          XmlDiffElement xmlDiffElement = new XmlDiffElement(++childPosition, node.LocalName, node.Prefix, node.NamespaceURI);
          this.LoadChildNodes((XmlDiffParentNode) xmlDiffElement, node);
          xmlDiffElement.ComputeHashValue(this._xmlHash);
          return (XmlDiffNode) xmlDiffElement;
        case XmlNodeType.Attribute:
          return (XmlDiffNode) null;
        case XmlNodeType.Text:
          string str = this._XmlDiff.IgnoreWhitespace ? XmlDiff.NormalizeText(node.Value) : node.Value;
          XmlDiffCharData xmlDiffCharData1 = new XmlDiffCharData(++childPosition, str, XmlDiffNodeType.Text);
          xmlDiffCharData1.ComputeHashValue(this._xmlHash);
          return (XmlDiffNode) xmlDiffCharData1;
        case XmlNodeType.CDATA:
          XmlDiffCharData xmlDiffCharData2 = new XmlDiffCharData(++childPosition, node.Value, XmlDiffNodeType.CDATA);
          xmlDiffCharData2.ComputeHashValue(this._xmlHash);
          return (XmlDiffNode) xmlDiffCharData2;
        case XmlNodeType.EntityReference:
          XmlDiffER xmlDiffEr = new XmlDiffER(++childPosition, node.Name);
          xmlDiffEr.ComputeHashValue(this._xmlHash);
          return (XmlDiffNode) xmlDiffEr;
        case XmlNodeType.ProcessingInstruction:
          ++childPosition;
          if (this._XmlDiff.IgnorePI)
            return (XmlDiffNode) null;
          XmlDiffPI xmlDiffPi = new XmlDiffPI(childPosition, node.Name, node.Value);
          xmlDiffPi.ComputeHashValue(this._xmlHash);
          return (XmlDiffNode) xmlDiffPi;
        case XmlNodeType.Comment:
          ++childPosition;
          if (this._XmlDiff.IgnoreComments)
            return (XmlDiffNode) null;
          XmlDiffCharData xmlDiffCharData3 = new XmlDiffCharData(childPosition, node.Value, XmlDiffNodeType.Comment);
          xmlDiffCharData3.ComputeHashValue(this._xmlHash);
          return (XmlDiffNode) xmlDiffCharData3;
        case XmlNodeType.DocumentType:
          ++childPosition;
          if (this._XmlDiff.IgnoreDtd)
            return (XmlDiffNode) null;
          XmlDocumentType xmlDocumentType = (XmlDocumentType) node;
          XmlDiffDocumentType diffDocumentType = new XmlDiffDocumentType(childPosition, xmlDocumentType.Name, xmlDocumentType.PublicId, xmlDocumentType.SystemId, xmlDocumentType.InternalSubset);
          diffDocumentType.ComputeHashValue(this._xmlHash);
          return (XmlDiffNode) diffDocumentType;
        case XmlNodeType.SignificantWhitespace:
          ++childPosition;
          if (this._XmlDiff.IgnoreWhitespace)
            return (XmlDiffNode) null;
          XmlDiffCharData xmlDiffCharData4 = new XmlDiffCharData(childPosition, node.Value, XmlDiffNodeType.SignificantWhitespace);
          xmlDiffCharData4.ComputeHashValue(this._xmlHash);
          return (XmlDiffNode) xmlDiffCharData4;
        case XmlNodeType.EndElement:
          return (XmlDiffNode) null;
        case XmlNodeType.XmlDeclaration:
          ++childPosition;
          if (this._XmlDiff.IgnoreXmlDecl)
            return (XmlDiffNode) null;
          XmlDiffXmlDeclaration diffXmlDeclaration = new XmlDiffXmlDeclaration(childPosition, XmlDiff.NormalizeXmlDeclaration(node.Value));
          diffXmlDeclaration.ComputeHashValue(this._xmlHash);
          return (XmlDiffNode) diffXmlDeclaration;
        default:
          return (XmlDiffNode) null;
      }
    }

    internal void LoadChildNodes(XmlDiffParentNode parent, XmlNode parentDomNode)
    {
      XmlDiffNode curLastChild = this._curLastChild;
      this._curLastChild = (XmlDiffNode) null;
      XmlNamedNodeMap attributes = (XmlNamedNodeMap) parentDomNode.Attributes;
      if (attributes != null && attributes.Count > 0)
      {
        foreach (XmlAttribute xmlAttribute in attributes)
        {
          if (xmlAttribute.Prefix == "xmlns")
          {
            if (!this._XmlDiff.IgnoreNamespaces)
            {
              XmlDiffNamespace xmlDiffNamespace = new XmlDiffNamespace(xmlAttribute.LocalName, xmlAttribute.Value);
              xmlDiffNamespace.ComputeHashValue(this._xmlHash);
              this.InsertAttributeOrNamespace((XmlDiffElement) parent, (XmlDiffAttributeOrNamespace) xmlDiffNamespace);
            }
          }
          else if (xmlAttribute.Prefix == string.Empty && xmlAttribute.LocalName == "xmlns")
          {
            if (!this._XmlDiff.IgnoreNamespaces)
            {
              XmlDiffNamespace xmlDiffNamespace = new XmlDiffNamespace(string.Empty, xmlAttribute.Value);
              xmlDiffNamespace.ComputeHashValue(this._xmlHash);
              this.InsertAttributeOrNamespace((XmlDiffElement) parent, (XmlDiffAttributeOrNamespace) xmlDiffNamespace);
            }
          }
          else
          {
            string str = this._XmlDiff.IgnoreWhitespace ? XmlDiff.NormalizeText(xmlAttribute.Value) : xmlAttribute.Value;
            XmlDiffAttribute xmlDiffAttribute = new XmlDiffAttribute(xmlAttribute.LocalName, xmlAttribute.Prefix, xmlAttribute.NamespaceURI, str);
            xmlDiffAttribute.ComputeHashValue(this._xmlHash);
            this.InsertAttributeOrNamespace((XmlDiffElement) parent, (XmlDiffAttributeOrNamespace) xmlDiffAttribute);
          }
        }
      }
      XmlNodeList childNodes = parentDomNode.ChildNodes;
      if (childNodes.Count != 0)
      {
        int childPosition = 0;
        IEnumerator enumerator = childNodes.GetEnumerator();
        while (enumerator.MoveNext())
        {
          if (((XmlNode) enumerator.Current).NodeType != XmlNodeType.Whitespace)
          {
            XmlDiffNode newChild = this.LoadNode((XmlNode) enumerator.Current, ref childPosition);
            if (newChild != null)
              this.InsertChild(parent, newChild);
          }
        }
      }
      this._curLastChild = curLastChild;
    }

    private void InsertChild(XmlDiffParentNode parent, XmlDiffNode newChild)
    {
      if (this._XmlDiff.IgnoreChildOrder)
      {
        XmlDiffNode node1 = parent.FirstChildNode;
        XmlDiffNode childNode = (XmlDiffNode) null;
        for (; node1 != null && XmlDiffDocument.OrderChildren(node1, newChild) <= 0; node1 = node1._nextSibling)
          childNode = node1;
        parent.InsertChildNodeAfter(childNode, newChild);
      }
      else
      {
        parent.InsertChildNodeAfter(this._curLastChild, newChild);
        this._curLastChild = newChild;
      }
    }

    private void InsertAttributeOrNamespace(
      XmlDiffElement element,
      XmlDiffAttributeOrNamespace newAttrOrNs)
    {
      element.InsertAttributeOrNamespace(newAttrOrNs);
    }

    internal static int OrderChildren(XmlDiffNode node1, XmlDiffNode node2)
    {
      int nodeType1 = (int) node1.NodeType;
      int nodeType2 = (int) node2.NodeType;
      if (nodeType1 < nodeType2)
        return -1;
      if (nodeType2 < nodeType1)
        return 1;
      switch (nodeType1)
      {
        case 1:
          return XmlDiffDocument.OrderElements(node1 as XmlDiffElement, node2 as XmlDiffElement);
        case 2:
        case 100:
          return 0;
        case 5:
          return XmlDiffDocument.OrderERs(node1 as XmlDiffER, node2 as XmlDiffER);
        case 7:
          return XmlDiffDocument.OrderPIs(node1 as XmlDiffPI, node2 as XmlDiffPI);
        case 101:
          if (((XmlDiffShrankNode) node1).MatchingShrankNode == ((XmlDiffShrankNode) node2).MatchingShrankNode)
            return 0;
          return node1.HashValue >= node2.HashValue ? 1 : -1;
        default:
          return XmlDiffDocument.OrderCharacterData(node1 as XmlDiffCharData, node2 as XmlDiffCharData);
      }
    }

    internal static int OrderElements(XmlDiffElement elem1, XmlDiffElement elem2)
    {
      int num;
      return (num = XmlDiffDocument.OrderStrings(elem1.LocalName, elem2.LocalName)) == 0 && (num = XmlDiffDocument.OrderStrings(elem1.NamespaceURI, elem2.NamespaceURI)) == 0 ? XmlDiffDocument.OrderSubTrees(elem1, elem2) : num;
    }

    internal static int OrderAttributesOrNamespaces(
      XmlDiffAttributeOrNamespace node1,
      XmlDiffAttributeOrNamespace node2)
    {
      int num;
      return node1.NodeType != node2.NodeType ? (node1.NodeType == XmlDiffNodeType.Namespace ? -1 : 1) : ((num = XmlDiffDocument.OrderStrings(node1.LocalName, node2.LocalName)) == 0 && (num = XmlDiffDocument.OrderStrings(node1.Prefix, node2.Prefix)) == 0 && ((num = XmlDiffDocument.OrderStrings(node1.NamespaceURI, node2.NamespaceURI)) == 0 && (num = XmlDiffDocument.OrderStrings(node1.Value, node2.Value)) == 0) ? 0 : num);
    }

    internal static int OrderERs(XmlDiffER er1, XmlDiffER er2)
    {
      return XmlDiffDocument.OrderStrings(er1.Name, er2.Name);
    }

    internal static int OrderPIs(XmlDiffPI pi1, XmlDiffPI pi2)
    {
      int num;
      return (num = XmlDiffDocument.OrderStrings(pi1.Name, pi2.Name)) == 0 && (num = XmlDiffDocument.OrderStrings(pi1.Value, pi2.Value)) == 0 ? 0 : num;
    }

    internal static int OrderCharacterData(XmlDiffCharData t1, XmlDiffCharData t2)
    {
      return XmlDiffDocument.OrderStrings(t1.Value, t2.Value);
    }

    internal static int OrderStrings(string s1, string s2)
    {
      int num = s1.Length < s2.Length ? s1.Length : s2.Length;
      int index = 0;
      while (index < num && (int) s1[index] == (int) s2[index])
        ++index;
      if (index < num)
        return (int) s1[index] >= (int) s2[index] ? 1 : -1;
      if (s1.Length == s2.Length)
        return 0;
      return s2.Length <= s1.Length ? 1 : -1;
    }

    internal static int OrderSubTrees(XmlDiffElement elem1, XmlDiffElement elem2)
    {
      XmlDiffAttributeOrNamespace node1_1 = elem1._attributes;
      XmlDiffAttributeOrNamespace node2_1 = elem2._attributes;
      while (node1_1 != null && node1_1.NodeType == XmlDiffNodeType.Namespace)
        node1_1 = (XmlDiffAttributeOrNamespace) node1_1._nextSibling;
      while (node2_1 != null && node2_1.NodeType == XmlDiffNodeType.Namespace)
        node2_1 = (XmlDiffAttributeOrNamespace) node2_1._nextSibling;
      for (; node1_1 != null && node2_1 != null; node2_1 = (XmlDiffAttributeOrNamespace) node2_1._nextSibling)
      {
        int num;
        if ((num = XmlDiffDocument.OrderAttributesOrNamespaces(node1_1, node2_1)) != 0)
          return num;
        node1_1 = (XmlDiffAttributeOrNamespace) node1_1._nextSibling;
      }
      if (node1_1 == node2_1)
      {
        XmlDiffNode node1_2 = elem1.FirstChildNode;
        XmlDiffNode node2_2;
        for (node2_2 = elem2.FirstChildNode; node1_2 != null && node2_2 != null; node2_2 = node2_2._nextSibling)
        {
          int num;
          if ((num = XmlDiffDocument.OrderChildren(node1_2, node2_2)) != 0)
            return num;
          node1_2 = node1_2._nextSibling;
        }
        if (node1_2 == node2_2)
          return 0;
        return node1_2 == null ? 1 : -1;
      }
      return node1_1 == null ? 1 : -1;
    }

    internal override void WriteTo(XmlWriter w)
    {
      this.WriteContentTo(w);
    }

    internal override void WriteContentTo(XmlWriter w)
    {
      for (XmlDiffNode xmlDiffNode = this.FirstChildNode; xmlDiffNode != null; xmlDiffNode = xmlDiffNode._nextSibling)
        xmlDiffNode.WriteTo(w);
    }
  }
}
