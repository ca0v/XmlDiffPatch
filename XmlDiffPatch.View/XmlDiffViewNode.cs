// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffViewNode
// Assembly: XmlDiffPatch.View, Version=1.0.1493.40755, Culture=neutral, PublicKeyToken=null
// MVID: 0D4C313F-7E60-4DB8-9CA5-4749E3A923DE
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.View.dll

using System.Diagnostics;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal abstract class XmlDiffViewNode
  {
    internal XmlDiffViewNode _nextSibbling = (XmlDiffViewNode) null;
    internal XmlDiffViewNode _parent = (XmlDiffViewNode) null;
    internal XmlDiffViewOperation _op = XmlDiffViewOperation.Match;
    internal int _opid = 0;
    internal XmlDiffViewNode.ChangeInfo _changeInfo = (XmlDiffViewNode.ChangeInfo) null;
    internal XmlNodeType _nodeType;

    internal XmlDiffViewNode(XmlNodeType nodeType)
    {
      this._nodeType = nodeType;
    }

    internal abstract string OuterXml { get; }

    internal virtual XmlDiffViewNode FirstChildNode
    {
      get
      {
        return (XmlDiffViewNode) null;
      }
    }

    internal abstract XmlDiffViewNode Clone(bool bDeep);

    internal abstract void DrawHtml(XmlWriter writer, int indent);

    internal void DrawHtmlNoChange(XmlWriter writer, int indent)
    {
      XmlDiffView.HtmlStartRow(writer);
      for (int index = 0; index < 2; ++index)
      {
        XmlDiffView.HtmlStartCell(writer, indent);
        if (XmlDiffView.HtmlWriteToPane[(int) this._op, index])
        {
          bool flag = this.OutputNavigation(writer);
          XmlDiffView.HtmlWriteString(writer, this._op, this.OuterXml);
          if (flag)
            writer.WriteEndElement();
        }
        else
          XmlDiffView.HtmlWriteEmptyString(writer);
        XmlDiffView.HtmlEndCell(writer);
      }
      XmlDiffView.HtmlEndRow(writer);
    }

    protected bool OutputNavigation(XmlWriter writer)
    {
      if (this._parent == null || this._parent._op != this._op)
      {
        switch (this._op)
        {
          case XmlDiffViewOperation.MoveTo:
            writer.WriteStartElement("a");
            writer.WriteAttributeString("name", "move_to_" + (object) this._opid);
            writer.WriteEndElement();
            writer.WriteStartElement("a");
            writer.WriteAttributeString("href", "#move_from_" + (object) this._opid);
            return true;
          case XmlDiffViewOperation.MoveFrom:
            writer.WriteStartElement("a");
            writer.WriteAttributeString("name", "move_from_" + (object) this._opid);
            writer.WriteEndElement();
            writer.WriteStartElement("a");
            writer.WriteAttributeString("href", "#move_to_" + (object) this._opid);
            return true;
        }
      }
      return false;
    }

    internal class ChangeInfo
    {
      internal string _localName;
      internal string _prefix;
      internal string _ns;
      internal string _value;
    }
  }
}
