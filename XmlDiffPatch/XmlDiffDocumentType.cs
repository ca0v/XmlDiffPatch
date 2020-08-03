// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffDocumentType
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffDocumentType : XmlDiffNode
  {
    private string _name;
    private string _publicId;
    private string _systemId;
    private string _subset;

    internal XmlDiffDocumentType(
      int position,
      string name,
      string publicId,
      string systemId,
      string subset)
      : base(position)
    {
      this._name = name;
      this._publicId = publicId;
      this._systemId = systemId;
      this._subset = subset;
    }

    internal override XmlDiffNodeType NodeType
    {
      get
      {
        return XmlDiffNodeType.DocumentType;
      }
    }

    internal string Name
    {
      get
      {
        return this._name;
      }
    }

    internal string PublicId
    {
      get
      {
        return this._publicId;
      }
    }

    internal string SystemId
    {
      get
      {
        return this._systemId;
      }
    }

    internal string Subset
    {
      get
      {
        return this._subset;
      }
    }

    internal override void ComputeHashValue(XmlHash xmlHash)
    {
      this._hashValue = xmlHash.HashDocumentType(this._name, this._publicId, this._systemId, this._subset);
    }

    internal override XmlDiffOperation GetDiffOperation(
      XmlDiffNode changedNode,
      XmlDiff xmlDiff)
    {
      if (changedNode.NodeType != XmlDiffNodeType.DocumentType)
        return XmlDiffOperation.Undefined;
            var diffDocumentType = (XmlDiffDocumentType) changedNode;
      return this.Name == diffDocumentType.Name && this.PublicId == diffDocumentType.PublicId && (this.SystemId == diffDocumentType.SystemId && this.Subset == diffDocumentType.Subset) ? XmlDiffOperation.Match : XmlDiffOperation.ChangeDTD;
    }

    internal override void WriteTo(XmlWriter w)
    {
      w.WriteDocType(this._name, string.Empty, string.Empty, this._subset);
    }

    internal override void WriteContentTo(XmlWriter w)
    {
    }
  }
}
