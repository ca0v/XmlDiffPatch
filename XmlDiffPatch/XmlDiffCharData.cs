// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffCharData
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class XmlDiffCharData : XmlDiffNode
    {
        private string _value;
        private XmlDiffNodeType _nodeType;

        internal XmlDiffCharData(int position, string value, XmlDiffNodeType nodeType)
          : base(position)
        {
            this._value = value;
            this._nodeType = nodeType;
        }

        internal override XmlDiffNodeType NodeType
        {
            get
            {
                return this._nodeType;
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
            this._hashValue = xmlHash.HashCharacterNode((XmlNodeType)this._nodeType, this._value);
        }

        internal override XmlDiffOperation GetDiffOperation(
          XmlDiffNode changedNode,
          XmlDiff xmlDiff)
        {
            if (this.NodeType != changedNode.NodeType || !(changedNode is XmlDiffCharData xmlDiffCharData))
                return XmlDiffOperation.Undefined;
            return this.Value == xmlDiffCharData.Value ? XmlDiffOperation.Match : XmlDiffOperation.ChangeCharacterData;
        }

        internal override void WriteTo(XmlWriter w)
        {
            switch (this._nodeType)
            {
                case XmlDiffNodeType.Text:
                case XmlDiffNodeType.SignificantWhitespace:
                    w.WriteString(this.Value);
                    break;
                case XmlDiffNodeType.CDATA:
                    w.WriteCData(this.Value);
                    break;
                case XmlDiffNodeType.Comment:
                    w.WriteComment(this.Value);
                    break;
            }
        }

        internal override void WriteContentTo(XmlWriter w)
        {
        }
    }
}
