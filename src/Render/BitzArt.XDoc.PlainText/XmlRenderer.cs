using System.Text;
using System.Xml;
using BitzArt.XDoc;
using BitzArt.XDoc.PlainText;

namespace BitzArt.XDoc.PlainText;

/// <summary>
/// Renders XML documentation to plain text.
/// <seealso href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/xml-documentation-comments">Microsoft specification</seealso>
/// </summary>
public class XmlRenderer : IXmlToPlainTextRenderer
{
    /// <summary>
    /// Converts an XML documentation node to the plain text.
    /// </summary>
    /// <param name="documentation"></param>
    /// <returns></returns>
    public string Render(MemberDocumentation? documentation)
    {
        if (documentation == null)
        {
            return string.Empty;
        }

        var text = Process(documentation);
        var normalizedText = Normalize(text);

        return normalizedText;
    }

    /// <summary>
    /// Normalize the input string by removing extra empty lines and trimming each line.
    /// </summary>
    internal static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var lines = input.Split('\n')
            .Select(line => line.TrimEnd())
            .ToList();

        var builder = new StringBuilder();

        var lastLineWasEmpty = false;

        foreach (var line in lines)
        {
            var currentLineIsEmpty = string.IsNullOrWhiteSpace(line);

            if (currentLineIsEmpty && lastLineWasEmpty)
            {
                continue;
            }

            builder.AppendLine(line);

            lastLineWasEmpty = currentLineIsEmpty;
        }

        var result = builder.ToString().Trim();

        return result;
    }


    private string Process(MemberDocumentation? documentation)
    {
        if (documentation is TypeDocumentation typeDocumentation)
        {
            return Process(typeDocumentation);
        }
        else if (documentation is PropertyDocumentation propertyDocumentation)
        {
            return Process(propertyDocumentation);
        }
        else if (documentation is FieldDocumentation fieldDocumentation)
        {
            return Process(fieldDocumentation);
        }
        else if (documentation is MethodDocumentation methodDocumentation)
        {
            return Process(methodDocumentation);
        }

        return string.Empty;
    }

    private string Process(TypeDocumentation typeDocumentation)
    {
        throw new NotImplementedException();
    }

    private string Process(FieldDocumentation fieldDocumentation)
    {
        throw new NotImplementedException();
    }

    private string Process(MethodDocumentation methodDocumentation)
    {
        throw new NotImplementedException();
    }

    private string Process(PropertyDocumentation propertyDocumentation)
    {
        throw new NotImplementedException();
    }


    internal string RenderXmlElement(XmlElement element)
    {
        switch (element.Name)
        {
            case "see":
            case "seealso":
                return RenderSeeBlock(element);

            case "paramref":
            case "typeparamref":
                return RenderRefBlock(element);

            case "typeparam":
                return RenderTypeParamBlock(element);

            case "param":
                return RenderParamBlock(element);

            case "returns":
                return RenderReturnsBlock(element);

            case "exception":
                return RenderExceptionBlock(element);

            case "value":
                return RenderValueBlock(element);

            case "list":
                return RenderListBlock(element);

            case "code":
                return RenderCodeBlock(element);

            case "c":
                return RenderCBlock(element);

            case "example":
            case "para":
            case "remarks":
            default:
                return RenderDefaultBlock(element);
        }
    }

    internal string RenderDefaultBlock(XmlElement element)
    {
        var builder = new StringBuilder();

        builder.AppendLine();
        builder.Append(ProcessChildren(element).Trim());

        return builder.ToString();
    }

    internal string RenderCBlock(XmlElement element)
    {
        return "`" + ProcessChildren(element).Trim() + "`";
    }

    internal string RenderReturnsBlock(XmlElement element)
    {
        var builder = new StringBuilder();
        builder.AppendLine();
        builder.Append("Returns: " + ProcessChildren(element).Trim());
        builder.AppendLine();

        return builder.ToString();
    }

    internal string RenderTypeParamBlock(XmlElement element)
    {
        var nameAttribute = element.Attributes["name"];

        if (nameAttribute != null)
        {
            return $"(Type: {nameAttribute?.Value}) ";
        }

        return string.Empty;
    }

    internal string RenderRefBlock(XmlElement element)
    {
        var nameAttribute = element.Attributes["name"];

        if (nameAttribute != null)
        {
            return nameAttribute.Value;
        }

        return string.Empty;
    }

    internal string RenderValueBlock(XmlElement element)
    {
        var builder = new StringBuilder();
        builder.AppendLine();
        builder.Append("Value: " + ProcessChildren(element).Trim());
        builder.AppendLine();

        return builder.ToString();
    }

    internal string RenderParamBlock(XmlElement element)
    {
        var builder = new StringBuilder();

        var nameAttribute = element.Attributes["name"];

        builder.AppendLine();
        builder.Append("(Parameter: " + nameAttribute?.Value + ") ");
        builder.Append(ProcessChildren(element).Trim());
        builder.AppendLine();

        return builder.ToString();
    }

    internal string RenderCodeBlock(XmlElement element)
    {
        var builder = new StringBuilder();

        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine("```");

        var codeContent = element.InnerText ?? "";

        builder.Append(codeContent);

        builder.AppendLine();
        builder.AppendLine("```");
        builder.AppendLine();

        return builder.ToString();
    }

    internal string RenderListBlock(XmlElement element)
    {
        var builder = new StringBuilder();

        builder.AppendLine();
        builder.AppendLine();

        var elementAttribute = element.Attributes["type"];
        var listType = elementAttribute?.Value;

        foreach (XmlNode child in element.ChildNodes)
        {
            if (child.Name != "item")
            {
                continue;
            }

            var itemText = ProcessChildren(child).Trim();

            var prefix = listType switch
            {
                "number" => "1. ",
                "bullet" => "– ",
                "table" => "* ",
                _ => "– "
            };

            builder.AppendLine(prefix + itemText);
        }

        builder.AppendLine();

        return builder.ToString();
    }

    internal string RenderExceptionBlock(XmlElement element)
    {
        var crefAttribute = element.Attributes["cref"];

        var builder = new StringBuilder();

        builder.AppendLine();
        builder.Append("Throws ");

        if (crefAttribute != null)
        {
            var value = crefAttribute.Value;

            if (value.StartsWith("T:"))
            {
                value = value[2..];
            }

            builder.Append(value);
        }

        builder.Append(": " + ProcessChildren(element).Trim());
        builder.AppendLine();

        return builder.ToString();
    }

    internal string RenderSeeBlock(XmlElement element)
    {
        var crefAttribute = element.Attributes["cref"];

        if (crefAttribute != null)
        {
            var crefValue = crefAttribute.Value;

            if (crefValue.StartsWith("T:"))
            {
                crefValue = crefValue[2..];
            }

            return crefValue;
        }

        return string.Empty;
    }

    internal static string RenderTextNode(XmlText textNode)
    {
        var builder = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(textNode.Value))
        {
            //Refactor it or remove
            var cleanedText = new StringBuilder();
            var lastWasWhitespace = false;

            foreach (var c in textNode.Value)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (!lastWasWhitespace)
                    {
                        cleanedText.Append(' ');
                        lastWasWhitespace = true;
                    }
                }
                else
                {
                    cleanedText.Append(c);
                    lastWasWhitespace = false;
                }
            }

            builder.Append(cleanedText.ToString());
        }
        else
        {
            builder.Append(textNode.Value);
        }

        return builder.ToString();
    }

    private string ProcessChildren(XmlNode node)
    {
        var builder = new StringBuilder();

        foreach (XmlNode child in node.ChildNodes)
        {
            throw new NotImplementedException();
            // builder.Append(Process(child));
        }

        var result = builder.ToString();

        return result;
    }
}