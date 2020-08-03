// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.OperationDescriptor
// Assembly: XmlDiffPatch.View, Version=1.0.1493.40755, Culture=neutral, PublicKeyToken=null
// MVID: 0D4C313F-7E60-4DB8-9CA5-4749E3A923DE
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.View.dll

namespace Microsoft.XmlDiffPatch
{
  internal class OperationDescriptor
  {
    private int _opid;
    internal OperationDescriptor.Type _type;
    internal XmlDiffPathMultiNodeList _nodeList;

    public OperationDescriptor(int opid, OperationDescriptor.Type type)
    {
      this._opid = opid;
      this._type = type;
      this._nodeList = new XmlDiffPathMultiNodeList();
    }

    internal enum Type
    {
      Move,
      NamespaceChange,
      PrefixChange,
    }
  }
}
