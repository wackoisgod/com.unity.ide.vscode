using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace VSCodeEditor.Tests
{
    public static class XMLUtilities
    {
        public static void AssertCompileItemsMatchExactly(XmlDocument projectXml, IEnumerable<string> expectedCompileItems)
        {
            var compileItems = projectXml.SelectAttributeValues("/Project/ItemGroup/Compile/@Include", GetModifiedXmlNamespaceManager(projectXml)).ToArray();
            CollectionAssert.AreEquivalent(expectedCompileItems, compileItems);
        }
        
        public static void AssertAnalyzerItemsMatchExactly(XmlDocument projectXml, IEnumerable<string> expectedAnalyzers)
        {
            CollectionAssert.AreEquivalent(
                expected: RelativeAssetPathsFor(expectedAnalyzers), 
                actual:projectXml.SelectAttributeValues("/Project/ItemGroup/Analyzer/@Include", GetModifiedXmlNamespaceManager(projectXml)).ToArray());
        }
        
        public static void AssertAnalyzerRuleSetsMatchExactly(XmlDocument projectXml, string expectedRuleSetFile)
        {
            CollectionAssert.Contains(
                projectXml.SelectInnerText("/Project/PropertyGroup/CodeAnalysisRuleSet",
                    GetModifiedXmlNamespaceManager(projectXml)).ToArray(), expectedRuleSetFile);
        }

        public static void AssertNonCompileItemsMatchExactly(XmlDocument projectXml, IEnumerable<string> expectedNoncompileItems)
        {
            var nonCompileItems = projectXml.SelectAttributeValues("/Project/ItemGroup/None/@Include", GetModifiedXmlNamespaceManager(projectXml)).ToArray();
            CollectionAssert.AreEquivalent(expectedNoncompileItems, nonCompileItems);
        }

        static XmlNamespaceManager GetModifiedXmlNamespaceManager(XmlDocument projectXml)
        {
            var xmlNamespaces = new XmlNamespaceManager(projectXml.NameTable);
            xmlNamespaces.AddNamespace("msb", "http://schemas.microsoft.com/developer/msbuild/2003");
            return xmlNamespaces;
        }

        static IEnumerable<string> RelativeAssetPathsFor(IEnumerable<string> fileNames)
        {
            return fileNames.Select(fileName => fileName.NormalizePath()).ToArray();
        }

        static IEnumerable<string> SelectAttributeValues(this XmlDocument xmlDocument, string xpathQuery, XmlNamespaceManager xmlNamespaceManager)
        {
            var result = xmlDocument.SelectNodes(xpathQuery, xmlNamespaceManager);
            foreach (XmlAttribute attribute in result)
                yield return attribute.Value;
        }

        static IEnumerable<string> SelectInnerText(this XmlDocument xmlDocument, string xpathQuery, XmlNamespaceManager xmlNamespaceManager)
        {
            var result = xmlDocument.SelectNodes(xpathQuery);
            foreach (XmlElement node in result)
            {
                yield return node.InnerText;
            }
        }

        public static XmlDocument FromText(string textContent)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(textContent);
            return xmlDocument;
        }

        public static string GetInnerText(XmlDocument xmlDocument, string xpathQuery)
        {
            return xmlDocument.SelectSingleNode(xpathQuery, GetModifiedXmlNamespaceManager(xmlDocument)).InnerText;
        }
    }
}
