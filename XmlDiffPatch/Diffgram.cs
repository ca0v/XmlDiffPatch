// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.Diffgram
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class Diffgram : DiffgramParentOperation
  {
    private XmlDiff _xmlDiff;
    private OperationDescriptor _descriptors;

    internal Diffgram(XmlDiff xmlDiff)
      : base(0UL)
    {
      this._xmlDiff = xmlDiff;
    }

    internal void AddDescriptor(OperationDescriptor desc)
    {
      desc._nextDescriptor = this._descriptors;
      this._descriptors = desc;
    }

    internal override void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
    {
      this._xmlDiff = xmlDiff;
      this.WriteTo(xmlWriter);
    }

    internal void WriteTo(XmlWriter xmlWriter)
    {
      xmlWriter.WriteStartDocument();
      xmlWriter.WriteStartElement("xd", "xmldiff", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
      xmlWriter.WriteAttributeString("version", "1.0");
      xmlWriter.WriteAttributeString("srcDocHash", this._xmlDiff._sourceDoc.HashValue.ToString());
      xmlWriter.WriteAttributeString("options", this._xmlDiff.GetXmlDiffOptionsString());
      xmlWriter.WriteAttributeString("fragments", this._xmlDiff._fragments == TriStateBool.Yes ? "yes" : "no");
      this.WriteChildrenTo(xmlWriter, this._xmlDiff);
      for (var operationDescriptor = this._descriptors; operationDescriptor != null; operationDescriptor = operationDescriptor._nextDescriptor)
        operationDescriptor.WriteTo(xmlWriter);
      xmlWriter.WriteEndElement();
      xmlWriter.WriteEndDocument();
    }
  }
}
