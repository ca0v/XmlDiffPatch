// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.DiffgramPosition
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class DiffgramPosition : DiffgramParentOperation
  {
    internal XmlDiffNode _sourceNode;

    internal DiffgramPosition(XmlDiffNode sourceNode)
      : base(0UL)
    {
      if (sourceNode is XmlDiffShrankNode)
        sourceNode = ((XmlDiffShrankNode) sourceNode)._lastNode;
      this._sourceNode = sourceNode;
    }

    internal override void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
    {
      xmlWriter.WriteStartElement("xd", "node", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
      xmlWriter.WriteAttributeString("match", this._sourceNode?.GetRelativeAddress());
      this.WriteChildrenTo(xmlWriter, xmlDiff);
      xmlWriter.WriteEndElement();
    }
  }
}
