// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.PatchChange
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class PatchChange : XmlPatchParentOperation
  {
    private XmlNode _matchNode;
    private string _name;
    private string _ns;
    private string _prefix;
    private string _value;

    internal PatchChange(
      XmlNode matchNode,
      string name,
      string ns,
      string prefix,
      XmlNode diffChangeNode)
    {
      this._matchNode = matchNode;
      this._name = name;
      this._ns = ns;
      this._prefix = prefix;
      if (diffChangeNode == null)
      {
        this._value = (string) null;
      }
      else
      {
        switch (matchNode.NodeType)
        {
          case XmlNodeType.ProcessingInstruction:
            if (name != null)
              break;
            this._name = diffChangeNode.FirstChild.Name;
            this._value = diffChangeNode.FirstChild.Value;
            break;
          case XmlNodeType.Comment:
            this._value = diffChangeNode.FirstChild.Value;
            break;
          default:
            this._value = diffChangeNode.InnerText;
            break;
        }
      }
    }

    internal override void Apply(XmlNode parent, ref XmlNode currentPosition)
    {
      switch (this._matchNode.NodeType)
      {
        case XmlNodeType.Element:
          if (this._name == null)
            this._name = this._matchNode.LocalName;
          if (this._ns == null)
            this._ns = this._matchNode.NamespaceURI;
          if (this._prefix == null)
            this._prefix = this._matchNode.Prefix;
          XmlElement element = parent.OwnerDocument.CreateElement(this._prefix, this._name, this._ns);
          XmlAttributeCollection attributes = this._matchNode.Attributes;
          while (attributes.Count > 0)
          {
            XmlAttribute node = (XmlAttribute) attributes.Item(0);
            attributes.RemoveAt(0);
            element.Attributes.Append(node);
          }
          XmlNode nextSibling;
          for (XmlNode xmlNode = this._matchNode.FirstChild; xmlNode != null; xmlNode = nextSibling)
          {
            nextSibling = xmlNode.NextSibling;
            this._matchNode.RemoveChild(xmlNode);
            element.AppendChild(xmlNode);
          }
          parent.ReplaceChild((XmlNode) element, this._matchNode);
          currentPosition = (XmlNode) element;
          this.ApplyChildren((XmlNode) element);
          break;
        case XmlNodeType.Attribute:
          if (this._name == null)
            this._name = this._matchNode.LocalName;
          if (this._ns == null)
            this._ns = this._matchNode.NamespaceURI;
          if (this._prefix == null)
            this._prefix = this._matchNode.Prefix;
          if (this._value == null)
            this._value = this._matchNode.Value;
          XmlAttribute attribute = parent.OwnerDocument.CreateAttribute(this._prefix, this._name, this._ns);
          attribute.Value = this._value;
          parent.Attributes.Remove((XmlAttribute) this._matchNode);
          parent.Attributes.Append(attribute);
          break;
        case XmlNodeType.Text:
        case XmlNodeType.CDATA:
        case XmlNodeType.Comment:
          ((XmlCharacterData) this._matchNode).Data = this._value;
          currentPosition = this._matchNode;
          break;
        case XmlNodeType.EntityReference:
          XmlEntityReference entityReference = parent.OwnerDocument.CreateEntityReference(this._name);
          parent.ReplaceChild((XmlNode) entityReference, this._matchNode);
          currentPosition = (XmlNode) entityReference;
          break;
        case XmlNodeType.ProcessingInstruction:
          if (this._name != null)
          {
            if (this._value == null)
              this._value = ((XmlProcessingInstruction) this._matchNode).Data;
            XmlProcessingInstruction processingInstruction = parent.OwnerDocument.CreateProcessingInstruction(this._name, this._value);
            parent.ReplaceChild((XmlNode) processingInstruction, this._matchNode);
            currentPosition = (XmlNode) processingInstruction;
            break;
          }
          ((XmlProcessingInstruction) this._matchNode).Data = this._value;
          currentPosition = this._matchNode;
          break;
        case XmlNodeType.DocumentType:
          if (this._name == null)
            this._name = this._matchNode.LocalName;
          if (this._ns == null)
            this._ns = ((XmlDocumentType) this._matchNode).SystemId;
          else if (this._ns == string.Empty)
            this._ns = (string) null;
          if (this._prefix == null)
            this._prefix = ((XmlDocumentType) this._matchNode).PublicId;
          else if (this._prefix == string.Empty)
            this._prefix = (string) null;
          if (this._value == null)
            this._value = ((XmlDocumentType) this._matchNode).InternalSubset;
          this._matchNode.ParentNode.ReplaceChild((XmlNode) this._matchNode.OwnerDocument.CreateDocumentType(this._name, this._prefix, this._ns, this._value), this._matchNode);
          break;
        case XmlNodeType.XmlDeclaration:
          XmlDeclaration matchNode = (XmlDeclaration) this._matchNode;
          matchNode.Encoding = (string) null;
          matchNode.Standalone = (string) null;
          matchNode.InnerText = this._value;
          break;
      }
    }
  }
}
