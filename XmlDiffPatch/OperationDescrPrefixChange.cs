// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.OperationDescrPrefixChange
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class OperationDescrPrefixChange : OperationDescriptor
  {
    private DiffgramGenerator.PrefixChange _prefixChange;

    internal OperationDescrPrefixChange(DiffgramGenerator.PrefixChange prefixChange)
      : base(prefixChange._opid)
    {
      this._prefixChange = prefixChange;
    }

    internal override string Type
    {
      get
      {
        return "prefix change";
      }
    }

    internal override void WriteTo(XmlWriter xmlWriter)
    {
      xmlWriter.WriteStartElement("xd", "descriptor", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
      xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
      xmlWriter.WriteAttributeString("type", this.Type);
      xmlWriter.WriteAttributeString("ns", this._prefixChange._NS);
      xmlWriter.WriteAttributeString("oldPrefix", this._prefixChange._oldPrefix);
      xmlWriter.WriteAttributeString("newPrefix", this._prefixChange._newPrefix);
      xmlWriter.WriteEndElement();
    }
  }
}
