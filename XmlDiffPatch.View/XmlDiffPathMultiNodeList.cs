// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffPathMultiNodeList
// Assembly: XmlDiffPatch.View, Version=1.0.1493.40755, Culture=neutral, PublicKeyToken=null
// MVID: 0D4C313F-7E60-4DB8-9CA5-4749E3A923DE
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.View.dll

using System.Diagnostics;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffPathMultiNodeList : XmlDiffPathNodeList
  {
    private int _count = 0;
    private ListChunk _chunks = (ListChunk) null;
    private ListChunk _lastChunk = (ListChunk) null;
    private ListChunk _currentChunk = (ListChunk) null;
    private int _currentChunkIndex = -1;

    internal XmlDiffPathMultiNodeList()
    {
    }

    internal override XmlDiffViewNode Current
    {
      get
      {
        return this._currentChunk == null || this._currentChunkIndex < 0 ? (XmlDiffViewNode) null : this._currentChunk[this._currentChunkIndex];
      }
    }

    internal override int Count
    {
      get
      {
        return this._count;
      }
    }

    internal override bool MoveNext()
    {
      if (this._currentChunk == null)
        return false;
      if (this._currentChunkIndex >= this._currentChunk._count - 1)
      {
        if (this._currentChunk._next == null)
          return false;
        this._currentChunk = this._currentChunk._next;
        this._currentChunkIndex = 0;
        Debug.Assert(this._currentChunk._count > 0);
        return true;
      }
      ++this._currentChunkIndex;
      return true;
    }

    internal override void Reset()
    {
      this._currentChunk = this._chunks;
      this._currentChunkIndex = -1;
    }

    internal override void AddNode(XmlDiffViewNode node)
    {
      if (this._lastChunk == null)
      {
        this._chunks = new ListChunk();
        this._lastChunk = this._chunks;
        this._currentChunk = this._chunks;
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
      internal XmlDiffViewNode[] _nodes = new XmlDiffViewNode[10];
      internal int _count = 0;
      internal ListChunk _next = (ListChunk) null;
      internal const int ChunkSize = 10;

      internal XmlDiffViewNode this[int i]
      {
        get
        {
          return this._nodes[i];
        }
      }

      internal void AddNode(XmlDiffViewNode node)
      {
        Debug.Assert(this._count < 10);
        this._nodes[this._count++] = node;
      }
    }
  }
}
