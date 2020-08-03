// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffPI
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffPI : XmlDiffCharData
  {
    private string _name;

    internal XmlDiffPI(int position, string name, string value)
      : base(position, value, XmlDiffNodeType.ProcessingInstruction)
    {
      this._name = name;
    }

    internal string Name
    {
      get
      {
        return this._name;
      }
    }

    internal override void ComputeHashValue(XmlHash xmlHash)
    {
      this._hashValue = xmlHash.HashPI(this.Name, this.Value);
    }

    internal override XmlDiffOperation GetDiffOperation(
      XmlDiffNode changedNode,
      XmlDiff xmlDiff)
    {
      if (changedNode.NodeType != XmlDiffNodeType.ProcessingInstruction)
        return XmlDiffOperation.Undefined;
      XmlDiffPI xmlDiffPi = (XmlDiffPI) changedNode;
      return this.Name == xmlDiffPi.Name ? (this.Value == xmlDiffPi.Value ? XmlDiffOperation.Match : XmlDiffOperation.ChangePI) : (this.Value == xmlDiffPi.Value ? XmlDiffOperation.ChangePI : XmlDiffOperation.Undefined);
    }

    internal override void WriteTo(XmlWriter w)
    {
      w.WriteProcessingInstruction(this.Name, this.Value);
    }

    internal override void WriteContentTo(XmlWriter w)
    {
    }
  }
}
