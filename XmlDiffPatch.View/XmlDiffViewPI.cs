// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffViewPI
// Assembly: XmlDiffPatch.View, Version=1.0.1493.40755, Culture=neutral, PublicKeyToken=null
// MVID: 0D4C313F-7E60-4DB8-9CA5-4749E3A923DE
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.View.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffViewPI : XmlDiffViewCharData
  {
    internal string _name;

    internal XmlDiffViewPI(string name, string value)
      : base(value, XmlNodeType.ProcessingInstruction)
    {
      this._name = name;
    }

    internal override string OuterXml
    {
      get
      {
        return "<?" + this._name + " " + this._value + "?>";
      }
    }

    internal override XmlDiffViewNode Clone(bool bDeep)
    {
      return (XmlDiffViewNode) new XmlDiffViewPI(this._name, this._value);
    }

    internal override void DrawHtml(XmlWriter writer, int indent)
    {
      if (this._op == XmlDiffViewOperation.Change)
      {
                var op1 = this._name == this._changeInfo._localName ? XmlDiffViewOperation.Match : XmlDiffViewOperation.Change;
                var op2 = this._value == this._changeInfo._value ? XmlDiffViewOperation.Match : XmlDiffViewOperation.Change;
        XmlDiffView.HtmlStartRow(writer);
        XmlDiffView.HtmlStartCell(writer, indent);
        XmlDiffView.HtmlWriteString(writer, "<?");
        XmlDiffView.HtmlWriteString(writer, op1, this._name);
        XmlDiffView.HtmlWriteString(writer, " ");
        XmlDiffView.HtmlWriteString(writer, op2, this._value);
        XmlDiffView.HtmlWriteString(writer, "?>");
        XmlDiffView.HtmlEndCell(writer);
        XmlDiffView.HtmlStartCell(writer, indent);
        XmlDiffView.HtmlWriteString(writer, "<?");
        XmlDiffView.HtmlWriteString(writer, op1, this._changeInfo._localName);
        XmlDiffView.HtmlWriteString(writer, " ");
        XmlDiffView.HtmlWriteString(writer, op2, this._changeInfo._value);
        XmlDiffView.HtmlWriteString(writer, "?>");
        XmlDiffView.HtmlEndCell(writer);
        XmlDiffView.HtmlEndRow(writer);
      }
      else
        this.DrawHtmlNoChange(writer, indent);
    }
  }
}
