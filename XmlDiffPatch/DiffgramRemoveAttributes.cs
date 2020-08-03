// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.DiffgramRemoveAttributes
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class DiffgramRemoveAttributes : DiffgramOperation
  {
    private AttributeInterval _attributes;

    internal DiffgramRemoveAttributes(XmlDiffAttribute sourceAttr)
      : base(0UL)
    {
      this._attributes = new AttributeInterval(sourceAttr, (AttributeInterval) null);
    }

    internal override void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
    {
      xmlWriter.WriteStartElement("xd", "remove", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
      DiffgramOperation.GetAddressOfAttributeInterval(this._attributes, xmlWriter);
      xmlWriter.WriteEndElement();
    }

    internal bool AddAttribute(XmlDiffAttribute srcAttr)
    {
      if (this._operationID != 0UL || srcAttr._parent != this._attributes._firstAttr._parent)
        return false;
      if (srcAttr._nextSibling == this._attributes._firstAttr)
        this._attributes._firstAttr = srcAttr;
      else
        this._attributes = new AttributeInterval(srcAttr, this._attributes);
      return true;
    }
  }
}
