// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlPatchParentOperation
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal abstract class XmlPatchParentOperation : XmlPatchOperation
  {
    internal XmlPatchOperation _firstChild;

    internal void InsertChildAfter(XmlPatchOperation child, XmlPatchOperation newChild)
    {
      if (child == null)
      {
        newChild._nextOp = this._firstChild;
        this._firstChild = newChild;
      }
      else
      {
        newChild._nextOp = child._nextOp;
        child._nextOp = newChild;
      }
    }

    protected void ApplyChildren(XmlNode parent)
    {
            var currentPosition = (XmlNode) null;
      for (var xmlPatchOperation = this._firstChild; xmlPatchOperation != null; xmlPatchOperation = xmlPatchOperation._nextOp)
        xmlPatchOperation.Apply(parent, ref currentPosition);
    }
  }
}
