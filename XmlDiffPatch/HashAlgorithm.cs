// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.HashAlgorithm
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

namespace Microsoft.XmlDiffPatch
{
  internal class HashAlgorithm
  {
    private ulong _hash;

    internal HashAlgorithm()
    {
    }

    internal ulong Hash
    {
      get
      {
        return this._hash;
      }
    }

    internal static ulong GetHash(string data)
    {
      return HashAlgorithm.GetHash(data, 0UL);
    }

    internal void AddString(string data)
    {
      this._hash = HashAlgorithm.GetHash(data, this._hash);
    }

    internal void AddInt(int i)
    {
      this._hash += (this._hash << 11) + (ulong) i;
    }

    internal void AddULong(ulong u)
    {
      this._hash += (this._hash << 11) + u;
    }

    private static ulong GetHash(string data, ulong hash)
    {
      hash += (hash << 13) + (ulong) data.Length;
      for (var index = 0; index < data.Length; ++index)
        hash += (hash << 17) + (ulong) data[index];
      return hash;
    }
  }
}
