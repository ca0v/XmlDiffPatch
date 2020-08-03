// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffXmlDeclaration
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffXmlDeclaration : XmlDiffNode
  {
    private string _value;

    internal XmlDiffXmlDeclaration(int position, string value)
      : base(position)
    {
      this._value = value;
    }

    internal override XmlDiffNodeType NodeType
    {
      get
      {
        return XmlDiffNodeType.XmlDeclaration;
      }
    }

    internal string Value
    {
      get
      {
        return this._value;
      }
    }

    internal override void ComputeHashValue(XmlHash xmlHash)
    {
      this._hashValue = xmlHash.HashXmlDeclaration(this._value);
    }

    internal override XmlDiffOperation GetDiffOperation(
      XmlDiffNode changedNode,
      XmlDiff xmlDiff)
    {
      if (changedNode.NodeType != XmlDiffNodeType.XmlDeclaration)
        return XmlDiffOperation.Undefined;
      return this.Value == ((XmlDiffXmlDeclaration) changedNode).Value ? XmlDiffOperation.Match : XmlDiffOperation.ChangeXmlDeclaration;
    }

    internal override void WriteTo(XmlWriter w)
    {
      w.WriteProcessingInstruction("xml", this._value);
    }

    internal override void WriteContentTo(XmlWriter w)
    {
    }
  }
}
