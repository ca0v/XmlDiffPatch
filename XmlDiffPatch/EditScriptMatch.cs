// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.EditScriptMatch
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

namespace Microsoft.XmlDiffPatch
{
  internal class EditScriptMatch : EditScript
  {
    internal int _firstSourceIndex;
    internal int _firstTargetIndex;
    internal int _length;

    internal EditScriptMatch(
      int startSourceIndex,
      int startTargetIndex,
      int length,
      EditScript next)
      : base(next)
    {
      this._firstSourceIndex = startSourceIndex;
      this._firstTargetIndex = startTargetIndex;
      this._length = length;
    }

    internal override EditScriptOperation Operation
    {
      get
      {
        return EditScriptOperation.Match;
      }
    }
  }
}
