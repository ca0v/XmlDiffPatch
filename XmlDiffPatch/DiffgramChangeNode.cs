// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.DiffgramChangeNode
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class DiffgramChangeNode : DiffgramParentOperation
  {
    private XmlDiffNode _sourceNode;
    private XmlDiffNode _targetNode;
    private XmlDiffOperation _op;

    internal DiffgramChangeNode(
      XmlDiffNode sourceNode,
      XmlDiffNode targetNode,
      XmlDiffOperation op,
      ulong operationID)
      : base(operationID)
    {
      this._sourceNode = sourceNode;
      this._targetNode = targetNode;
      this._op = op;
    }

    internal override void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
    {
      xmlWriter.WriteStartElement("xd", "change", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
      xmlWriter.WriteAttributeString("match", this._sourceNode.GetRelativeAddress());
      if (this._operationID != 0UL)
        xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
      switch (this._op)
      {
        case XmlDiffOperation.ChangeElementName:
                    var sourceNode1 = (XmlDiffElement) this._sourceNode;
                    var targetNode1 = (XmlDiffElement) this._targetNode;
          if (sourceNode1.LocalName != targetNode1.LocalName)
            xmlWriter.WriteAttributeString("name", targetNode1.LocalName);
          if (sourceNode1.Prefix != targetNode1.Prefix && !xmlDiff.IgnorePrefixes && !xmlDiff.IgnoreNamespaces)
            xmlWriter.WriteAttributeString("prefix", targetNode1.Prefix);
          if (sourceNode1.NamespaceURI != targetNode1.NamespaceURI && !xmlDiff.IgnoreNamespaces)
            xmlWriter.WriteAttributeString("ns", targetNode1.NamespaceURI);
          this.WriteChildrenTo(xmlWriter, xmlDiff);
          break;
        case XmlDiffOperation.ChangePI:
                    var sourceNode2 = (XmlDiffPI) this._sourceNode;
                    var targetNode2 = (XmlDiffPI) this._targetNode;
          if (sourceNode2.Value == targetNode2.Value)
          {
            xmlWriter.WriteAttributeString("name", targetNode2.Name);
            break;
          }
          xmlWriter.WriteProcessingInstruction(targetNode2.Name, targetNode2.Value);
          break;
        case XmlDiffOperation.ChangeER:
          xmlWriter.WriteAttributeString("name", ((XmlDiffER) this._targetNode).Name);
          break;
        case XmlDiffOperation.ChangeCharacterData:
                    var targetNode3 = (XmlDiffCharData) this._targetNode;
          switch (this._targetNode.NodeType)
          {
            case XmlDiffNodeType.Text:
            case XmlDiffNodeType.SignificantWhitespace:
              xmlWriter.WriteString(targetNode3.Value);
              break;
            case XmlDiffNodeType.CDATA:
              xmlWriter.WriteCData(targetNode3.Value);
              break;
            case XmlDiffNodeType.Comment:
              xmlWriter.WriteComment(targetNode3.Value);
              break;
          }
          break;
        case XmlDiffOperation.ChangeXmlDeclaration:
          xmlWriter.WriteString(((XmlDiffXmlDeclaration) this._targetNode).Value);
          break;
        case XmlDiffOperation.ChangeDTD:
                    var sourceNode3 = (XmlDiffDocumentType) this._sourceNode;
                    var targetNode4 = (XmlDiffDocumentType) this._targetNode;
          if (sourceNode3.Name != targetNode4.Name)
            xmlWriter.WriteAttributeString("name", targetNode4.Name);
          if (sourceNode3.SystemId != targetNode4.SystemId)
            xmlWriter.WriteAttributeString("systemId", targetNode4.SystemId);
          if (sourceNode3.PublicId != targetNode4.PublicId)
            xmlWriter.WriteAttributeString("publicId", targetNode4.PublicId);
          if (sourceNode3.Subset != targetNode4.Subset)
          {
            xmlWriter.WriteCData(targetNode4.Subset);
            break;
          }
          break;
        case XmlDiffOperation.ChangeAttr:
                    var sourceNode4 = (XmlDiffAttribute) this._sourceNode;
                    var targetNode5 = (XmlDiffAttribute) this._targetNode;
          if (sourceNode4.Prefix != targetNode5.Prefix && !xmlDiff.IgnorePrefixes && !xmlDiff.IgnoreNamespaces)
            xmlWriter.WriteAttributeString("prefix", targetNode5.Prefix);
          if (sourceNode4.NamespaceURI != targetNode5.NamespaceURI && !xmlDiff.IgnoreNamespaces)
            xmlWriter.WriteAttributeString("ns", targetNode5.NamespaceURI);
          xmlWriter.WriteString(targetNode5.Value);
          break;
      }
      xmlWriter.WriteEndElement();
    }
  }
}
