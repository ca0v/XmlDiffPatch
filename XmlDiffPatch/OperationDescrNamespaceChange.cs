// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.OperationDescrNamespaceChange
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class OperationDescrNamespaceChange : OperationDescriptor
  {
    private DiffgramGenerator.NamespaceChange _nsChange;

    internal OperationDescrNamespaceChange(DiffgramGenerator.NamespaceChange nsChange)
      : base(nsChange._opid)
    {
      this._nsChange = nsChange;
    }

    internal override void WriteTo(XmlWriter xmlWriter)
    {
      xmlWriter.WriteStartElement("xd", "descriptor", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
      xmlWriter.WriteAttributeString("opid", this._opid.ToString());
      xmlWriter.WriteAttributeString("type", OperationDescriptor.Type.NamespaceChange.ToString());
      xmlWriter.WriteAttributeString("prefix", this._nsChange._prefix);
      xmlWriter.WriteAttributeString("oldNs", this._nsChange._oldNS);
      xmlWriter.WriteAttributeString("newNs", this._nsChange._newNS);
      xmlWriter.WriteEndElement();
    }
  }
}
