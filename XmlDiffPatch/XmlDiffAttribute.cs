// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffAttribute
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffAttribute : XmlDiffAttributeOrNamespace
  {
    private string _localName;
    private string _prefix;
    private string _ns;
    private string _value;

    internal XmlDiffAttribute(string localName, string prefix, string ns, string value)
    {
      this._localName = localName;
      this._prefix = prefix;
      this._ns = ns;
      this._value = value;
    }

    internal override XmlDiffNodeType NodeType
    {
      get
      {
        return XmlDiffNodeType.Attribute;
      }
    }

    internal override string LocalName
    {
      get
      {
        return this._localName;
      }
    }

    internal override string NamespaceURI
    {
      get
      {
        return this._ns;
      }
    }

    internal override string Prefix
    {
      get
      {
        return this._prefix;
      }
    }

    internal string Name
    {
      get
      {
        return this._prefix.Length > 0 ? this._prefix + ":" + this._localName : this._localName;
      }
    }

    internal override string Value
    {
      get
      {
        return this._value;
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
      this._hashValue = xmlHash.HashAttribute(this._localName, this._prefix, this._ns, this._value);
    }

    internal override bool IsSameAs(XmlDiffNode node, XmlDiff xmlDiff)
    {
      XmlDiffAttribute xmlDiffAttribute = (XmlDiffAttribute) node;
      return this.LocalName == xmlDiffAttribute.LocalName && (xmlDiff.IgnoreNamespaces || this.NamespaceURI == xmlDiffAttribute.NamespaceURI) && (xmlDiff.IgnorePrefixes || this.Prefix == xmlDiffAttribute.Prefix) && this.Value == xmlDiffAttribute.Value;
    }

    internal override XmlDiffOperation GetDiffOperation(
      XmlDiffNode changedNode,
      XmlDiff xmlDiff)
    {
      return XmlDiffOperation.Undefined;
    }

    internal override void WriteTo(XmlWriter w)
    {
      w.WriteStartAttribute(this.Prefix, this.LocalName, this.NamespaceURI);
      this.WriteContentTo(w);
      w.WriteEndAttribute();
    }

    internal override void WriteContentTo(XmlWriter w)
    {
      w.WriteString(this.Value);
    }

    internal override string GetRelativeAddress()
    {
      return "@" + this.Name;
    }
  }
}
