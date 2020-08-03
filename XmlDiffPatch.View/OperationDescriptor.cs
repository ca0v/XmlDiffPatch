// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.OperationDescriptor
// Assembly: XmlDiffPatch.View, Version=1.0.1493.40755, Culture=neutral, PublicKeyToken=null
// MVID: 0D4C313F-7E60-4DB8-9CA5-4749E3A923DE
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.View.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class OperationDescriptor
    {
        internal readonly ulong _opid;
        internal Type _type;
        internal XmlDiffPathMultiNodeList _nodeList;
        internal OperationDescriptor _nextDescriptor;

        internal OperationDescriptor(ulong opid)
        {
            this._opid = opid;
        }

        public OperationDescriptor(int opid, Type type)
        {
            this._opid = (ulong)opid;
            this._type = type;
            this._nodeList = new XmlDiffPathMultiNodeList();
        }

        internal enum Type
        {
            Move,
            NamespaceChange,
            PrefixChange,
        }

        internal virtual void WriteTo(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("xd", "descriptor", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
            xmlWriter.WriteAttributeString("opid", this._opid.ToString());
            xmlWriter.WriteAttributeString("type", System.Enum.GetName(typeof(Type), this._type));
            xmlWriter.WriteEndElement();
        }
    }
}
