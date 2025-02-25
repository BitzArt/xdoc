using System.Collections.Immutable;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace BitzArt.XDoc;

public class ParsedContentBuilder
{
    public ParsedContent Build(TypeDocumentation typeDocumentation)
    {
        var xmlNode = typeDocumentation.Node ?? new XmlDocument();
        var xDoc = typeDocumentation.Source;
        var type = typeDocumentation.Type;

        var parent = GetParent(xmlNode, xDoc, type);

        var references = GetReferences(xmlNode, xDoc);

        var parsedContent = new ParsedContent
        {
            Parent = parent,
            References = references,
            Xml = xmlNode,
            Type = type
        };

        return parsedContent;
    }

    private IReadOnlyCollection<ParsedContent> GetReferences(XmlNode xmlNode, XDoc xDoc)
    {
        if (xmlNode == null || string.IsNullOrWhiteSpace(xmlNode.InnerXml))
        {
            return ImmutableList<ParsedContent>.Empty;
        }
        
        var doc = XDocument.Parse(xmlNode.InnerXml);

        var refs = doc.Descendants("see")
            .Select(e => e.Attribute("cref")?.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(o => o!)
            .Distinct()
            .ToList();

        //xDoc.Get()

        var references = new List<ParsedContent>();

        foreach (var r in refs)
        {
            var typeName = r.Substring(2, r.Length - 2);
            var type = GetTypeInfo(typeName);

            if (type == null)
            {
                continue;
            }

            var typeDocumentation = xDoc.Get(type);

            references.Add(new ParsedContent
            {
                Type = type,
                Xml = typeDocumentation.Node,
                References = GetReferences(typeDocumentation.Node, xDoc),
                Parent = GetParent(typeDocumentation.Node, xDoc, type)
            });
        }

        return references;
    }

    private Type? GetTypeInfo(string typeName)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        var type = assemblies
            .Select(a => a.GetType(typeName, false))
            .FirstOrDefault(t => t != null);

        return type;
    }

    private ParsedContent? GetParent(XmlNode xmlNode, XDoc xDoc, Type type)
    {
        if (xmlNode?.FirstChild?.Name == "inheritdoc")
        {
            var parentTypeDocumentation = xDoc.Get(type.BaseType);

            var parent = GetParent(parentTypeDocumentation.Node, xDoc, type.BaseType);

            return parent;
        }
        // <member name="P:TestAssembly.B.Dog.Field1">
        //     <inheritdoc/>
        //     </member>

        return null;
    }

    public ParsedContent Build<T>(MemberDocumentation<T> memberDocumentation) where T : class
    {
        var xmlNode = memberDocumentation.Node;
        var xDoc = memberDocumentation.Source;
        var type = memberDocumentation.DeclaringType;

        var parent = GetParent(xmlNode, xDoc, type);

        var references = GetReferences(xmlNode, xDoc);

        var parsedContent = new ParsedContent
        {
            Parent = parent,
            References = references,
            Xml = xmlNode,
            Type = type
        };

        return parsedContent;
    }
}