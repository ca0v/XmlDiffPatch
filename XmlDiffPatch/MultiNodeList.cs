// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.MultiNodeList
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Collections;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class MultiNodeList : XmlPatchNodeList
  {
    private int _count = 0;
    internal ListChunk _chunks = (ListChunk) null;
    private ListChunk _lastChunk = (ListChunk) null;

    internal MultiNodeList()
    {
    }

    public override XmlNode Item(int index)
    {
      if (this._chunks == null)
        return (XmlNode) null;
      if (index < 10)
        return this._chunks[index];
      var num = index / 10;
            var listChunk = this._chunks;
      for (; num > 0; --num)
        listChunk = listChunk._next;
      return listChunk[index % 10];
    }

    public override int Count
    {
      get
      {
        return this._count;
      }
    }

    public override IEnumerator GetEnumerator()
    {
      return (IEnumerator) new Enumerator(this);
    }

    internal override void AddNode(XmlNode node)
    {
      if (this._lastChunk == null)
      {
        this._chunks = new ListChunk();
        this._lastChunk = this._chunks;
      }
      else if (this._lastChunk._count == 10)
      {
        this._lastChunk._next = new ListChunk();
        this._lastChunk = this._lastChunk._next;
      }
      this._lastChunk.AddNode(node);
      ++this._count;
    }

    internal class ListChunk
    {
      internal XmlNode[] _nodes = new XmlNode[10];
      internal int _count = 0;
      internal ListChunk _next = (ListChunk) null;
      internal const int ChunkSize = 10;

      internal XmlNode this[int i]
      {
        get
        {
          return this._nodes[i];
        }
      }

      internal void AddNode(XmlNode node)
      {
        this._nodes[this._count++] = node;
      }
    }

    private class Enumerator : IEnumerator
    {
      private int _currentChunkIndex = 0;
      private MultiNodeList _nodeList;
      private ListChunk _currentChunk;

      internal Enumerator(MultiNodeList nodeList)
      {
        this._nodeList = nodeList;
        this._currentChunk = nodeList._chunks;
      }

      public object Current
      {
        get
        {
          return this._currentChunk == null ? (object) null : (object) this._currentChunk[this._currentChunkIndex];
        }
      }

      public bool MoveNext()
      {
        if (this._currentChunk == null)
          return false;
        if (this._currentChunkIndex >= this._currentChunk._count - 1)
        {
          if (this._currentChunk._next == null)
            return false;
          this._currentChunk = this._currentChunk._next;
          this._currentChunkIndex = 0;
          return true;
        }
        ++this._currentChunkIndex;
        return true;
      }

      public void Reset()
      {
        this._currentChunk = this._nodeList._chunks;
        this._currentChunkIndex = -1;
      }
    }
  }
}
