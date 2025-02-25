using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace BitzArt.XDoc;

/// <summary>
/// Builds parsed content objects by processing XML documentation nodes
/// and resolving their references and inheritance hierarchy.
/// </summary>
/// <remarks>
/// This class is responsible for:
/// - Creating ParsedContent instances from TypeDocumentation and MemberDocumentation
/// - Resolving XML documentation inheritance chains
/// - Processing XML documentation references
/// </remarks>
public class ParsedContentBuilder
{
    /// <summary>
    /// Builds a ParsedContent object from <see cref="TypeDocumentation"/>  by processing its XML documentation and resolving references.
    /// </summary>
    /// <param name="typeDocumentation">The type documentation containing XML nodes and type information.</param>
    /// <returns>A ParsedContent object containing resolved documentation, references and inheritance chain.</returns>
    public ParsedContent Build(TypeDocumentation typeDocumentation) =>
        GetParsedContent(typeDocumentation.Node, typeDocumentation.Source, typeDocumentation.Type);

    /// <summary>
    /// Builds a ParsedContent object from <see cref="MemberDocumentation{T}"/> by processing its XML documentation and resolving references.
    /// </summary>
    /// <param name="memberDocumentation">The member documentation containing XML nodes and member information.</param>
    /// <returns>A <see cref="ParsedContent"/> object containing resolved documentation, references and inheritance chain.</returns>
    /// <typeparam name="T">The type of the member being documented.</typeparam>
    public ParsedContent Build<T>(MemberDocumentation<T> memberDocumentation) where T : MemberInfo =>
        GetParsedContent(memberDocumentation.Node, memberDocumentation.Source, memberDocumentation.Member);

    public ParsedContent Build<T>(PropertyDocumentation memberDocumentation) where T : MemberInfo =>
        GetParsedContent(memberDocumentation.Node, memberDocumentation.Source, memberDocumentation.Member);

    private ParsedContent GetParsedContent<TMember>(XmlNode? xmlNode, XDoc xDoc, TMember memberInfo)
        where TMember : MemberInfo
    {
        var parent = GetParent(xmlNode, xDoc, memberInfo);
        var references = GetReferences(xmlNode, xDoc);

        return new ParsedContent
        {
            Parent = parent,
            References = references,
            Xml = xmlNode,
            Type = null
        };
    }

    private ParsedContent GetParsedContent(XmlNode? xmlNode, XDoc xDoc, Type type)
    {
        var parent = GetParent(xmlNode, xDoc, type);
        var references = GetReferences(xmlNode, xDoc);

        return new ParsedContent
        {
            Parent = parent,
            References = references,
            Xml = xmlNode,
            Type = type
        };
    }

    private IReadOnlyCollection<ParsedContent> GetReferences(XmlNode? xmlNode, XDoc xDoc)
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
                Xml = typeDocumentation?.Node,
                References = GetReferences(typeDocumentation?.Node, xDoc),
                Parent = GetParent(typeDocumentation?.Node, xDoc, type)
            });
        }

        return references;
    }

    private Type? GetTypeInfo(string typeName)
    {
        var type = AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(a => a.GetType(typeName, false))
            .FirstOrDefault(t => t != null); // What if we have multiple types with the same name and namespace?

        return type;
    }

    private ParsedContent? GetParent(XmlNode? xmlNode, XDoc xDoc, Type type)
    {
        if (xmlNode?.FirstChild?.Name != "inheritdoc" || type.BaseType == null)
        {
            return null;
        }

        var parentTypeDocumentation = xDoc.Get(type.BaseType);

        var parent = GetParent(parentTypeDocumentation?.Node, xDoc, type.BaseType);

        return parent;
    }

    private ParsedContent? GetParent(XmlNode? xmlNode, XDoc xDoc, MemberInfo memberInfo)
    {
        if (xmlNode?.FirstChild?.Name == "inheritdoc")
        {
            var parentTypes = GetParentTypes(memberInfo);

            foreach (var parent in parentTypes)
            {
                var parentMembers = parent.GetMember(memberInfo.Name);

                foreach (MemberInfo parentMember in parentMembers)
                {
                    if (parentMember is PropertyInfo parentPropertyInfo)
                    {
                        var parentPropertyDocumentation = xDoc.Get(parentPropertyInfo);

                        if (parentPropertyDocumentation == null)
                        {
                            continue;
                        }

                        var references = GetReferences(parentPropertyDocumentation.Node, xDoc);
                        var parentMemberParent = GetParent(parentPropertyDocumentation.Node, xDoc,
                            parentPropertyDocumentation.DeclaringType);

                        return new ParsedContent
                        {
                            Type = parentPropertyDocumentation.DeclaringType,
                            Xml = parentPropertyDocumentation.Node,
                            References = references,
                            Parent = parentMemberParent
                        };
                    }
                    else if (parentMember is FieldInfo parentFieldInfo)
                    {
                        // xDoc.Get(parentFieldInfo);
                    }
                    else if (parentMember is MethodInfo parentMethodInfo)
                    {
                        // xDoc.Get(parentMethodInfo);
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Retrieves a collection of parent types (interfaces and declaring type) for a given member.
    /// </summary>
    /// <param name="memberInfo">The member information to analyze.</param>
    /// <returns>A frozen set of <see cref="Type"/> objects representing parent types and interfaces.</returns>
    public IReadOnlyCollection<Type> GetParentTypes(MemberInfo memberInfo)
    {
        var result = new List<Type>();

        var interfaces = memberInfo.DeclaringType?.GetInterfaces() ?? [];

        if (interfaces.Any())
        {
            result.AddRange(interfaces);
        }

        if (memberInfo.DeclaringType != null)
        {
            result.Add(memberInfo.DeclaringType);
        }

        return result.ToFrozenSet();
    }
}