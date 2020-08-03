// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.DiffgramAddNode
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class DiffgramAddNode : DiffgramParentOperation
  {
    private XmlDiffNode _targetNode;

    internal DiffgramAddNode(XmlDiffNode targetNode, ulong operationID)
      : base(operationID)
    {
      this._targetNode = targetNode;
    }

    internal override void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
    {
      xmlWriter.WriteStartElement("xd", "add", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
      switch (this._targetNode.NodeType)
      {
        case XmlDiffNodeType.XmlDeclaration:
          xmlWriter.WriteAttributeString("type", 17.ToString());
          if (this._operationID != 0UL)
            xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
          xmlWriter.WriteString(((XmlDiffXmlDeclaration) this._targetNode).Value);
          break;
        case XmlDiffNodeType.DocumentType:
          xmlWriter.WriteAttributeString("type", 10.ToString());
          XmlDiffDocumentType targetNode1 = (XmlDiffDocumentType) this._targetNode;
          if (this._operationID != 0UL)
            xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
          xmlWriter.WriteAttributeString("name", targetNode1.Name);
          if (targetNode1.PublicId != string.Empty)
            xmlWriter.WriteAttributeString("publicId", targetNode1.PublicId);
          if (targetNode1.SystemId != string.Empty)
            xmlWriter.WriteAttributeString("systemId", targetNode1.SystemId);
          if (targetNode1.Subset != string.Empty)
          {
            xmlWriter.WriteCData(targetNode1.Subset);
            break;
          }
          break;
        case XmlDiffNodeType.Element:
          xmlWriter.WriteAttributeString("type", 1.ToString());
          XmlDiffElement targetNode2 = this._targetNode as XmlDiffElement;
          xmlWriter.WriteAttributeString("name", targetNode2.LocalName);
          if (targetNode2.NamespaceURI != string.Empty)
            xmlWriter.WriteAttributeString("ns", targetNode2.NamespaceURI);
          if (targetNode2.Prefix != string.Empty)
            xmlWriter.WriteAttributeString("prefix", targetNode2.Prefix);
          if (this._operationID != 0UL)
            xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
          this.WriteChildrenTo(xmlWriter, xmlDiff);
          break;
        case XmlDiffNodeType.Attribute:
          xmlWriter.WriteAttributeString("type", 2.ToString());
          XmlDiffAttribute targetNode3 = this._targetNode as XmlDiffAttribute;
          xmlWriter.WriteAttributeString("name", targetNode3.LocalName);
          if (targetNode3.NamespaceURI != string.Empty)
            xmlWriter.WriteAttributeString("ns", targetNode3.NamespaceURI);
          if (targetNode3.Prefix != string.Empty)
            xmlWriter.WriteAttributeString("prefix", targetNode3.Prefix);
          if (this._operationID != 0UL)
            xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
          xmlWriter.WriteString(targetNode3.Value);
          break;
        case XmlDiffNodeType.Text:
          xmlWriter.WriteAttributeString("type", ((int) this._targetNode.NodeType).ToString());
          if (this._operationID != 0UL)
            xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
          xmlWriter.WriteString((this._targetNode as XmlDiffCharData).Value);
          break;
        case XmlDiffNodeType.CDATA:
          xmlWriter.WriteAttributeString("type", ((int) this._targetNode.NodeType).ToString());
          if (this._operationID != 0UL)
            xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
          xmlWriter.WriteCData((this._targetNode as XmlDiffCharData).Value);
          break;
        case XmlDiffNodeType.EntityReference:
          xmlWriter.WriteAttributeString("type", 5.ToString());
          if (this._operationID != 0UL)
            xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
          xmlWriter.WriteAttributeString("name", ((XmlDiffER) this._targetNode).Name);
          break;
        case XmlDiffNodeType.ProcessingInstruction:
          xmlWriter.WriteAttributeString("type", ((int) this._targetNode.NodeType).ToString());
          if (this._operationID != 0UL)
            xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
          XmlDiffPI targetNode4 = this._targetNode as XmlDiffPI;
          xmlWriter.WriteProcessingInstruction(targetNode4.Name, targetNode4.Value);
          break;
        case XmlDiffNodeType.Comment:
          xmlWriter.WriteAttributeString("type", ((int) this._targetNode.NodeType).ToString());
          if (this._operationID != 0UL)
            xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
          xmlWriter.WriteComment((this._targetNode as XmlDiffCharData).Value);
          break;
        case XmlDiffNodeType.SignificantWhitespace:
          xmlWriter.WriteAttributeString("type", ((int) this._targetNode.NodeType).ToString());
          if (this._operationID != 0UL)
            xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
          xmlWriter.WriteString(((XmlDiffCharData) this._targetNode).Value);
          break;
        case XmlDiffNodeType.Namespace:
          xmlWriter.WriteAttributeString("type", 2.ToString());
          XmlDiffNamespace targetNode5 = this._targetNode as XmlDiffNamespace;
          if (targetNode5.Prefix != string.Empty)
          {
            xmlWriter.WriteAttributeString("prefix", "xmlns");
            xmlWriter.WriteAttributeString("name", targetNode5.Prefix);
          }
          else
            xmlWriter.WriteAttributeString("name", "xmlns");
          if (this._operationID != 0UL)
            xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
          xmlWriter.WriteString(targetNode5.NamespaceURI);
          break;
      }
      xmlWriter.WriteEndElement();
    }
  }
}
