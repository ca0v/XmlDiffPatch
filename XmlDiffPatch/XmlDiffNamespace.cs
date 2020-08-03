// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffNamespace
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffNamespace : XmlDiffAttributeOrNamespace
  {
    private string _prefix;
    private string _namespaceURI;

    internal XmlDiffNamespace(string prefix, string namespaceURI)
    {
      this._prefix = prefix;
      this._namespaceURI = namespaceURI;
    }

    internal override XmlDiffNodeType NodeType
    {
      get
      {
        return XmlDiffNodeType.Namespace;
      }
    }

    internal override string Prefix
    {
      get
      {
        return this._prefix;
      }
    }

    internal override string NamespaceURI
    {
      get
      {
        return this._namespaceURI;
      }
    }

    internal override string LocalName
    {
      get
      {
        return string.Empty;
      }
    }

    internal override string Value
    {
      get
      {
        return string.Empty;
      }
    }

    internal string Name
    {
      get
      {
        return this._prefix.Length > 0 ? "xmlns:" + this._prefix : "xmlns";
      }
    }

    internal override void ComputeHashValue(XmlHash xmlHash)
    {
      this._hashValue = xmlHash.HashNamespace(this._prefix, this._namespaceURI);
    }

    internal override XmlDiffOperation GetDiffOperation(
      XmlDiffNode changedNode,
      XmlDiff xmlDiff)
    {
      return XmlDiffOperation.Undefined;
    }

    internal override void WriteTo(XmlWriter w)
    {
      if (this.Prefix == string.Empty)
        w.WriteAttributeString(string.Empty, "xmlns", "http://www.w3.org/2000/xmlns/", this.NamespaceURI);
      else
        w.WriteAttributeString("xmlns", this.Prefix, "http://www.w3.org/2000/xmlns/", this.NamespaceURI);
    }

    internal override void WriteContentTo(XmlWriter w)
    {
    }

    internal override string GetRelativeAddress()
    {
      return "@" + this.Name;
    }
  }
}
