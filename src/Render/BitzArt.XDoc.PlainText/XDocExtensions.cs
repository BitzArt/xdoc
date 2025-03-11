using System.Text;

namespace BitzArt.XDoc.PlainText;

/// <summary>
/// Extension methods for <see cref="XDoc"/> objects which allow rendering documentation in plain text.
/// </summary>
public static class XDocExtensions
{
    private static readonly XmlRenderer Renderer = new XmlRenderer();
    
    /// <summary>
    /// Renders the documentation of a <see cref="Type"/> as plain text.
    /// </summary>
    /// <param name="documentation"></param>
    /// <returns></returns>
    public static string ToPlainText(this MemberDocumentation? documentation)
    {
        if (documentation == null)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();

        if (documentation.Inherited != null)
        {
            builder.AppendLine(ToPlainText(documentation.Inherited.Target));
        }
        else
        {
            builder.AppendLine(Renderer.Render(documentation));
        }

        if (documentation.References.Any())
        {
            builder.Append(RenderReference(documentation.References));
        }

        return builder.ToString().Trim();
    }

    private static string RenderReference(IReadOnlyCollection<MemberDocumentationReference> references)
    {
        var builder = new StringBuilder();

        builder.AppendLine();
        builder.AppendLine("References: ");

        foreach (var reference in references)
        {
            var name = ResolveName(reference.Target);
            
            builder.AppendLine($" – {name}");
            builder.AppendLine(ToPlainText(reference.Target));
            builder.AppendLine();
        }

        return builder.ToString();
    }

    private static string ResolveName(MemberDocumentation documentation)
    {
        throw new NotImplementedException();
    }
}