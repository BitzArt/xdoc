using System.Text;
using System.Xml;

namespace BitzArt.XDoc.PlainText;

/// <summary>
/// Lightweight XML renderer that converts XML documentation to plain text.
/// This implementation, can only render the text content of the XML nodes, but not resolve and render references.
/// </summary>
public class SimplePlainTextRenderer : IXmlToPlainTextRenderer
{
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

        var text = "";

        var xmlNode = documentation.Inherited != null
            ? documentation.Inherited.RequirementNode
            : documentation.Node;

        text = Render(xmlNode);

        var result = Normalize(text);

        return result;
    }

    private string Render(XmlNode? node)
    {
        if (node == null)
        {
            return string.Empty;
        }

        if (node is XmlText textNode)
        {
            return RenderTextNode(textNode);
        }
        else if (node is XmlElement element)
        {
            return RenderXmlElement(element);
        }
        else
        {
            return node.InnerText;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
   private string RenderXmlElement(XmlElement element)
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
           string crefValue = crefAttribute.Value;
           
           // Remove the T: prefix if present (for type references)
           if (crefValue.StartsWith("T:"))
           {
               crefValue = crefValue[2..];
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

   private string RenderTextNode(XmlText textNode)
    {
        var result = textNode.Value ?? "";
        
        return result;
    }
}