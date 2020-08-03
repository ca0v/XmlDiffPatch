// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffViewAttribute
// Assembly: XmlDiffPatch.View, Version=1.0.1493.40755, Culture=neutral, PublicKeyToken=null
// MVID: 0D4C313F-7E60-4DB8-9CA5-4749E3A923DE
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.View.dll

using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffViewAttribute : XmlDiffViewNode
  {
    internal string _localName;
    internal string _prefix;
    internal string _ns;
    internal string _name;
    internal string _value;

    internal XmlDiffViewAttribute(
      string localName,
      string prefix,
      string ns,
      string name,
      string value)
      : base(XmlNodeType.Attribute)
    {
      this._localName = localName;
      this._prefix = prefix;
      this._ns = ns;
      this._value = value;
      this._name = name;
    }

    internal XmlDiffViewAttribute(string localName, string prefix, string ns, string value)
      : base(XmlNodeType.Attribute)
    {
      this._localName = localName;
      this._prefix = prefix;
      this._ns = ns;
      this._value = value;
      if (prefix == string.Empty)
        this._name = this._localName;
      else
        this._name = this._prefix + ":" + this._localName;
    }

    internal override string OuterXml
    {
      get
      {
        string str = string.Empty;
        if (this._prefix != string.Empty)
          str = this._prefix + ":";
        return str + this._localName + "=\"" + this._value + "\"";
      }
    }

    internal override XmlDiffViewNode Clone(bool bDeep)
    {
      return (XmlDiffViewNode) new XmlDiffViewAttribute(this._localName, this._prefix, this._name, this._ns, this._value);
    }

    internal override void DrawHtml(XmlWriter writer, int indent)
    {
      throw new Exception("This methods should never be called.");
    }
  }
}
