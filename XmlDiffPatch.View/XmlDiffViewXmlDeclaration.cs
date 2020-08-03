// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffViewXmlDeclaration
// Assembly: XmlDiffPatch.View, Version=1.0.1493.40755, Culture=neutral, PublicKeyToken=null
// MVID: 0D4C313F-7E60-4DB8-9CA5-4749E3A923DE
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.View.dll

using System.Diagnostics;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffViewXmlDeclaration : XmlDiffViewNode
  {
    private string _value;

    internal XmlDiffViewXmlDeclaration(string value)
      : base(XmlNodeType.XmlDeclaration)
    {
      this._value = value;
    }

    internal override string OuterXml
    {
      get
      {
        return "<?xml " + this._value + "?>";
      }
    }

    internal override XmlDiffViewNode Clone(bool bDeep)
    {
      return (XmlDiffViewNode) new XmlDiffViewXmlDeclaration(this._value);
    }

    internal override void DrawHtml(XmlWriter writer, int indent)
    {
      if (this._op == XmlDiffViewOperation.Change)
      {
        Debug.Assert(this._value != this._changeInfo._value);
        XmlDiffView.HtmlStartRow(writer);
        XmlDiffView.HtmlStartCell(writer, indent);
        XmlDiffView.HtmlWriteString(writer, "<?xml ");
        XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Change, this._value);
        XmlDiffView.HtmlWriteString(writer, "?>");
        XmlDiffView.HtmlEndCell(writer);
        XmlDiffView.HtmlStartCell(writer, indent);
        XmlDiffView.HtmlWriteString(writer, "<?xml ");
        XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Change, this._changeInfo._value);
        XmlDiffView.HtmlWriteString(writer, "?>");
        XmlDiffView.HtmlEndCell(writer);
        XmlDiffView.HtmlEndRow(writer);
      }
      else
        this.DrawHtmlNoChange(writer, indent);
    }
  }
}
