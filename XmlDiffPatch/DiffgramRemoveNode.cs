﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.DiffgramRemoveNode
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class DiffgramRemoveNode : DiffgramParentOperation
  {
    private XmlDiffNode _sourceNode;
    private bool _bSubtree;

    internal DiffgramRemoveNode(XmlDiffNode sourceNode, bool bSubtree, ulong operationID)
      : base(operationID)
    {
      this._sourceNode = sourceNode;
      this._bSubtree = bSubtree;
    }

    internal override void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
    {
      xmlWriter.WriteStartElement("xd", "remove", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
      xmlWriter.WriteAttributeString("match", this._sourceNode.GetRelativeAddress());
      if (!this._bSubtree)
        xmlWriter.WriteAttributeString("subtree", "no");
      if (this._operationID != 0UL)
        xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
      this.WriteChildrenTo(xmlWriter, xmlDiff);
      xmlWriter.WriteEndElement();
    }
  }
}
