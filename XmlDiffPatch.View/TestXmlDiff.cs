// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.TestXmlDiff
// Assembly: XmlDiffPatch.View, Version=1.0.1493.40755, Culture=neutral, PublicKeyToken=null
// MVID: 0D4C313F-7E60-4DB8-9CA5-4749E3A923DE
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.View.dll

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class TestXmlDiff
  {
    private static void Main(string[] args)
    {
      bool bFragments = false;
      bool flag1 = false;
      XmlDiffAlgorithm xmlDiffAlgorithm = XmlDiffAlgorithm.Auto;
      try
      {
        if (args.Length < 3)
        {
          TestXmlDiff.WriteUsage();
          return;
        }
        XmlDiffOptions options = XmlDiffOptions.None;
        int index = 0;
        string empty = string.Empty;
        while (args[index][0] == '/')
        {
          if (args[index].Length != 2)
          {
            Console.Write("Invalid option: " + args[index] + "\n");
            return;
          }
          switch (args[index][1])
          {
            case 'c':
              options |= XmlDiffOptions.IgnoreComments;
              break;
            case 'd':
              options |= XmlDiffOptions.IgnoreDtd;
              break;
            case 'e':
              flag1 = true;
              break;
            case 'f':
              bFragments = true;
              break;
            case 'n':
              options |= XmlDiffOptions.IgnoreNamespaces;
              break;
            case 'o':
              options |= XmlDiffOptions.IgnoreChildOrder;
              break;
            case 'p':
              options |= XmlDiffOptions.IgnorePI;
              break;
            case 'r':
              options |= XmlDiffOptions.IgnorePrefixes;
              break;
            case 't':
              xmlDiffAlgorithm = XmlDiffAlgorithm.Fast;
              break;
            case 'w':
              options |= XmlDiffOptions.IgnoreWhitespace;
              break;
            case 'x':
              options |= XmlDiffOptions.IgnoreXmlDecl;
              break;
            case 'z':
              xmlDiffAlgorithm = XmlDiffAlgorithm.Precise;
              break;
            default:
              Console.Write("Invalid option: " + args[index] + "\n");
              return;
          }
          empty += (string) (object) args[index][1];
          ++index;
          if (args.Length - index < 3)
          {
            TestXmlDiff.WriteUsage();
            return;
          }
        }
        string str1 = args[index];
        string str2 = args[index + 1];
        string str3 = args[index + 2];
        bool flag2 = args.Length - index == 4 && args[index + 3] == "verify";
        string str4 = str1.Substring(str1.LastIndexOf("\\") + 1) + " & " + str2.Substring(str2.LastIndexOf("\\") + 1) + " -> " + str3.Substring(str3.LastIndexOf("\\") + 1);
        if (empty != string.Empty)
          str4 = str4 + " (" + empty + ")";
        Console.Write(str4.Length >= 60 ? str4 + "\n" + new string(' ', 60) : str4 + new string(' ', 60 - str4.Length));
        XmlWriter diffgramWriter1 = (XmlWriter) new XmlTextWriter(str3, (Encoding) new UnicodeEncoding());
        XmlDiff xmlDiff = new XmlDiff(options);
        xmlDiff.Algorithm = xmlDiffAlgorithm;
        bool flag3;
        if (flag1)
        {
          if (bFragments)
          {
            Console.Write("Cannot have option 'd' and 'f' together.");
            return;
          }
          XmlDocument xmlDocument1 = new XmlDocument();
          xmlDocument1.Load(str1);
          XmlDocument xmlDocument2 = new XmlDocument();
          xmlDocument2.Load(str2);
          flag3 = xmlDiff.Compare((XmlNode) xmlDocument1, (XmlNode) xmlDocument2, diffgramWriter1);
        }
        else
          flag3 = xmlDiff.Compare(str1, str2, bFragments, diffgramWriter1);
        if (flag3)
          Console.Write("identical");
        else
          Console.Write("different");
        diffgramWriter1.Close();
        if (!flag3 && flag2)
        {
          XmlNode sourceNode;
          if (bFragments)
          {
            NameTable nameTable = new NameTable();
            XmlTextReader xmlTextReader = new XmlTextReader((Stream) new FileStream(str1, FileMode.Open, FileAccess.Read), XmlNodeType.Element, new XmlParserContext((XmlNameTable) nameTable, new XmlNamespaceManager((XmlNameTable) nameTable), string.Empty, XmlSpace.Default));
            XmlDocument xmlDocument = new XmlDocument();
            XmlDocumentFragment documentFragment = xmlDocument.CreateDocumentFragment();
            XmlNode newChild;
            while ((newChild = xmlDocument.ReadNode((XmlReader) xmlTextReader)) != null)
            {
              if (newChild.NodeType != XmlNodeType.Whitespace)
                documentFragment.AppendChild(newChild);
            }
            sourceNode = (XmlNode) documentFragment;
          }
          else
          {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.XmlResolver = (XmlResolver) null;
            xmlDocument.Load(str1);
            sourceNode = (XmlNode) xmlDocument;
          }
          new XmlPatch().Patch(ref sourceNode, (XmlReader) new XmlTextReader(str3));
          if (sourceNode.NodeType == XmlNodeType.Document)
          {
            ((XmlDocument) sourceNode).Save("_patched.xml");
          }
          else
          {
            XmlTextWriter xmlTextWriter = new XmlTextWriter("_patched.xml", Encoding.Unicode);
            sourceNode.WriteTo((XmlWriter) xmlTextWriter);
            xmlTextWriter.Close();
          }
          XmlWriter diffgramWriter2 = (XmlWriter) new XmlTextWriter("_2ndDiff.xml", (Encoding) new UnicodeEncoding());
          if (xmlDiff.Compare("_patched.xml", str2, bFragments, diffgramWriter2))
            Console.Write(" - ok");
          else
            Console.Write(" - FAILED");
          diffgramWriter2.Close();
        }
        Console.Write("\n");
      }
      catch (Exception ex)
      {
        Console.Write("\n*** Error: " + ex.Message + " (source: " + ex.Source + ")\n");
      }
      if (!Debugger.IsAttached)
        return;
      Console.Write("\nPress enter...\n");
      Console.Read();
    }

    private static void WriteUsage()
    {
      Console.Write("TestXmlDiff - test application for XmlDiff\n");
      Console.Write("USAGE: testapp [options] <source xml> <target xml> <diffgram> [verify]\n\nOptions:\n/o    ignore child order\n/c    ignore comments\n/p    ignore processing instructions\n/w    ignore whitespaces, normalize text value\n/n    ignore namespaces\n/r    ignore prefixes\n/x    ignore XML declaration\n");
    }
  }
}
