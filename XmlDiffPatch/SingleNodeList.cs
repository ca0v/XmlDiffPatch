// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.SingleNodeList
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Collections;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class SingleNodeList : XmlPatchNodeList
  {
    private XmlNode _node;

    internal SingleNodeList()
    {
    }

    public override XmlNode Item(int index)
    {
      return index == 0 ? this._node : (XmlNode) null;
    }

    public override int Count
    {
      get
      {
        return 1;
      }
    }

    public override IEnumerator GetEnumerator()
    {
      return (IEnumerator) new Enumerator(this._node);
    }

    internal override void AddNode(XmlNode node)
    {
      if (this._node != null)
        XmlPatchError.Error("Internal Error. XmlDiffPathSingleNodeList can contain one node only.");
      this._node = node;
    }

    private class Enumerator : IEnumerator
    {
      private State _state = SingleNodeList.Enumerator.State.BeforeNode;
      private XmlNode _node;

      internal Enumerator(XmlNode node)
      {
        this._node = node;
      }

      public object Current
      {
        get
        {
          return this._state != SingleNodeList.Enumerator.State.OnNode ? (object) null : (object) this._node;
        }
      }

      public bool MoveNext()
      {
        switch (this._state)
        {
          case SingleNodeList.Enumerator.State.BeforeNode:
            this._state = SingleNodeList.Enumerator.State.OnNode;
            return true;
          case SingleNodeList.Enumerator.State.OnNode:
            this._state = SingleNodeList.Enumerator.State.AfterNode;
            return false;
          case SingleNodeList.Enumerator.State.AfterNode:
            return false;
          default:
            return false;
        }
      }

      public void Reset()
      {
        this._state = SingleNodeList.Enumerator.State.BeforeNode;
      }

      private enum State
      {
        BeforeNode,
        OnNode,
        AfterNode,
      }
    }
  }
}
