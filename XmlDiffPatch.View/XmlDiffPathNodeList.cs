// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffPathNodeList
// Assembly: XmlDiffPatch.View, Version=1.0.1493.40755, Culture=neutral, PublicKeyToken=null
// MVID: 0D4C313F-7E60-4DB8-9CA5-4749E3A923DE
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.View.dll

namespace Microsoft.XmlDiffPatch
{
  internal abstract class XmlDiffPathNodeList
  {
    internal abstract void AddNode(XmlDiffViewNode node);

    internal abstract void Reset();

    internal abstract XmlDiffViewNode Current { get; }

    internal abstract bool MoveNext();

    internal abstract int Count { get; }
  }
}
