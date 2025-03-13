using System.Text;
using System.Xml;

namespace BitzArt.XDoc;

/// <summary>
/// Lightweight XML renderer that converts XML documentation to plain text.
/// This implementation, can only render the text content of the XML nodes, but not resolve and render references.
/// </summary>
public class PlainTextRenderer
{
    /// <summary>
    /// Converts an XML documentation node to the plain text.
    /// </summary>
    /// <param name="documentation"></param>
    /// <returns></returns>
    public static string Render(MemberDocumentation? documentation)
    {
        if (documentation == null)
        {
            return string.Empty;
        }

        var xmlNode = documentation.Inherited != null
            ? documentation.Inherited.RequirementNode
            : documentation.Node;

        var result = Render(xmlNode);

        return result;
    }

    /// <summary>
    /// Renders the content of an XML node to plain text.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    internal static string Render(XmlNode? node)
        => Normalize(node switch
    {
        null => string.Empty,
        XmlText textNode => RenderTextNode(textNode),
        XmlElement element => RenderXmlElement(element),
        _ => node.InnerText
    });

    /// <summary>
    /// Normalize the input string by removing extra empty lines and trimming each line.
    /// </summary>
    private static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var lines = input
            .Split('\n')
            .Select(line => line.Trim())
            .Where(o => !string.IsNullOrWhiteSpace(o))
            .ToList();

        var result = string.Join("\n", lines);

        return result;
    }

    /// <summary>
    /// Renders the content of an XML element to plain text, including handling child nodes and references.
    /// </summary>
    /// <param name="element">The XML element to render.</param>
    /// <returns>The plain text representation of the XML element.</returns>
    private static string RenderXmlElement(XmlElement element)
    {
        var builder = new StringBuilder();

        var childNodes = element.ChildNodes.Cast<XmlNode>().ToList();

        foreach (var child in childNodes)
        {
            if (child is XmlText textNode)
            {
                builder.Append(RenderTextNode(textNode));
            }
            else if (child is XmlElement childElement)
            {
                if (childElement.Name == "see" || childElement.Name == "seealso")
                {
                    builder.Append(RenderReference(childElement));
                }
                else
                {
                    // For other elements, recursively render their content
                    builder.Append(Render(childElement));
                }
            }
        }

        var result = builder.ToString();

        return result;
    }

    /// <summary>
    /// Render a see/seealso reference.
    /// </summary>
    /// <param name="childElement"></param>
    /// <returns></returns>
    private static string RenderReference(XmlElement childElement)
    {
        var builder = new StringBuilder();

        // Handle see/seealso references
        var crefAttribute = childElement.Attributes["cref"];

        if (crefAttribute != null)
        {
            var crefValue = crefAttribute.Value;

            if (crefValue.StartsWith("T:") ||
                crefValue.StartsWith("P:") ||
                crefValue.StartsWith("M:") ||
                crefValue.StartsWith("F:"))
            {
                var lastIndexOfDot = crefValue.LastIndexOf('.');

                crefValue = crefValue.Substring(lastIndexOfDot + 1, crefValue.Length - lastIndexOfDot - 1);
            }

            builder.Append(crefValue);
        }
        else
        {
            // If no cref attribute, just use the inner text
            builder.Append(childElement.InnerText);
        }

        return builder.ToString();
    }

    /// <summary>
    /// Renders the content of an XML text node to plain text.
    /// </summary>
    /// <param name="textNode">The XML text node to render.</param>
    /// <returns>The plain text representation of the XML text node.</returns>
    private static string RenderTextNode(XmlText textNode)
    {
        return textNode.Value ?? "";
    }
}