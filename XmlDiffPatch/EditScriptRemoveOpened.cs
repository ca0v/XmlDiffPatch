// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.EditScriptRemoveOpened
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

namespace Microsoft.XmlDiffPatch
{
  internal class EditScriptRemoveOpened : EditScriptOpened
  {
    internal int _startSourceIndex;

    internal EditScriptRemoveOpened(int startSourceIndex, EditScript next)
      : base(next)
    {
      this._startSourceIndex = startSourceIndex;
    }

    internal override EditScriptOperation Operation
    {
      get
      {
        return EditScriptOperation.OpenedRemove;
      }
    }

    internal override EditScript GetClosedScript(
      int currentSourceIndex,
      int currentTargetIndex)
    {
      return (EditScript) new EditScriptRemove(this._startSourceIndex, currentSourceIndex, this._nextEditScript);
    }
  }
}
