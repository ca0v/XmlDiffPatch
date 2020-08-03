// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffViewER
// Assembly: XmlDiffPatch.View, Version=1.0.1493.40755, Culture=neutral, PublicKeyToken=null
// MVID: 0D4C313F-7E60-4DB8-9CA5-4749E3A923DE
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.View.dll

using System.Diagnostics;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffViewER : XmlDiffViewNode
  {
    private string _name;

    internal XmlDiffViewER(string name)
      : base(XmlNodeType.EntityReference)
    {
      this._name = name;
    }

    internal override string OuterXml
    {
      get
      {
        return "&" + this._name + ";";
      }
    }

    internal override XmlDiffViewNode Clone(bool bDeep)
    {
      return (XmlDiffViewNode) new XmlDiffViewER(this._name);
    }

    internal override void DrawHtml(XmlWriter writer, int indent)
    {
      if (this._op == XmlDiffViewOperation.Change)
      {
        Debug.Assert(this._name != this._changeInfo._localName);
        XmlDiffView.HtmlStartRow(writer);
        XmlDiffView.HtmlStartCell(writer, indent);
        XmlDiffView.HtmlWriteString(writer, "&");
        XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Change, this._name);
        XmlDiffView.HtmlWriteString(writer, ";");
        XmlDiffView.HtmlEndCell(writer);
        XmlDiffView.HtmlStartCell(writer, indent);
        XmlDiffView.HtmlWriteString(writer, "&");
        XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Change, this._changeInfo._localName);
        XmlDiffView.HtmlWriteString(writer, ";");
        XmlDiffView.HtmlEndCell(writer);
        XmlDiffView.HtmlEndRow(writer);
      }
      else
        this.DrawHtmlNoChange(writer, indent);
    }
  }
}
