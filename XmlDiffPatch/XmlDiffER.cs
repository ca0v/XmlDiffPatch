// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffER
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffER : XmlDiffNode
  {
    private string _name;

    internal XmlDiffER(int position, string name)
      : base(position)
    {
      this._name = name;
    }

    internal override XmlDiffNodeType NodeType
    {
      get
      {
        return XmlDiffNodeType.EntityReference;
      }
    }

    internal string Name
    {
      get
      {
        return this._name;
      }
    }

    internal override bool CanMerge
    {
      get
      {
        return false;
      }
    }

    internal override void ComputeHashValue(XmlHash xmlHash)
    {
      this._hashValue = xmlHash.HashER(this._name);
    }

    internal override XmlDiffOperation GetDiffOperation(
      XmlDiffNode changedNode,
      XmlDiff xmlDiff)
    {
      if (changedNode.NodeType != XmlDiffNodeType.EntityReference)
        return XmlDiffOperation.Undefined;
      return this.Name == ((XmlDiffER) changedNode).Name ? XmlDiffOperation.Match : XmlDiffOperation.ChangeER;
    }

    internal override void WriteTo(XmlWriter w)
    {
      w.WriteEntityRef(this._name);
    }

    internal override void WriteContentTo(XmlWriter w)
    {
    }
  }
}
