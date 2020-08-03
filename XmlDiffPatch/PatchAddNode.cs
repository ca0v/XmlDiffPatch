// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.PatchAddNode
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class PatchAddNode : XmlPatchParentOperation
  {
    private XmlNodeType _nodeType;
    private string _name;
    private string _ns;
    private string _prefix;
    private string _value;
    private bool _ignoreChildOrder;

    internal PatchAddNode(
      XmlNodeType nodeType,
      string name,
      string ns,
      string prefix,
      string value,
      bool ignoreChildOrder)
    {
      this._nodeType = nodeType;
      this._name = name;
      this._ns = ns;
      this._prefix = prefix;
      this._value = value;
      this._ignoreChildOrder = ignoreChildOrder;
    }

    internal override void Apply(XmlNode parent, ref XmlNode currentPosition)
    {
      XmlNode xmlNode1 = (XmlNode) null;
      if (this._nodeType == XmlNodeType.Attribute)
      {
        XmlNode xmlNode2 = !(this._prefix == "xmlns") ? (!(this._prefix == "") || !(this._name == "xmlns") ? (XmlNode) parent.OwnerDocument.CreateAttribute(this._prefix, this._name, this._ns) : (XmlNode) parent.OwnerDocument.CreateAttribute(this._name)) : (XmlNode) parent.OwnerDocument.CreateAttribute(this._prefix + ":" + this._name);
        xmlNode2.Value = this._value;
        parent.Attributes.Append((XmlAttribute) xmlNode2);
      }
      else
      {
        switch (this._nodeType)
        {
          case XmlNodeType.Element:
            xmlNode1 = (XmlNode) parent.OwnerDocument.CreateElement(this._prefix, this._name, this._ns);
            this.ApplyChildren(xmlNode1);
            break;
          case XmlNodeType.Text:
            xmlNode1 = (XmlNode) parent.OwnerDocument.CreateTextNode(this._value);
            break;
          case XmlNodeType.CDATA:
            xmlNode1 = (XmlNode) parent.OwnerDocument.CreateCDataSection(this._value);
            break;
          case XmlNodeType.EntityReference:
            xmlNode1 = (XmlNode) parent.OwnerDocument.CreateEntityReference(this._name);
            break;
          case XmlNodeType.ProcessingInstruction:
            xmlNode1 = (XmlNode) parent.OwnerDocument.CreateProcessingInstruction(this._name, this._value);
            break;
          case XmlNodeType.Comment:
            xmlNode1 = (XmlNode) parent.OwnerDocument.CreateComment(this._value);
            break;
          case XmlNodeType.DocumentType:
            XmlDocument ownerDocument1 = parent.OwnerDocument;
            if (this._prefix == string.Empty)
              this._prefix = (string) null;
            if (this._ns == string.Empty)
              this._ns = (string) null;
            XmlDocumentType documentType = ownerDocument1.CreateDocumentType(this._name, this._prefix, this._ns, this._value);
            if (ownerDocument1.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
            {
              ownerDocument1.InsertAfter((XmlNode) documentType, ownerDocument1.FirstChild);
              return;
            }
            ownerDocument1.InsertBefore((XmlNode) documentType, ownerDocument1.FirstChild);
            return;
          case XmlNodeType.XmlDeclaration:
            XmlDocument ownerDocument2 = parent.OwnerDocument;
            XmlDeclaration xmlDeclaration = ownerDocument2.CreateXmlDeclaration("1.0", string.Empty, string.Empty);
            xmlDeclaration.Value = this._value;
            ownerDocument2.InsertBefore((XmlNode) xmlDeclaration, ownerDocument2.FirstChild);
            return;
        }
        if (this._ignoreChildOrder)
          parent.AppendChild(xmlNode1);
        else
          parent.InsertAfter(xmlNode1, currentPosition);
        currentPosition = xmlNode1;
      }
    }
  }
}
