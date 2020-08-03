// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffPathSingleNodeList
// Assembly: XmlDiffPatch.View, Version=1.0.1493.40755, Culture=neutral, PublicKeyToken=null
// MVID: 0D4C313F-7E60-4DB8-9CA5-4749E3A923DE
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.View.dll

using System;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffPathSingleNodeList : XmlDiffPathNodeList
  {
    private XmlDiffPathSingleNodeList.State _state = XmlDiffPathSingleNodeList.State.BeforeNode;
    private XmlDiffViewNode _node;

    internal XmlDiffPathSingleNodeList()
    {
    }

    internal override int Count
    {
      get
      {
        return 1;
      }
    }

    internal override XmlDiffViewNode Current
    {
      get
      {
        return this._state == XmlDiffPathSingleNodeList.State.OnNode ? this._node : (XmlDiffViewNode) null;
      }
    }

    internal override bool MoveNext()
    {
      switch (this._state)
      {
        case XmlDiffPathSingleNodeList.State.BeforeNode:
          this._state = XmlDiffPathSingleNodeList.State.OnNode;
          return true;
        case XmlDiffPathSingleNodeList.State.OnNode:
          this._state = XmlDiffPathSingleNodeList.State.AfterNode;
          return false;
        case XmlDiffPathSingleNodeList.State.AfterNode:
          return false;
        default:
          return false;
      }
    }

    internal override void Reset()
    {
      this._state = XmlDiffPathSingleNodeList.State.BeforeNode;
    }

    internal override void AddNode(XmlDiffViewNode node)
    {
      if (this._node != null)
        throw new Exception("XmlDiffPathSingleNodeList can contain one node only.");
      this._node = node;
    }

    private enum State
    {
      BeforeNode,
      OnNode,
      AfterNode,
    }
  }
}
